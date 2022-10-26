// <copyright file="YoutubeClient.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Net.Http;
using Drastic.YouTube.Channels;
using Drastic.YouTube.Playlists;
using Drastic.YouTube.Search;
using Drastic.YouTube.Utils;
using Drastic.YouTube.Videos;

namespace Drastic.YouTube;

/// <summary>
/// Client for interacting with YouTube.
/// </summary>
public class YoutubeClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="YoutubeClient"/> class.
    /// Initializes an instance of <see cref="YoutubeClient" />.
    /// </summary>
    public YoutubeClient(HttpClient http)
    {
        this.Videos = new VideoClient(http);
        this.Playlists = new PlaylistClient(http);
        this.Channels = new ChannelClient(http);
        this.Search = new SearchClient(http);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YoutubeClient"/> class.
    /// Initializes an instance of <see cref="YoutubeClient" />.
    /// </summary>
    public YoutubeClient()
        : this(Http.Client)
    {
    }

    /// <summary>
    /// Gets operations related to YouTube videos.
    /// </summary>
    public VideoClient Videos { get; }

    /// <summary>
    /// Gets operations related to YouTube playlists.
    /// </summary>
    public PlaylistClient Playlists { get; }

    /// <summary>
    /// Gets operations related to YouTube channels.
    /// </summary>
    public ChannelClient Channels { get; }

    /// <summary>
    /// Gets operations related to YouTube search.
    /// </summary>
    public SearchClient Search { get; }
}