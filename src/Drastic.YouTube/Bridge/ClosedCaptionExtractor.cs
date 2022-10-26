// <copyright file="ClosedCaptionExtractor.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Drastic.YouTube.Utils;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Bridge;

internal class ClosedCaptionExtractor
{
    private readonly XElement content;

    public ClosedCaptionExtractor(XElement content) => this.content = content;

    public string? TryGetText() => Memo.Cache(this, () =>
        (string?)this.content);

    public TimeSpan? TryGetOffset() => Memo.Cache(this, () =>
        ((double?)this.content.Attribute("t"))?.Pipe(TimeSpan.FromMilliseconds));

    public TimeSpan? TryGetDuration() => Memo.Cache(this, () =>
        ((double?)this.content.Attribute("d"))?.Pipe(TimeSpan.FromMilliseconds));

    public IReadOnlyList<ClosedCaptionPartExtractor> GetParts() => Memo.Cache(this, () =>
        this.content
            .Elements("s")
            .Select(x => new ClosedCaptionPartExtractor(x))
            .ToArray());
}