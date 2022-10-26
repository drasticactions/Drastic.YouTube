// <copyright file="SegmentedHttpStream.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Utils;

// Special abstraction that works around YouTube's stream throttling
// and provides seeking support.
internal class SegmentedHttpStream : Stream
{
    private readonly HttpClient http;
    private readonly string url;
    private readonly long? segmentSize;

    private Stream? segmentStream;
    private long actualPosition;

    public SegmentedHttpStream(HttpClient http, string url, long length, long? segmentSize)
    {
        this.url = url;
        this.http = http;
        this.Length = length;
        this.segmentSize = segmentSize;
    }

    [ExcludeFromCodeCoverage]
    public override bool CanRead => true;

    [ExcludeFromCodeCoverage]
    public override bool CanSeek => true;

    [ExcludeFromCodeCoverage]
    public override bool CanWrite => false;

    public override long Length { get; }

    public override long Position { get; set; }

    public async ValueTask InitializeAsync(CancellationToken cancellationToken = default) =>
        await this.ResolveSegmentAsync(cancellationToken);

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        while (true)
        {
            // Check if consumer changed position between reads
            if (this.actualPosition != this.Position)
            {
                this.ResetSegment();
            }

            // Check if finished reading (exit condition)
            if (this.Position >= this.Length)
            {
                return 0;
            }

            var stream = await this.ResolveSegmentAsync(cancellationToken);
            var bytesRead = await stream.ReadAsync(buffer, offset, count, cancellationToken);
            this.actualPosition = this.Position += bytesRead;

            if (bytesRead != 0)
            {
                return bytesRead;
            }

            // Reached the end of the segment, try to load the next one
            this.ResetSegment();
        }
    }

    [ExcludeFromCodeCoverage]
    public override int Read(byte[] buffer, int offset, int count) =>
        this.ReadAsync(buffer, offset, count).GetAwaiter().GetResult();

    [ExcludeFromCodeCoverage]
    public override long Seek(long offset, SeekOrigin origin) => this.Position = origin switch
    {
        SeekOrigin.Begin => offset,
        SeekOrigin.Current => this.Position + offset,
        SeekOrigin.End => this.Length + offset,
        _ => throw new ArgumentOutOfRangeException(nameof(origin)),
    };

    [ExcludeFromCodeCoverage]
    public override void Flush() =>
        throw new NotSupportedException();

    [ExcludeFromCodeCoverage]
    public override void SetLength(long value) =>
        throw new NotSupportedException();

    [ExcludeFromCodeCoverage]
    public override void Write(byte[] buffer, int offset, int count) =>
        throw new NotSupportedException();

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            this.ResetSegment();
        }
    }

    private void ResetSegment()
    {
        this.segmentStream?.Dispose();
        this.segmentStream = null;
    }

    private async ValueTask<Stream> ResolveSegmentAsync(CancellationToken cancellationToken = default)
    {
        if (this.segmentStream is not null)
        {
            return this.segmentStream;
        }

        var from = this.Position;

        var to = this.segmentSize is not null
            ? this.Position + this.segmentSize - 1
            : null;

        var stream = await this.http.GetStreamAsync(this.url, from, to, true, cancellationToken);

        return this.segmentStream = stream;
    }
}