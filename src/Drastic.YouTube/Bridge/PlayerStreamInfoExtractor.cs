// <copyright file="PlayerStreamInfoExtractor.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using Drastic.YouTube.Utils;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Bridge;

internal class PlayerStreamInfoExtractor : IStreamInfoExtractor
{
    private readonly JsonElement content;

    public PlayerStreamInfoExtractor(JsonElement content) => this.content = content;

    public int? TryGetItag() => Memo.Cache(this, () =>
        this.content
            .GetPropertyOrNull("itag")?
            .GetInt32OrNull());

    public string? TryGetUrl() => Memo.Cache(this, () =>
        this.content
            .GetPropertyOrNull("url")?
            .GetStringOrNull() ??

        this.TryGetCipherData()?.GetValueOrDefault("url"));

    public string? TryGetSignature() => Memo.Cache(this, () =>
        this.TryGetCipherData()?.GetValueOrDefault("s"));

    public string? TryGetSignatureParameter() => Memo.Cache(this, () =>
        this.TryGetCipherData()?.GetValueOrDefault("sp"));

    public long? TryGetContentLength() => Memo.Cache(this, () =>
        this.content
            .GetPropertyOrNull("contentLength")?
            .GetStringOrNull()?
            .ParseLongOrNull() ??

        this.TryGetUrl()?
            .Pipe(s => Regex.Match(s, @"[\?&]clen=(\d+)").Groups[1].Value)
            .NullIfWhiteSpace()?
            .ParseLongOrNull());

    public long? TryGetBitrate() => Memo.Cache(this, () =>
        this.content
            .GetPropertyOrNull("bitrate")?
            .GetInt64OrNull());

    public string? TryGetContainer() => Memo.Cache(this, () =>
        this.TryGetMimeType()?
            .SubstringUntil(";")
            .SubstringAfter("/"));

    public string? TryGetCodecs() => Memo.Cache(this, () =>
        this.TryGetMimeType()?
            .SubstringAfter("codecs=\"")
            .SubstringUntil("\""));

    public string? TryGetAudioCodec() => Memo.Cache(this, () =>
        this.IsAudioOnly()
            ? this.TryGetCodecs()
            : this.TryGetCodecs()?.SubstringAfter(", ").NullIfWhiteSpace());

    public string? TryGetVideoCodec() => Memo.Cache(this, () =>
    {
        var codec = this.IsAudioOnly()
            ? null
            : this.TryGetCodecs()?.SubstringUntil(", ").NullIfWhiteSpace();

        // "unknown" value indicates av01 codec
        if (string.Equals(codec, "unknown", StringComparison.OrdinalIgnoreCase))
        {
            return "av01.0.05M.08";
        }

        return codec;
    });

    public string? TryGetVideoQualityLabel() => Memo.Cache(this, () =>
        this.content
            .GetPropertyOrNull("qualityLabel")?
            .GetStringOrNull());

    public int? TryGetVideoWidth() => Memo.Cache(this, () =>
        this.content
            .GetPropertyOrNull("width")?
            .GetInt32OrNull());

    public int? TryGetVideoHeight() => Memo.Cache(this, () =>
        this.content
            .GetPropertyOrNull("height")?
            .GetInt32OrNull());

    public int? TryGetFramerate() => Memo.Cache(this, () =>
        this.content
            .GetPropertyOrNull("fps")?
            .GetInt32OrNull());

    private string? TryGetMimeType() => Memo.Cache(this, () =>
    this.content
        .GetPropertyOrNull("mimeType")?
        .GetStringOrNull());

    private bool IsAudioOnly() => Memo.Cache(this, () =>
        this.TryGetMimeType()?.StartsWith("audio/", StringComparison.OrdinalIgnoreCase) ?? false);

    private IReadOnlyDictionary<string, string>? TryGetCipherData() => Memo.Cache(this, () =>
        this.content
            .GetPropertyOrNull("cipher")?
            .GetStringOrNull()?
            .Pipe(Url.SplitQuery) ??

        this.content
            .GetPropertyOrNull("signatureCipher")?
            .GetStringOrNull()?
            .Pipe(Url.SplitQuery));
}