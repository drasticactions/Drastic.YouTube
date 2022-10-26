// <copyright file="DashStreamInfoExtractor.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Drastic.YouTube.Utils;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Bridge;

internal class DashStreamInfoExtractor : IStreamInfoExtractor
{
    private readonly XElement content;

    public DashStreamInfoExtractor(XElement content) => this.content = content;

    public int? TryGetItag() => Memo.Cache(this, () =>
        (int?)this.content.Attribute("id"));

    public string? TryGetUrl() => Memo.Cache(this, () =>
        (string?)this.content.Element("BaseURL"));

    // DASH streams don't have signatures
    public string? TryGetSignature() => null;

    // DASH streams don't have signatures
    public string? TryGetSignatureParameter() => null;

    public long? TryGetContentLength() => Memo.Cache(this, () =>
        (long?)this.content.Attribute("contentLength") ??

        this.TryGetUrl()?
            .Pipe(s => Regex.Match(s, @"[/\?]clen[/=](\d+)").Groups[1].Value)
            .NullIfWhiteSpace()?
            .ParseLongOrNull());

    public long? TryGetBitrate() => Memo.Cache(this, () =>
        (long?)this.content.Attribute("bandwidth"));

    public string? TryGetContainer() => Memo.Cache(this, () =>
        this.TryGetUrl()?
            .Pipe(s => Regex.Match(s, @"mime[/=]\w*%2F([\w\d]*)").Groups[1].Value)
            .Pipe(WebUtility.UrlDecode));

    public string? TryGetAudioCodec() => Memo.Cache(this, () =>
        this.IsAudioOnly()
            ? (string?)this.content.Attribute("codecs")
            : null);

    public string? TryGetVideoCodec() => Memo.Cache(this, () =>
        this.IsAudioOnly()
            ? null
            : (string?)this.content.Attribute("codecs"));

    public string? TryGetVideoQualityLabel() => null;

    public int? TryGetVideoWidth() => Memo.Cache(this, () =>
        (int?)this.content.Attribute("width"));

    public int? TryGetVideoHeight() => Memo.Cache(this, () =>
        (int?)this.content.Attribute("height"));

    public int? TryGetFramerate() => Memo.Cache(this, () =>
        (int?)this.content.Attribute("frameRate"));

    private bool IsAudioOnly() => Memo.Cache(this, () =>
    this.content.Element("AudioChannelConfiguration") is not null);
}