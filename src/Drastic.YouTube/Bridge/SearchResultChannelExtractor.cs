// <copyright file="SearchResultChannelExtractor.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Drastic.YouTube.Utils;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Bridge;

internal class SearchResultChannelExtractor
{
    private readonly JsonElement content;

    public SearchResultChannelExtractor(JsonElement content) =>
        this.content = content;

    public string? TryGetChannelId() => Memo.Cache(this, () =>
        this.content
            .GetPropertyOrNull("channelId")?
            .GetStringOrNull());

    public string? TryGetChannelTitle() => Memo.Cache(this, () =>
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

    public IReadOnlyList<ThumbnailExtractor> GetChannelThumbnails() => Memo.Cache(this, () =>
        this.content
            .GetPropertyOrNull("thumbnail")?
            .GetPropertyOrNull("thumbnails")?
            .EnumerateArrayOrNull()?
            .Select(j => new ThumbnailExtractor(j))
            .ToArray() ??

        Array.Empty<ThumbnailExtractor>());
}