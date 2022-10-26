// <copyright file="ChannelClient.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Drastic.YouTube.Bridge;
using Drastic.YouTube.Common;
using Drastic.YouTube.Exceptions;
using Drastic.YouTube.Playlists;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Channels;

/// <summary>
/// Operations related to YouTube channels.
/// </summary>
public class ChannelClient
{
    private readonly HttpClient http;
    private readonly ChannelController controller;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChannelClient"/> class.
    /// Initializes an instance of <see cref="ChannelClient" />.
    /// </summary>
    public ChannelClient(HttpClient http)
    {
        this.http = http;
        this.controller = new ChannelController(http);
    }

    /// <summary>
    /// Gets the metadata associated with the specified channel.
    /// </summary>
    /// <returns></returns>
    public async ValueTask<Channel> GetAsync(
        ChannelId channelId,
        CancellationToken cancellationToken = default) =>
        this.Extract(await this.controller.GetChannelPageAsync(channelId, cancellationToken));

    /// <summary>
    /// Gets the metadata associated with the channel of the specified user.
    /// </summary>
    /// <returns></returns>
    public async ValueTask<Channel> GetByUserAsync(
        UserName userName,
        CancellationToken cancellationToken = default) =>
        this.Extract(await this.controller.GetChannelPageAsync(userName, cancellationToken));

    /// <summary>
    /// Gets the metadata associated with the channel identified by the specified slug or custom URL.
    /// </summary>
    /// <returns></returns>
    public async ValueTask<Channel> GetBySlugAsync(
        ChannelSlug channelSlug,
        CancellationToken cancellationToken = default) =>
        this.Extract(await this.controller.GetChannelPageAsync(channelSlug, cancellationToken));

    /// <summary>
    /// Enumerates videos uploaded by the specified channel.
    /// </summary>
    /// <returns></returns>
    // TODO: should return <IVideo> sequence instead (breaking change)
    public IAsyncEnumerable<PlaylistVideo> GetUploadsAsync(
        ChannelId channelId,
        CancellationToken cancellationToken = default)
    {
        // Replace 'UC' in channel ID with 'UU'
        var playlistId = "UU" + channelId.Value.Substring(2);
        return new PlaylistClient(this.http).GetVideosAsync(playlistId, cancellationToken);
    }

    private Channel Extract(ChannelPageExtractor channelPage)
    {
        var channelId =
            channelPage.TryGetChannelId() ??
            throw new DrasticYouTubeException("Could not extract channel ID.");

        var title =
            channelPage.TryGetChannelTitle() ??
            throw new DrasticYouTubeException("Could not extract channel title.");

        var logoUrl =
            channelPage.TryGetChannelLogoUrl() ??
            throw new DrasticYouTubeException("Could not extract channel logo URL.");

        var logoSize = Regex
            .Matches(logoUrl, @"\bs(\d+)\b")
            .Cast<Match>()
            .LastOrDefault()?
            .Groups[1]
            .Value
            .NullIfWhiteSpace()?
            .ParseIntOrNull() ?? 100;

        var thumbnails = new[] { new Thumbnail(logoUrl, new Resolution(logoSize, logoSize)) };

        return new Channel(channelId, title, thumbnails);
    }
}