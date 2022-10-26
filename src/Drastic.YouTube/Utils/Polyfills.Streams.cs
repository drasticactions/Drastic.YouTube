// <copyright file="Polyfills.Streams.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

#if !NET5_0
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable SA1649 // File name should match first type name
internal static class StreamPolyfills
#pragma warning restore SA1649 // File name should match first type name
{
#if !NETSTANDARD2_1 && !NETCOREAPP3_0
    public static async Task<int> ReadAsync(this Stream stream, byte[] buffer, CancellationToken cancellationToken) =>
        await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
#endif

    public static async Task<Stream> ReadAsStreamAsync(
        this HttpContent httpContent,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await httpContent.ReadAsStreamAsync();
    }

    public static async Task<string> ReadAsStringAsync(
        this HttpContent httpContent,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await httpContent.ReadAsStringAsync();
    }
}
#endif