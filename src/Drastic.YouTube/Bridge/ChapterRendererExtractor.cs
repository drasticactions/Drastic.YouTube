// <copyright file="ChapterRendererExtractor.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Text.Json;
using Drastic.YouTube.Utils;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Bridge;

internal class ChapterRendererExtractor
{
    private readonly JsonElement content;

    public ChapterRendererExtractor(JsonElement content) => this.content = content;

    public long? TryGetTimeRangeStartMillis() => Memo.Cache(this, () =>
        this.content.GetPropertyOrNull("chapterRenderer")?.GetPropertyOrNull("timeRangeStartMillis")?.GetInt64OrNull());

    public string? TryGetTitle() => Memo.Cache(this, () =>
        this.content.GetPropertyOrNull("chapterRenderer")?.GetPropertyOrNull("title")?.GetPropertyOrNull("simpleText")?.GetStringOrNull());

    public IReadOnlyList<ThumbnailExtractor> GetThumbnails() => Memo.Cache(this, () =>
        this.content.GetPropertyOrNull("chapterRenderer")?
            .GetPropertyOrNull("thumbnail")?
            .GetPropertyOrNull("thumbnails")?
            .EnumerateArrayOrNull()?
            .Select(j => new ThumbnailExtractor(j))
            .ToArray() ??

        Array.Empty<ThumbnailExtractor>());
}
