// <copyright file="PlaylistNextResponseExtractor.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Drastic.YouTube.Utils;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Bridge;

internal partial class PlaylistNextResponseExtractor : IPlaylistExtractor
{
    private readonly JsonElement content;

    public PlaylistNextResponseExtractor(JsonElement content) => this.content = content;

    public bool IsPlaylistAvailable() => Memo.Cache(this, () =>
        this.TryGetPlaylistRoot() is not null);

    public string? TryGetPlaylistTitle() => Memo.Cache(this, () =>
        this.TryGetPlaylistRoot()?
            .GetPropertyOrNull("title")?
            .GetStringOrNull());

    public string? TryGetPlaylistAuthor() => Memo.Cache(this, () =>
        this.TryGetPlaylistRoot()?
            .GetPropertyOrNull("ownerName")?
            .GetPropertyOrNull("simpleText")?
            .GetStringOrNull());

    public string? TryGetPlaylistChannelId() => null;

    public string? TryGetPlaylistDescription() => null;

    public IReadOnlyList<ThumbnailExtractor> GetPlaylistThumbnails() => Memo.Cache(this, () =>
        this.GetVideos()
            .FirstOrDefault()?
            .GetVideoThumbnails() ??

        Array.Empty<ThumbnailExtractor>());

    public IReadOnlyList<PlaylistVideoExtractor> GetVideos() => Memo.Cache(this, () =>
        this.TryGetPlaylistRoot()?
            .GetPropertyOrNull("contents")?
            .EnumerateArrayOrNull()?
            .Select(j => j.GetPropertyOrNull("playlistPanelVideoRenderer"))
            .WhereNotNull()
            .Select(j => new PlaylistVideoExtractor(j))
            .ToArray() ??

        Array.Empty<PlaylistVideoExtractor>());

    public string? TryGetVisitorData() => Memo.Cache(this, () =>
        this.content
            .GetPropertyOrNull("responseContext")?
            .GetPropertyOrNull("visitorData")?
            .GetStringOrNull());

    private JsonElement? TryGetPlaylistRoot() => Memo.Cache(this, () =>
    this.content
        .GetPropertyOrNull("contents")?
        .GetPropertyOrNull("twoColumnWatchNextResults")?
        .GetPropertyOrNull("playlist")?
        .GetPropertyOrNull("playlist"));
}

internal partial class PlaylistNextResponseExtractor
{
    public static PlaylistNextResponseExtractor Create(string raw) => new(Json.Parse(raw));
}