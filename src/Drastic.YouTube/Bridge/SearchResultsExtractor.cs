// <copyright file="SearchResultsExtractor.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Drastic.YouTube.Utils;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Bridge;

internal partial class SearchResultsExtractor
{
    private readonly JsonElement content;

    public SearchResultsExtractor(JsonElement content) => this.content = content;

    public IReadOnlyList<SearchResultVideoExtractor> GetVideos() => Memo.Cache(this, () =>
        this.TryGetContentRoot()?
            .EnumerateDescendantProperties("videoRenderer")
            .Select(j => new SearchResultVideoExtractor(j))
            .ToArray() ??

        Array.Empty<SearchResultVideoExtractor>());

    public IReadOnlyList<SearchResultPlaylistExtractor> GetPlaylists() => Memo.Cache(this, () =>
        this.TryGetContentRoot()?
            .EnumerateDescendantProperties("playlistRenderer")
            .Select(j => new SearchResultPlaylistExtractor(j))
            .ToArray() ??

        Array.Empty<SearchResultPlaylistExtractor>());

    public IReadOnlyList<SearchResultChannelExtractor> GetChannels() => Memo.Cache(this, () =>
        this.TryGetContentRoot()?
            .EnumerateDescendantProperties("channelRenderer")
            .Select(j => new SearchResultChannelExtractor(j))
            .ToArray() ??

        Array.Empty<SearchResultChannelExtractor>());

    public string? TryGetContinuationToken() => Memo.Cache(this, () =>
        this.TryGetContentRoot()?
            .EnumerateDescendantProperties("continuationCommand")
            .FirstOrNull()?
            .GetPropertyOrNull("token")?
            .GetStringOrNull());

    // Search results response is incredibly inconsistent (5+ variations),
    // so we employ descendant searching, which is inefficient but resilient.
    private JsonElement? TryGetContentRoot() => Memo.Cache(this, () =>
        this.content.GetPropertyOrNull("contents") ??
        this.content.GetPropertyOrNull("onResponseReceivedCommands"));
}

internal partial class SearchResultsExtractor
{
    public static SearchResultsExtractor Create(string raw) => new(Json.Parse(raw));
}