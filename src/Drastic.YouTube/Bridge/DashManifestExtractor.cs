// <copyright file="DashManifestExtractor.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Drastic.YouTube.Utils;

namespace Drastic.YouTube.Bridge;

internal partial class DashManifestExtractor
{
    private readonly XElement content;

    public DashManifestExtractor(XElement content) => this.content = content;

    public IReadOnlyList<IStreamInfoExtractor> GetStreams() => Memo.Cache(this, () =>
        this.content
            .Descendants("Representation")

            // Skip non-media representations (like "rawcc")
            // https://github.com/Tyrrrz/Drastic.YouTube/issues/546
            .Where(x => x
                .Attribute("id")?
                .Value
                .All(char.IsDigit) == true)

            // Skip segmented streams
            // https://github.com/Tyrrrz/Drastic.YouTube/issues/159
            .Where(x => x
                .Descendants("Initialization")
                .FirstOrDefault()?
                .Attribute("sourceURL")?
                .Value
                .Contains("sq/") != true)

            // Skip streams without codecs
            .Where(x => !string.IsNullOrWhiteSpace(x.Attribute("codecs")?.Value))
            .Select(x => new DashStreamInfoExtractor(x))
            .ToArray());
}

internal partial class DashManifestExtractor
{
    public static DashManifestExtractor Create(string raw) => new(Xml.Parse(raw));
}