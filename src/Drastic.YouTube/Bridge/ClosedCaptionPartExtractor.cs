// <copyright file="ClosedCaptionPartExtractor.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Xml.Linq;
using Drastic.YouTube.Utils;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Bridge;

internal class ClosedCaptionPartExtractor
{
    private readonly XElement content;

    public ClosedCaptionPartExtractor(XElement content) => this.content = content;

    public string? TryGetText() => Memo.Cache(this, () =>
        (string?)this.content);

    public TimeSpan? TryGetOffset() => Memo.Cache(this, () =>
        ((double?)this.content.Attribute("t"))?.Pipe(TimeSpan.FromMilliseconds) ??
        ((double?)this.content.Attribute("ac"))?.Pipe(TimeSpan.FromMilliseconds) ??
        TimeSpan.Zero);
}