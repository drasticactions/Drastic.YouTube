// <copyright file="StreamController.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Drastic.YouTube.Bridge;

namespace Drastic.YouTube.Videos.Streams;

internal class StreamController : VideoController
{
    public StreamController(HttpClient http)
        : base(http)
    {
    }

    public async ValueTask<PlayerSourceExtractor?> TryGetPlayerSourceAsync(
        CancellationToken cancellationToken = default)
    {
        var iframeContent = await this.SendHttpRequestAsync(
            "https://www.youtube.com/iframe_api",
            cancellationToken);

        var version = Regex.Match(iframeContent, @"player\\?/([0-9a-fA-F]{8})\\?/").Groups[1].Value;
        if (string.IsNullOrWhiteSpace(version))
        {
            return null;
        }

        var source = await this.SendHttpRequestAsync(
            $"https://www.youtube.com/s/player/{version}/player_ias.vflset/en_US/base.js",
            cancellationToken);

        return PlayerSourceExtractor.Create(source);
    }

    public async ValueTask<DashManifestExtractor> GetDashManifestAsync(
        string url,
        CancellationToken cancellationToken = default)
    {
        var raw = await this.SendHttpRequestAsync(url, cancellationToken);
        return DashManifestExtractor.Create(raw);
    }
}