// <copyright file="AudioOnlyStreamInfo.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace Drastic.YouTube.Videos.Streams;

/// <summary>
/// Metadata associated with an audio-only YouTube media stream.
/// </summary>
public class AudioOnlyStreamInfo : IAudioStreamInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioOnlyStreamInfo"/> class.
    /// Initializes an instance of <see cref="AudioOnlyStreamInfo" />.
    /// </summary>
    public AudioOnlyStreamInfo(
        string url,
        Container container,
        FileSize size,
        Bitrate bitrate,
        string audioCodec)
    {
        this.Url = url;
        this.Container = container;
        this.Size = size;
        this.Bitrate = bitrate;
        this.AudioCodec = audioCodec;
    }

    /// <inheritdoc />
    public string Url { get; }

    /// <inheritdoc />
    public Container Container { get; }

    /// <inheritdoc />
    public FileSize Size { get; }

    /// <inheritdoc />
    public Bitrate Bitrate { get; }

    /// <inheritdoc />
    public string AudioCodec { get; }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"Audio-only ({this.Container})";
}