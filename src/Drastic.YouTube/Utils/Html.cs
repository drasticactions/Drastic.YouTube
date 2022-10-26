// <copyright file="Html.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace Drastic.YouTube.Utils;

internal static class Html
{
    private static readonly HtmlParser HtmlParser = new();

    public static IHtmlDocument Parse(string source) => HtmlParser.ParseDocument(source);
}