// <copyright file="IVideoStreamInfo.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Drastic.YouTube.Common;

namespace Drastic.YouTube.Videos.Streams;

/// <summary>
/// Metadata associated with a media stream that contains video.
/// </summary>
public interface IVideoStreamInfo : IStreamInfo
{
    /// <summary>
    /// Gets video codec.
    /// </summary>
    string VideoCodec { get; }

    /// <summary>
    /// Gets video quality.
    /// </summary>
    VideoQuality VideoQuality { get; }

    /// <summary>
    /// Gets video resolution.
    /// </summary>
    Resolution VideoResolution { get; }
}

/// <summary>
/// Extensions for <see cref="IVideoStreamInfo" />.
/// </summary>
public static class VideoStreamInfoExtensions
{
    /// <summary>
    /// Gets the video stream with the highest video quality (including framerate).
    /// Returns null if the sequence is empty.
    /// </summary>
    /// <returns></returns>
    public static IVideoStreamInfo? TryGetWithHighestVideoQuality(this IEnumerable<IVideoStreamInfo> streamInfos) =>
        streamInfos.OrderByDescending(s => s.VideoQuality).FirstOrDefault();

    /// <summary>
    /// Gets the video stream with the highest video quality (including framerate).
    /// </summary>
    /// <returns></returns>
    public static IVideoStreamInfo GetWithHighestVideoQuality(this IEnumerable<IVideoStreamInfo> streamInfos) =>
        streamInfos.TryGetWithHighestVideoQuality() ??
        throw new InvalidOperationException("Input stream collection is empty.");
}