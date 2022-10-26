// <copyright file="VideoOnlyStreamInfo.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Drastic.YouTube.Common;

namespace Drastic.YouTube.Videos.Streams;

/// <summary>
/// Metadata associated with a video-only media stream.
/// </summary>
public class VideoOnlyStreamInfo : IVideoStreamInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoOnlyStreamInfo"/> class.
    /// Initializes an instance of <see cref="VideoOnlyStreamInfo" />.
    /// </summary>
    public VideoOnlyStreamInfo(
        string url,
        Container container,
        FileSize size,
        Bitrate bitrate,
        string videoCodec,
        VideoQuality videoQuality,
        Resolution videoResolution)
    {
        this.Url = url;
        this.Container = container;
        this.Size = size;
        this.Bitrate = bitrate;
        this.VideoCodec = videoCodec;
        this.VideoQuality = videoQuality;
        this.VideoResolution = videoResolution;
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
    public string VideoCodec { get; }

    /// <inheritdoc />
    public VideoQuality VideoQuality { get; }

    /// <inheritdoc />
    public Resolution VideoResolution { get; }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"Video-only ({this.VideoQuality} | {this.Container})";
}