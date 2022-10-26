// <copyright file="IStreamInfo.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Drastic.YouTube.Videos.Streams;

/// <summary>
/// Metadata associated with a media stream of a YouTube video.
/// </summary>
public interface IStreamInfo
{
    /// <summary>
    /// Gets stream URL.
    /// </summary>
    string Url { get; }

    /// <summary>
    /// Gets stream container.
    /// </summary>
    Container Container { get; }

    /// <summary>
    /// Gets stream size.
    /// </summary>
    FileSize Size { get; }

    /// <summary>
    /// Gets stream bitrate.
    /// </summary>
    Bitrate Bitrate { get; }
}

/// <summary>
/// Extensions for <see cref="IStreamInfo" />.
/// </summary>
public static class StreamInfoExtensions
{
    /// <summary>
    /// Gets the stream with the highest bitrate.
    /// Returns null if the sequence is empty.
    /// </summary>
    /// <returns></returns>
    public static IStreamInfo? TryGetWithHighestBitrate(this IEnumerable<IStreamInfo> streamInfos) =>
        streamInfos.OrderByDescending(s => s.Bitrate).FirstOrDefault();

    /// <summary>
    /// Gets the stream with the highest bitrate.
    /// </summary>
    /// <returns></returns>
    public static IStreamInfo GetWithHighestBitrate(this IEnumerable<IStreamInfo> streamInfos) =>
        streamInfos.TryGetWithHighestBitrate() ??
        throw new InvalidOperationException("Input stream collection is empty.");
}