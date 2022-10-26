// <copyright file="MuxedStreamInfo.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Drastic.YouTube.Common;

namespace Drastic.YouTube.Videos.Streams;

/// <summary>
/// Metadata associated with a muxed (audio + video combined) media stream.
/// </summary>
public class MuxedStreamInfo : IAudioStreamInfo, IVideoStreamInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MuxedStreamInfo"/> class.
    /// Initializes an instance of <see cref="MuxedStreamInfo" />.
    /// </summary>
    public MuxedStreamInfo(
        string url,
        Container container,
        FileSize size,
        Bitrate bitrate,
        string audioCodec,
        string videoCodec,
        VideoQuality videoQuality,
        Resolution resolution)
    {
        this.Url = url;
        this.Container = container;
        this.Size = size;
        this.Bitrate = bitrate;
        this.AudioCodec = audioCodec;
        this.VideoCodec = videoCodec;
        this.VideoQuality = videoQuality;
        this.VideoResolution = resolution;
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
    public string VideoCodec { get; }

    /// <inheritdoc />
    public VideoQuality VideoQuality { get; }

    /// <inheritdoc />
    public Resolution VideoResolution { get; }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"Muxed ({this.VideoQuality} | {this.Container})";
}