// <copyright file="ThumbnailExtractor.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Text.Json;
using Drastic.YouTube.Utils;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Bridge;

internal class ThumbnailExtractor
{
    private readonly JsonElement content;

    public ThumbnailExtractor(JsonElement content) => this.content = content;

    public string? TryGetUrl() => Memo.Cache(this, () =>
        this.content.GetPropertyOrNull("url")?.GetStringOrNull());

    public int? TryGetWidth() => Memo.Cache(this, () =>
        this.content.GetPropertyOrNull("width")?.GetInt32OrNull());

    public int? TryGetHeight() => Memo.Cache(this, () =>
        this.content.GetPropertyOrNull("height")?.GetInt32OrNull());
}