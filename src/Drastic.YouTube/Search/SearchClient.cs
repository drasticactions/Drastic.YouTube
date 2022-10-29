// <copyright file="SearchClient.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using Drastic.YouTube.Common;
using Drastic.YouTube.Exceptions;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Search;

/// <summary>
/// Operations related to YouTube search.
/// </summary>
public class SearchClient
{
    private readonly SearchController controller;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchClient"/> class.
    /// Initializes an instance of <see cref="SearchClient" />.
    /// </summary>
    public SearchClient(HttpClient http) =>
        this.controller = new SearchController(http);

    /// <summary>
    /// Enumerates batches of search results returned by the specified query.
    /// </summary>
    /// <returns></returns>
    public async IAsyncEnumerable<Batch<ISearchResult>> GetResultBatchesAsync(
        string searchQuery,
        SearchFilter searchFilter,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var encounteredIds = new HashSet<string>(StringComparer.Ordinal);
        var continuationToken = default(string?);

        do
        {
            var results = new List<ISearchResult>();

            var searchResults = await this.controller.GetSearchResultsAsync(
                searchQuery,
                searchFilter,
                continuationToken,
                cancellationToken);

            foreach (var videoExtractor in searchResults.GetVideos())
            {
                if (searchFilter is not SearchFilter.None and not SearchFilter.Video)
                {
                    Debug.Fail("Did not expect videos in search results.");
                    break;
                }

                var id =
                    videoExtractor.TryGetVideoId() ??
                    throw new DrasticYouTubeException("Could not extract video ID.");

                // Don't yield the same result twice
                if (!encounteredIds.Add(id))
                {
                    continue;
                }

                var title =
                    videoExtractor.TryGetVideoTitle() ??
                    throw new DrasticYouTubeException("Could not extract video title.");

                var channelTitle =
                    videoExtractor.TryGetVideoAuthor() ??
                    throw new DrasticYouTubeException("Could not extract video author.");

                var channelId =
                    videoExtractor.TryGetVideoChannelId() ??
                    throw new DrasticYouTubeException("Could not extract video channel ID.");

                var richThumbnails = videoExtractor.GetVideoRichThumbnails();

                var standardThumbnails = videoExtractor.GetVideoThumbnails();

                var thumbnailSet = standardThumbnails.Concat(richThumbnails);

                var duration = videoExtractor.TryGetVideoDuration();

                var thumbnails = new List<Thumbnail>();

                thumbnails.AddRange(Thumbnail.GetDefaultSet(id));

                foreach (var thumbnailExtractor in thumbnailSet)
                {
                    var thumbnailUrl =
                        thumbnailExtractor.TryGetUrl() ??
                        throw new DrasticYouTubeException("Could not extract thumbnail URL.");

                    var thumbnailWidth =
                        thumbnailExtractor.TryGetWidth() ??
                        throw new DrasticYouTubeException("Could not extract thumbnail width.");

                    var thumbnailHeight =
                        thumbnailExtractor.TryGetHeight() ??
                        throw new DrasticYouTubeException("Could not extract thumbnail height.");

                    var thumbnailResolution = new Resolution(thumbnailWidth, thumbnailHeight);
                    var thumbnail = new Thumbnail(thumbnailUrl, thumbnailResolution, thumbnailExtractor.Type);
                    thumbnails.Add(thumbnail);
                }

                var video = new VideoSearchResult(
                    id,
                    title,
                    new Author(channelId, channelTitle),
                    duration,
                    thumbnails);

                results.Add(video);
            }

            foreach (var playlistExtractor in searchResults.GetPlaylists())
            {
                if (searchFilter is not SearchFilter.None and not SearchFilter.Playlist)
                {
                    Debug.Fail("Did not expect playlists in search results.");
                    break;
                }

                var id =
                    playlistExtractor.TryGetPlaylistId() ??
                    throw new DrasticYouTubeException("Could not extract playlist ID.");

                // Don't yield the same result twice
                if (!encounteredIds.Add(id))
                {
                    continue;
                }

                var title =
                    playlistExtractor.TryGetPlaylistTitle() ??
                    throw new DrasticYouTubeException("Could not extract playlist title.");

                // System playlists have no author
                var channelId = playlistExtractor.TryGetPlaylistChannelId();
                var channelTitle = playlistExtractor.TryGetPlaylistAuthor();
                var author = channelId is not null && channelTitle is not null
                    ? new Author(channelId, channelTitle)
                    : null;

                var thumbnails = new List<Thumbnail>();

                foreach (var thumbnailExtractor in playlistExtractor.GetPlaylistThumbnails())
                {
                    var thumbnailUrl =
                        thumbnailExtractor.TryGetUrl() ??
                        throw new DrasticYouTubeException("Could not extract thumbnail URL.");

                    var thumbnailWidth =
                        thumbnailExtractor.TryGetWidth() ??
                        throw new DrasticYouTubeException("Could not extract thumbnail width.");

                    var thumbnailHeight =
                        thumbnailExtractor.TryGetHeight() ??
                        throw new DrasticYouTubeException("Could not extract thumbnail height.");

                    var thumbnailResolution = new Resolution(thumbnailWidth, thumbnailHeight);
                    var thumbnail = new Thumbnail(thumbnailUrl, thumbnailResolution, thumbnailExtractor.Type);
                    thumbnails.Add(thumbnail);
                }

                var playlist = new PlaylistSearchResult(id, title, author, thumbnails);
                results.Add(playlist);
            }

            foreach (var channelExtractor in searchResults.GetChannels())
            {
                if (searchFilter is not SearchFilter.None and not SearchFilter.Channel)
                {
                    Debug.Fail("Did not expect channels in search results.");
                    break;
                }

                var channelId =
                    channelExtractor.TryGetChannelId() ??
                    throw new DrasticYouTubeException("Could not extract channel ID.");

                var title =
                    channelExtractor.TryGetChannelTitle() ??
                    throw new DrasticYouTubeException("Could not extract channel title.");

                var thumbnails = new List<Thumbnail>();

                foreach (var thumbnailExtractor in channelExtractor.GetChannelThumbnails())
                {
                    var thumbnailUrl =
                        thumbnailExtractor.TryGetUrl() ??
                        throw new DrasticYouTubeException("Could not extract thumbnail URL.");

                    var thumbnailWidth =
                        thumbnailExtractor.TryGetWidth() ??
                        throw new DrasticYouTubeException("Could not extract thumbnail width.");

                    var thumbnailHeight =
                        thumbnailExtractor.TryGetHeight() ??
                        throw new DrasticYouTubeException("Could not extract thumbnail height.");

                    var thumbnailResolution = new Resolution(thumbnailWidth, thumbnailHeight);
                    var thumbnail = new Thumbnail(thumbnailUrl, thumbnailResolution);
                    thumbnails.Add(thumbnail);
                }

                var channel = new ChannelSearchResult(channelId, title, thumbnails);
                results.Add(channel);
            }

            yield return Batch.Create(results);

            continuationToken = searchResults.TryGetContinuationToken();
        }
        while (!string.IsNullOrWhiteSpace(continuationToken));
    }

