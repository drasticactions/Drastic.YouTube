// <copyright file="ClosedCaptionController.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Drastic.YouTube.Bridge;
using Drastic.YouTube.Utils;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Videos.ClosedCaptions;

internal class ClosedCaptionController : VideoController
{
    public ClosedCaptionController(HttpClient http)
        : base(http)
    {
    }

    public async ValueTask<ClosedCaptionTrackExtractor> GetClosedCaptionTrackAsync(
        string url,
        CancellationToken cancellationToken = default)
    {
        // Enforce known format
        var urlWithFormat = url
            .Pipe(s => Url.SetQueryParameter(s, "format", "3"))
            .Pipe(s => Url.SetQueryParameter(s, "fmt", "3"));

        var raw = await this.SendHttpRequestAsync(urlWithFormat, cancellationToken);

        return ClosedCaptionTrackExtractor.Create(raw);
    }
}