// <copyright file="PlaylistVideoExtractor.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Drastic.YouTube.Utils;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Bridge;

internal class PlaylistVideoExtractor
{
    private static readonly string[] DurationFormats = { @"m\:ss", @"mm\:ss", @"h\:mm\:ss", @"hh\:mm\:ss" };

    private readonly JsonElement content;

    public PlaylistVideoExtractor(JsonElement content) => this.content = content;

    public int? TryGetIndex() => Memo.Cache(this, () =>
        this.content
            .GetPropertyOrNull("navigationEndpoint")?
            .GetPropertyOrNull("watchEndpoint")?
            .GetPropertyOrNull("index")?
            .GetInt32OrNull());

    public string? TryGetVideoId() => Memo.Cache(this, () =>
        this.content
            .GetPropertyOrNull("videoId")?
            .GetStringOrNull());

    public string? TryGetVideoTitle() => Memo.Cache(this, () =>
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

    public string? TryGetVideoAuthor() => Memo.Cache(this, () =>
        this.TryGetAuthorDetails()?
            .GetPropertyOrNull("text")?
            .GetStringOrNull());

    public string? TryGetVideoChannelId() => Memo.Cache(this, () =>
        this.TryGetAuthorDetails()?
            .GetPropertyOrNull("navigationEndpoint")?
            .GetPropertyOrNull("browseEndpoint")?
            .GetPropertyOrNull("browseId")?
            .GetStringOrNull());

    public TimeSpan? TryGetVideoDuration() => Memo.Cache(this, () =>
        this.content
            .GetPropertyOrNull("lengthSeconds")?
            .GetStringOrNull()?
            .ParseDoubleOrNull()?
            .Pipe(TimeSpan.FromSeconds) ??

        this.content
            .GetPropertyOrNull("lengthText")?
            .GetPropertyOrNull("simpleText")?
            .GetStringOrNull()?
            .ParseTimeSpanOrNull(DurationFormats) ??

        this.content
            .GetPropertyOrNull("lengthText")?
            .GetPropertyOrNull("runs")?
            .EnumerateArrayOrNull()?
            .Select(j => j.GetPropertyOrNull("text")?.GetStringOrNull())
            .WhereNotNull()
            .ConcatToString()
            .ParseTimeSpanOrNull(DurationFormats));

    public IReadOnlyList<ThumbnailExtractor> GetVideoThumbnails() => Memo.Cache(this, () =>
        this.content
            .GetPropertyOrNull("thumbnail")?
            .GetPropertyOrNull("thumbnails")?
            .EnumerateArrayOrNull()?
            .Select(j => new ThumbnailExtractor(j))
            .ToArray() ??

        Array.Empty<ThumbnailExtractor>());

    private JsonElement? TryGetAuthorDetails() => Memo.Cache(this, () =>
    this.content
        .GetPropertyOrNull("longBylineText")?
        .GetPropertyOrNull("runs")?
        .EnumerateArrayOrNull()?
        .ElementAtOrNull(0) ??

    this.content
        .GetPropertyOrNull("shortBylineText")?
        .GetPropertyOrNull("runs")?
        .EnumerateArrayOrNull()?
        .ElementAtOrNull(0));
}