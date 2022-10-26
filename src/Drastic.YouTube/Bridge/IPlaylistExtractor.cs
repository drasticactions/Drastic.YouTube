// <copyright file="IPlaylistExtractor.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Drastic.YouTube.Bridge;

internal interface IPlaylistExtractor
{
    string? TryGetPlaylistTitle();

    string? TryGetPlaylistAuthor();

    string? TryGetPlaylistChannelId();

    string? TryGetPlaylistDescription();

    IReadOnlyList<ThumbnailExtractor> GetPlaylistThumbnails();
}