// <copyright file="VideoClient.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Drastic.YouTube.Bridge;
using Drastic.YouTube.Common;
using Drastic.YouTube.Exceptions;
using Drastic.YouTube.Videos.ClosedCaptions;
using Drastic.YouTube.Videos.Storyboard;
using Drastic.YouTube.Videos.Streams;

namespace Drastic.YouTube.Videos;

/// <summary>
/// Operations related to YouTube videos.
/// </summary>
public class VideoClient
{
    private readonly VideoController controller;

    /// <summary>
    /// Initializes a new instance of the <see cref="VideoClient"/> class.
    /// Initializes an instance of <see cref="VideoClient" />.
    /// </summary>
    public VideoClient(HttpClient http)
    {
        this.controller = new VideoController(http);

        this.Streams = new StreamClient(http);
        this.ClosedCaptions = new ClosedCaptionClient(http);
        this.Storyboard = new StoryboardClient(http);
    }

    /// <summary>
    /// Gets operations related to media streams of YouTube videos.
    /// </summary>
    public StreamClient Streams { get; }

    /// <summary>
    /// Gets operations related to closed captions of YouTube videos.
    /// </summary>
    public ClosedCaptionClient ClosedCaptions { get; }

    /// <summary>
    /// Gets operations related to storyboards of YouTube videos.
    /// </summary>
    public Drastic.YouTube.Videos.Storyboard.StoryboardClient Storyboard { get; }

    /// <summary>
    /// Gets the metadata associated with the specified video.
    /// </summary>
    /// <returns></returns>
    public async ValueTask<Video> GetAsync(
        VideoId videoId,
        CancellationToken cancellationToken = default)
    {
        var watchPage = await this.controller.GetVideoWatchPageAsync(videoId, cancellationToken);

        var initialData = watchPage.TryGetInitialData();

        var chapters = initialData?
            .TryGetChapters()?
            .Select(t =>
            {
                var time =
                    t.TryGetTimeRangeStartMillis() ??
                    throw new DrasticYouTubeException("Could not extract time range.");

                var title =
                    t.TryGetTitle() ??
                    throw new DrasticYouTubeException("Could not extract title.");

                var thumbnailExtractor =
                    t.GetThumbnails() ??
                    throw new DrasticYouTubeException("Could not extract thumbnails.");

                var thumbnails = t
                    .GetThumbnails()
                        .Select(t =>
                            {
                                var thumbnailUrl =
                                    t.TryGetUrl() ??
                                    throw new DrasticYouTubeException("Could not extract thumbnail URL.");

                                var thumbnailWidth =
                                    t.TryGetWidth() ??
                                    throw new DrasticYouTubeException("Could not extract thumbnail width.");

                                var thumbnailHeight =
                                    t.TryGetHeight() ??
                                    throw new DrasticYouTubeException("Could not extract thumbnail height.");

                                var thumbnailResolution = new Resolution(thumbnailWidth, thumbnailHeight);

                                return new Thumbnail(thumbnailUrl, thumbnailResolution);
                            })
                    .ToList();

                return new ChapterDescription(title, time, thumbnails);
            }).ToArray()
            ?? Array.Empty<ChapterDescription>();

        // Heatmap could be empty or not exist for a given video.
        // If so, set it as empty.
        // If it does exist and we can't extract, then throw.
        var heatmap = initialData?
            .TryGetHeatmap()?
            .Select(t =>
            {
                var time =
                    t.TryGetTimeRangeStartMillis() ??
                    throw new DrasticYouTubeException("Could not extract time range.");

                var marker =
                    t.TryGetMarkerRangeStartMillis() ??
                    throw new DrasticYouTubeException("Could not extract marker range.");

                var score =
                    t.TryGetHeatMarkerIntensityScoreNormalized() ??
                    throw new DrasticYouTubeException("Could not extract intensity score.");

                return new Heatmap(time, marker, score);
            }).ToArray()
            ?? Array.Empty<Heatmap>();

        var playerResponse =
            watchPage.TryGetPlayerResponse() ??
            await this.controller.GetPlayerResponseAsync(videoId, cancellationToken);

        var storyboard =
            playerResponse.TryGetVideoStoryboard() ??
            throw new DrasticYouTubeException("Could not extract video storyboard.");

        var title =
            playerResponse.TryGetVideoTitle() ??
            throw new DrasticYouTubeException("Could not extract video title.");

        var channelTitle =
            playerResponse.TryGetVideoAuthor() ??
            throw new DrasticYouTubeException("Could not extract video author.");

        var channelId =
            playerResponse.TryGetVideoChannelId() ??
            throw new DrasticYouTubeException("Could not extract video channel ID.");

        var uploadDate =
            playerResponse.TryGetVideoUploadDate() ??
            throw new DrasticYouTubeException("Could not extract video upload date.");

        var description = playerResponse.TryGetVideoDescription() ?? string.Empty;
        var duration = playerResponse.TryGetVideoDuration();

        var thumbnails = playerResponse
            .GetVideoThumbnails()
            .Select(t =>
            {
                var thumbnailUrl =
                    t.TryGetUrl() ??
                    throw new DrasticYouTubeException("Could not extract thumbnail URL.");

                var thumbnailWidth =
                    t.TryGetWidth() ??
                    throw new DrasticYouTubeException("Could not extract thumbnail width.");

                var thumbnailHeight =
                    t.TryGetHeight() ??
                    throw new DrasticYouTubeException("Could not extract thumbnail height.");

                var thumbnailResolution = new Resolution(thumbnailWidth, thumbnailHeight);

                return new Thumbnail(thumbnailUrl, thumbnailResolution);
            })
            .Concat(Thumbnail.GetDefaultSet(videoId))
            .ToArray();

        var keywords = playerResponse.GetVideoKeywords();

        // Engagement statistics may be hidden
        var viewCount = playerResponse.TryGetVideoViewCount() ?? 0;
        var likeCount = watchPage.TryGetVideoLikeCount() ?? 0;
        var dislikeCount = watchPage.TryGetVideoDislikeCount() ?? 0;

        return new Video(
            videoId,
            title,
            new Author(channelId, channelTitle),
            uploadDate,
            description,
            duration,
            thumbnails,
            keywords,
            new Engagement(viewCount, likeCount, dislikeCount),
            heatmap,
            storyboard is not null ? new Uri(storyboard) : null,
            chapters);
    }
}