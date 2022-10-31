// <copyright file="FFmpeg.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;
using Drastic.YouTube.Converter.Utils.Extensions;

namespace Drastic.YouTube.Converter;

// Ideally this should use named pipes and stream through stdout.
// However, named pipes aren't well supported on non-Windows OS and
// stdout streaming only works with some specific formats.
internal partial class FFmpeg
{
    private readonly string filePath;

    public FFmpeg(string filePath) => this.filePath = filePath;

    public async ValueTask ExecuteAsync(
        string arguments,
        IProgress<double>? progress,
        CancellationToken cancellationToken = default)
    {
        var stdErrBuffer = new StringBuilder();

        var stdErrPipe = PipeTarget.Merge(
            PipeTarget.ToStringBuilder(stdErrBuffer), // error data collector
            progress?.Pipe(p => new FFmpegProgressRouter(p)) ?? PipeTarget.Null); // progress

        var result = await Cli.Wrap(this.filePath)
            .WithArguments(arguments)
            .WithStandardErrorPipe(stdErrPipe)
            .WithValidation(CommandResultValidation.None)
            .ExecuteAsync(cancellationToken);

        if (result.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"FFmpeg exited with a non-zero exit code ({result.ExitCode})." +
                Environment.NewLine +

                "Arguments:" +
                Environment.NewLine +
                arguments +
                Environment.NewLine +

                "Standard error:" +
                Environment.NewLine +
                stdErrBuffer);
        }
    }
}

internal partial class FFmpeg
{
    private class FFmpegProgressRouter : PipeTarget
    {
        private readonly StringBuilder buffer = new();
        private readonly IProgress<double> output;

        private TimeSpan? totalDuration;
        private TimeSpan? lastOffset;

        public FFmpegProgressRouter(IProgress<double> output) => this.output = output;

        public override async Task CopyFromAsync(Stream source, CancellationToken cancellationToken = default)
        {
            using var reader = new StreamReader(source, Console.OutputEncoding, false, 1024, true);

            var buffer = new char[1024];
            int charsRead;

            while ((charsRead = await reader.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.buffer.Append(buffer, 0, charsRead);
                this.HandleBuffer();
            }
        }

        private TimeSpan? TryParseTotalDuration(string data) => data
            .Pipe(s => Regex.Match(s, @"Duration:\s(\d\d:\d\d:\d\d.\d\d)").Groups[1].Value)
            .NullIfWhiteSpace()?
            .Pipe(s => TimeSpan.ParseExact(s, "c", CultureInfo.InvariantCulture));

        private TimeSpan? TryParseCurrentOffset(string data) => data
            .Pipe(s => Regex.Matches(s, @"time=(\d\d:\d\d:\d\d.\d\d)")
                .Cast<Match>()
                .LastOrDefault()?
                .Groups[1]
                .Value)?
            .NullIfWhiteSpace()?
            .Pipe(s => TimeSpan.ParseExact(s, "c", CultureInfo.InvariantCulture));

        private void HandleBuffer()
        {
            var data = this.buffer.ToString();

            this.totalDuration ??= this.TryParseTotalDuration(data);
            if (this.totalDuration is null)
            {
                return;
            }

            var currentOffset = this.TryParseCurrentOffset(data);
            if (currentOffset is null || currentOffset == this.lastOffset)
            {
                return;
            }

            this.lastOffset = currentOffset;

            var progress = (
                currentOffset.Value.TotalMilliseconds / this.totalDuration.Value.TotalMilliseconds)
            .Clamp(0, 1);

            this.output.Report(progress);
        }
    }
}