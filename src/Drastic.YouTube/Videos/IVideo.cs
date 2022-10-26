// <copyright file="IVideo.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Drastic.YouTube.Common;

namespace Drastic.YouTube.Videos;

/// <summary>
/// Properties shared by video metadata resolved from different sources.
/// </summary>
public interface IVideo
{
    /// <summary>
    /// Gets video ID.
    /// </summary>
    VideoId Id { get; }

    /// <summary>
    /// Gets video URL.
    /// </summary>
    string Url { get; }

    /// <summary>
    /// Gets video title.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Gets video author.
    /// </summary>
    Author Author { get; }

    /// <summary>
    /// Gets video duration.
    /// </summary>
    /// <remarks>
    /// May be null if the video is a currently ongoing live stream.
    /// </remarks>
    TimeSpan? Duration { get; }

    /// <summary>
    /// Gets video thumbnails.
    /// </summary>
    IReadOnlyList<Thumbnail> Thumbnails { get; }
}