// <copyright file="ChannelController.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Drastic.YouTube.Bridge;
using Drastic.YouTube.Exceptions;

namespace Drastic.YouTube.Channels;

internal class ChannelController : YoutubeControllerBase
{
    public ChannelController(HttpClient http)
        : base(http)
    {
    }

    public async ValueTask<ChannelPageExtractor> GetChannelPageAsync(
        ChannelId channelId,
        CancellationToken cancellationToken = default) =>
        await this.GetChannelPageAsync("channel/" + channelId, cancellationToken);

    public async ValueTask<ChannelPageExtractor> GetChannelPageAsync(
    UserName userName,
    CancellationToken cancellationToken = default) =>
    await this.GetChannelPageAsync("user/" + userName, cancellationToken);

    public async ValueTask<ChannelPageExtractor> GetChannelPageAsync(
        ChannelSlug channelSlug,
        CancellationToken cancellationToken = default) =>
        await this.GetChannelPageAsync("c/" + channelSlug, cancellationToken);

    private async ValueTask<ChannelPageExtractor> GetChannelPageAsync(
        string channelRoute,
        CancellationToken cancellationToken = default)
    {
        var url = $"https://www.youtube.com/{channelRoute}?hl=en";

        for (var retry = 0; retry <= 5; retry++)
        {
            var raw = await this.SendHttpRequestAsync(url, cancellationToken);

            var channelPage = ChannelPageExtractor.TryCreate(raw);
            if (channelPage is not null)
            {
                return channelPage;
            }
        }

        throw new DrasticYouTubeException(
            "Channel page is broken. " +
            "Please try again in a few minutes.");
    }
}