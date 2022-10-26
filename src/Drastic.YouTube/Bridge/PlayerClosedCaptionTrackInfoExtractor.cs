﻿// <copyright file="PlayerClosedCaptionTrackInfoExtractor.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Text.Json;
using Drastic.YouTube.Utils;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Bridge;

internal class PlayerClosedCaptionTrackInfoExtractor
{
    private readonly JsonElement content;

    public PlayerClosedCaptionTrackInfoExtractor(JsonElement content) => this.content = content;

    public string? TryGetUrl() => Memo.Cache(this, () =>
        this.content
            .GetPropertyOrNull("baseUrl")?
            .GetStringOrNull());

    public string? TryGetLanguageCode() => Memo.Cache(this, () =>
        this.content
            .GetPropertyOrNull("languageCode")?
            .GetStringOrNull());

    public string? TryGetLanguageName() => Memo.Cache(this, () =>
        this.content
            .GetPropertyOrNull("name")?
            .GetPropertyOrNull("simpleText")?
            .GetStringOrNull() ??

        this.content
            .GetPropertyOrNull("name")?
            .GetPropertyOrNull("runs")?
            .EnumerateArrayOrNull()?
            .Select(j => j.GetPropertyOrNull("text")?.GetStringOrNull())
            .WhereNotNull()
            .ConcatToString());

    public bool IsAutoGenerated() => Memo.Cache(this, () =>
        this.content
            .GetPropertyOrNull("vssId")?
            .GetStringOrNull()?
            .StartsWith("a.", StringComparison.OrdinalIgnoreCase) ?? false);
}