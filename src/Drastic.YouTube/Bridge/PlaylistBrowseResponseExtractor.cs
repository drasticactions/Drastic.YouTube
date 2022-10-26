// <copyright file="PlaylistBrowseResponseExtractor.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Drastic.YouTube.Utils;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Bridge;

internal partial class PlaylistBrowseResponseExtractor : IPlaylistExtractor
{
    private readonly JsonElement content;

    public PlaylistBrowseResponseExtractor(JsonElement content) => this.content = content;

    public bool IsPlaylistAvailable() => Memo.Cache(this, () =>
        this.TryGetSidebar() is not null);

    public string? TryGetPlaylistTitle() => Memo.Cache(this, () =>
        this.TryGetSidebarPrimary()?
            .GetPropertyOrNull("title")?
            .GetPropertyOrNull("simpleText")?
            .GetStringOrNull() ??

        this.TryGetSidebarPrimary()?
            .GetPropertyOrNull("title")?
            .GetPropertyOrNull("runs")?
            .EnumerateArrayOrNull()?
            .Select(j => j.GetPropertyOrNull("text")?.GetStringOrNull())
            .WhereNotNull()
            .ConcatToString());

    public string? TryGetPlaylistAuthor() => Memo.Cache(this, () =>
        this.TryGetPlaylistAuthorDetails()?
            .GetPropertyOrNull("title")?
            .GetPropertyOrNull("simpleText")?
            .GetStringOrNull() ??

        this.TryGetPlaylistAuthorDetails()?
            .GetPropertyOrNull("title")?
            .GetPropertyOrNull("runs")?
            .EnumerateArrayOrNull()?
            .Select(j => j.GetPropertyOrNull("text")?.GetStringOrNull())
            .WhereNotNull()
            .ConcatToString());

    public string? TryGetPlaylistChannelId() => Memo.Cache(this, () =>
        this.TryGetPlaylistAuthorDetails()?
            .GetPropertyOrNull("navigationEndpoint")?
            .GetPropertyOrNull("browseEndpoint")?
            .GetPropertyOrNull("browseId")?
            .GetStringOrNull());

    public string? TryGetPlaylistDescription() => Memo.Cache(this, () =>
        this.TryGetSidebarPrimary()?
            .GetPropertyOrNull("description")?
            .GetPropertyOrNull("simpleText")?
            .GetStringOrNull() ??

        this.TryGetSidebarPrimary()?
            .GetPropertyOrNull("description")?
            .GetPropertyOrNull("runs")?
            .EnumerateArrayOrNull()?
            .Select(j => j.GetPropertyOrNull("text")?.GetStringOrNull())
            .WhereNotNull()
            .ConcatToString());

    public IReadOnlyList<ThumbnailExtractor> GetPlaylistThumbnails() => Memo.Cache(this, () =>
        this.TryGetSidebarPrimary()?
            .GetPropertyOrNull("thumbnailRenderer")?
            .GetPropertyOrNull("playlistVideoThumbnailRenderer")?
            .GetPropertyOrNull("thumbnail")?
            .GetPropertyOrNull("thumbnails")?
            .EnumerateArrayOrNull()?
            .Select(j => new ThumbnailExtractor(j))
            .ToArray() ??

        this.TryGetSidebarPrimary()?
            .GetPropertyOrNull("thumbnailRenderer")?
            .GetPropertyOrNull("playlistCustomThumbnailRenderer")?
            .GetPropertyOrNull("thumbnail")?
            .GetPropertyOrNull("thumbnails")?
            .EnumerateArrayOrNull()?
            .Select(j => new ThumbnailExtractor(j))
            .ToArray() ??

        Array.Empty<ThumbnailExtractor>());

    private JsonElement? TryGetSidebar() => Memo.Cache(this, () =>
    this.content
        .GetPropertyOrNull("sidebar")?
        .GetPropertyOrNull("playlistSidebarRenderer")?
        .GetPropertyOrNull("items"));

    private JsonElement? TryGetSidebarPrimary() => Memo.Cache(this, () =>
        this.TryGetSidebar()?
            .EnumerateArrayOrNull()?
            .ElementAtOrNull(0)?
            .GetPropertyOrNull("playlistSidebarPrimaryInfoRenderer"));

    private JsonElement? TryGetSidebarSecondary() => Memo.Cache(this, () =>
        this.TryGetSidebar()?
            .EnumerateArrayOrNull()?
            .ElementAtOrNull(1)?
            .GetPropertyOrNull("playlistSidebarSecondaryInfoRenderer"));

    private JsonElement? TryGetPlaylistAuthorDetails() => Memo.Cache(this, () =>
        this.TryGetSidebarSecondary()?
            .GetPropertyOrNull("videoOwner")?
            .GetPropertyOrNull("videoOwnerRenderer"));
}

internal partial class PlaylistBrowseResponseExtractor
{
    public static PlaylistBrowseResponseExtractor Create(string raw) => new(Json.Parse(raw));
}