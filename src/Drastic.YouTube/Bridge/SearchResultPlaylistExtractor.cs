// <copyright file="SearchResultPlaylistExtractor.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Drastic.YouTube.Utils;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Bridge;

internal class SearchResultPlaylistExtractor
{
    private readonly JsonElement content;

    public SearchResultPlaylistExtractor(JsonElement content) =>
        this.content = content;

    public string? TryGetPlaylistId() => Memo.Cache(this, () =>
        this.content
            .GetPropertyOrNull("playlistId")?
            .GetStringOrNull());

    public string? TryGetPlaylistTitle() => Memo.Cache(this, () =>
        this.content
            .GetPropertyOrNull("title")?
            .GetPropertyOrNull("simpleText")?
            .GetStringOrNull() ??

        this.content
            .GetPropertyOrNull("title")?
            .GetPropertyOrNull("runs")?
            .EnumerateArrayOrNull()?
            .Select(j => j.GetPropertyOrNull("text")?.GetStringOrNull())
            .WhereNotNull()
            .ConcatToString());

    public string? TryGetPlaylistAuthor() => Memo.Cache(this, () =>
        this.TryGetPlaylistAuthorDetails()?
            .GetPropertyOrNull("text")?
            .GetStringOrNull());

    public string? TryGetPlaylistChannelId() => Memo.Cache(this, () =>
        this.TryGetPlaylistAuthorDetails()?
            .GetPropertyOrNull("navigationEndpoint")?
            .GetPropertyOrNull("browseEndpoint")?
            .GetPropertyOrNull("browseId")?
            .GetStringOrNull());

    public IReadOnlyList<ThumbnailExtractor> GetPlaylistThumbnails() => Memo.Cache(this, () =>
        this.content
            .GetPropertyOrNull("thumbnails")?
            .EnumerateDescendantProperties("thumbnails")
            .SelectMany(j => j.EnumerateArrayOrEmpty())
            .Select(j => new ThumbnailExtractor(j))
            .ToArray() ??

        Array.Empty<ThumbnailExtractor>());

    private JsonElement? TryGetPlaylistAuthorDetails() => Memo.Cache(this, () =>
        this.content
            .GetPropertyOrNull("longBylineText")?
            .GetPropertyOrNull("runs")?
            .EnumerateArrayOrNull()?
            .ElementAtOrNull(0));
}