// <copyright file="ClosedCaptionTrackExtractor.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Drastic.YouTube.Utils;

namespace Drastic.YouTube.Bridge;

internal partial class ClosedCaptionTrackExtractor
{
    private readonly XElement content;

    public ClosedCaptionTrackExtractor(XElement content) => this.content = content;

    public IReadOnlyList<ClosedCaptionExtractor> GetClosedCaptions() => Memo.Cache(this, () =>
        this.content
            .Descendants("p")
            .Select(x => new ClosedCaptionExtractor(x))
            .ToArray());
}

internal partial class ClosedCaptionTrackExtractor
{
    public static ClosedCaptionTrackExtractor Create(string raw) => new(Xml.Parse(raw));
}