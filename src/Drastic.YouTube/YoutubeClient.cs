// <copyright file="YoutubeClient.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Net.Http;
using Drastic.YouTube.Channels;
using Drastic.YouTube.Exceptions;
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

public static class YoutubeClientExtensions
{
    /// <summary>
    /// Get the Rich Thumbnail for a given video id.
    /// This requires using the Search command, as it is only provided in search or playlist results.
    /// </summary>
    /// <returns></returns>
    public static async ValueTask<Common.Thumbnail> GetRichThumbnailForVideoAsync(this YoutubeClient client, VideoId videoId)
    {
        // We need to call the search API to get the results. If we give it a video id, it should return the requested
        // video first, but if it doesn't we will ask for a few more and check there, just in case.
        var searchResults = (await client.Search.GetVideosAsync(videoId.ToString()).ToListAsync()).Take(5).FirstOrDefault(n => n.Id == videoId);

        if (searchResults is null)
        {
            throw new DrasticYouTubeException("Could not find Rich Thumbnail in search results");
        }

        return searchResults.Thumbnails.FirstOrDefault(n => n.Type == Common.ThumbnailType.Rich) ?? throw new DrasticYouTubeException("Could not find Rich Thumbnail in video thumbnail results");
    }
}