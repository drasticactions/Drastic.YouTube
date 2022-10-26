// <copyright file="VideoWatchPageExtractor.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Drastic.YouTube.Utils;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Bridge;

internal partial class VideoWatchPageExtractor
{
    private readonly IHtmlDocument content;

    public VideoWatchPageExtractor(IHtmlDocument content) => this.content = content;

    public bool IsVideoAvailable() => Memo.Cache(this, () =>
        this.content.QuerySelector("meta[property=\"og:url\"]") is not null);

    public long? TryGetVideoLikeCount() => Memo.Cache(this, () =>
        this.content
            .Source
            .Text
            .Pipe(s => Regex.Match(s, @"""label""\s*:\s*""([\d,\.]+) likes""").Groups[1].Value)
            .NullIfWhiteSpace()?
            .StripNonDigit()
            .ParseLongOrNull());

    public long? TryGetVideoDislikeCount() => Memo.Cache(this, () =>
        this.content
            .Source
            .Text
            .Pipe(s => Regex.Match(s, @"""label""\s*:\s*""([\d,\.]+) dislikes""").Groups[1].Value)
            .NullIfWhiteSpace()?
            .StripNonDigit()
            .ParseLongOrNull());

    public InitialDataExtractor? TryGetInitialData() => Memo.Cache(this, () =>
        this.content
            .GetElementsByTagName("script")
            .Select(e => e.Text())
            .Select(s => Regex.Match(s, @"var\s+ytInitialData\s*=\s*(\{.*\})").Groups[1].Value)
            .FirstOrDefault(s => !string.IsNullOrWhiteSpace(s))?
            .NullIfWhiteSpace()?
            .Pipe(Json.Extract)?
            .Pipe(Json.TryParse)?
            .Pipe(j => new InitialDataExtractor(j)));

    public PlayerResponseExtractor? TryGetPlayerResponse() => Memo.Cache(this, () =>
        this.content
            .GetElementsByTagName("script")
            .Select(e => e.Text())
            .Select(s => Regex.Match(s, @"var\s+ytInitialPlayerResponse\s*=\s*(\{.*\})").Groups[1].Value)
            .FirstOrDefault(s => !string.IsNullOrWhiteSpace(s))?
            .NullIfWhiteSpace()?
            .Pipe(Json.Extract)
            .Pipe(Json.TryParse)?
            .Pipe(j => new PlayerResponseExtractor(j)) ??

        this.TryGetPlayerConfig()?
            .GetPropertyOrNull("args")?
            .GetPropertyOrNull("player_response")?
            .GetStringOrNull()?
            .Pipe(Json.TryParse)?
            .Pipe(j => new PlayerResponseExtractor(j)));

    private JsonElement? TryGetPlayerConfig() => Memo.Cache(this, () =>
        this.content
            .GetElementsByTagName("script")
            .Select(e => e.Text())
            .Select(s => Regex.Match(s, @"ytplayer\.config\s*=\s*(\{.*\})").Groups[1].Value)
            .FirstOrDefault(s => !string.IsNullOrWhiteSpace(s))?
            .NullIfWhiteSpace()?
            .Pipe(Json.Extract)
            .Pipe(Json.TryParse));
}

internal partial class VideoWatchPageExtractor
{
    public static VideoWatchPageExtractor? TryCreate(string raw)
    {
        var content = Html.Parse(raw);

        var isValid = content.Body?.QuerySelector("#player") is not null;
        if (!isValid)
        {
            return null;
        }

        return new VideoWatchPageExtractor(content);
    }
}