    /// <summary>
    /// Enumerates batches of search results returned by the specified query.
    /// </summary>
    /// <returns></returns>
    public IAsyncEnumerable<Batch<ISearchResult>> GetResultBatchesAsync(
        string searchQuery,
        CancellationToken cancellationToken = default) =>
        this.GetResultBatchesAsync(searchQuery, SearchFilter.None, cancellationToken);

    /// <summary>
    /// Enumerates search results returned by the specified query.
    /// </summary>
    /// <returns></returns>
    public IAsyncEnumerable<ISearchResult> GetResultsAsync(
        string searchQuery,
        CancellationToken cancellationToken = default) =>
        this.GetResultBatchesAsync(searchQuery, cancellationToken).FlattenAsync();

    /// <summary>
    /// Enumerates video search results returned by the specified query.
    /// </summary>
    /// <returns></returns>
    public IAsyncEnumerable<VideoSearchResult> GetVideosAsync(
        string searchQuery,
        CancellationToken cancellationToken = default) =>
        this.GetResultBatchesAsync(searchQuery, SearchFilter.Video, cancellationToken)
            .FlattenAsync()
            .OfTypeAsync<VideoSearchResult>();

    /// <summary>
    /// Enumerates playlist search results returned by the specified query.
    /// </summary>
    /// <returns></returns>
    public IAsyncEnumerable<PlaylistSearchResult> GetPlaylistsAsync(
        string searchQuery,
        CancellationToken cancellationToken = default) =>
        this.GetResultBatchesAsync(searchQuery, SearchFilter.Playlist, cancellationToken)
            .FlattenAsync()
            .OfTypeAsync<PlaylistSearchResult>();

    /// <summary>
    /// Enumerates channel search results returned by the specified query.
    /// </summary>
    /// <returns></returns>
    public IAsyncEnumerable<ChannelSearchResult> GetChannelsAsync(
        string searchQuery,
        CancellationToken cancellationToken = default) =>
        this.GetResultBatchesAsync(searchQuery, SearchFilter.Channel, cancellationToken)
            .FlattenAsync()
            .OfTypeAsync<ChannelSearchResult>();
}