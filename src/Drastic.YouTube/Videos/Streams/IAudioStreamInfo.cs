// <copyright file="IAudioStreamInfo.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace Drastic.YouTube.Videos.Streams;

/// <summary>
/// Metadata associated with a media stream that contains audio.
/// </summary>
public interface IAudioStreamInfo : IStreamInfo
{
    /// <summary>
    /// Gets audio codec.
    /// </summary>
    string AudioCodec { get; }
}