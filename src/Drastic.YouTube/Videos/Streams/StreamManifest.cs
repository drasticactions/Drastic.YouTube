// <copyright file="StreamManifest.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace Drastic.YouTube.Videos.Streams;

/// <summary>
/// Contains information about available media streams on a YouTube video.
/// </summary>
public class StreamManifest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StreamManifest"/> class.
    /// Initializes an instance of <see cref="StreamManifest" />.
    /// </summary>
    public StreamManifest(IReadOnlyList<IStreamInfo> streams)
    {
        this.Streams = streams;
    }

    /// <summary>
    /// Gets available streams.
    /// </summary>
    public IReadOnlyList<IStreamInfo> Streams { get; }

    /// <summary>
    /// Gets streams that contain audio (i.e. muxed and audio-only streams).
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IAudioStreamInfo> GetAudioStreams() =>
        this.Streams.OfType<IAudioStreamInfo>();

    /// <summary>
    /// Gets streams that contain video (i.e. muxed and video-only streams).
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IVideoStreamInfo> GetVideoStreams() =>
        this.Streams.OfType<IVideoStreamInfo>();

    /// <summary>
    /// Gets muxed streams (i.e. streams containing both audio and video).
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MuxedStreamInfo> GetMuxedStreams() =>
        this.Streams.OfType<MuxedStreamInfo>();

    /// <summary>
    /// Gets audio-only streams.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<AudioOnlyStreamInfo> GetAudioOnlyStreams() =>
        this.GetAudioStreams().OfType<AudioOnlyStreamInfo>();

    /// <summary>
    /// Gets video-only streams.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<VideoOnlyStreamInfo> GetVideoOnlyStreams() =>
        this.GetVideoStreams().OfType<VideoOnlyStreamInfo>();
}