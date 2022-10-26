// <copyright file="ChannelPageExtractor.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using AngleSharp.Html.Dom;
using Drastic.YouTube.Utils;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Bridge;

internal partial class ChannelPageExtractor
{
    private readonly IHtmlDocument content;

    public ChannelPageExtractor(IHtmlDocument content) => this.content = content;

    public string? TryGetChannelUrl() => Memo.Cache(this, () =>
        this.content
            .QuerySelector("meta[property=\"og:url\"]")?
            .GetAttribute("content"));

    public string? TryGetChannelId() => Memo.Cache(this, () =>
        this.TryGetChannelUrl()?.SubstringAfter("channel/", StringComparison.OrdinalIgnoreCase));

    public string? TryGetChannelTitle() => Memo.Cache(this, () =>
        this.content
            .QuerySelector("meta[property=\"og:title\"]")?
            .GetAttribute("content"));

    public string? TryGetChannelLogoUrl() => Memo.Cache(this, () =>
        this.content
            .QuerySelector("meta[property=\"og:image\"]")?
            .GetAttribute("content"));
}

internal partial class ChannelPageExtractor
{
    public static ChannelPageExtractor? TryCreate(string raw)
    {
        var content = Html.Parse(raw);

        var isValid = content.QuerySelector("meta[property=\"og:url\"]") is not null;
        if (!isValid)
        {
            return null;
        }

        return new ChannelPageExtractor(content);
    }
}