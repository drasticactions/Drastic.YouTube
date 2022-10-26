// <copyright file="IPlaylist.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Drastic.YouTube.Common;

namespace Drastic.YouTube.Playlists;

/// <summary>
/// Properties shared by playlist metadata resolved from different sources.
/// </summary>
public interface IPlaylist
{
    /// <summary>
    /// Gets playlist ID.
    /// </summary>
    PlaylistId Id { get; }

    /// <summary>
    /// Gets playlist URL.
    /// </summary>
    string Url { get; }

    /// <summary>
    /// Gets playlist title.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Gets playlist author.
    /// </summary>
    /// <remarks>
    /// May be null in case of auto-generated playlists (e.g. mixes, topics, etc).
    /// </remarks>
    Author? Author { get; }

    /// <summary>
    /// Gets playlist thumbnails.
    /// </summary>
    IReadOnlyList<Thumbnail> Thumbnails { get; }
}