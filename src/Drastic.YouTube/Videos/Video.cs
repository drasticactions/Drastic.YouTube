// <copyright file="Video.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Drastic.YouTube.Common;

namespace Drastic.YouTube.Videos;

/// <summary>
/// Metadata associated with a YouTube video.
/// </summary>
public class Video : IVideo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Video"/> class.
    /// Initializes an instance of <see cref="Video" />.
    /// </summary>
    public Video(
        VideoId id,
        string title,
        Author author,
        DateTimeOffset uploadDate,
        string description,
        TimeSpan? duration,
        IReadOnlyList<Thumbnail> thumbnails,
        IReadOnlyList<string> keywords,
        Engagement engagement,
        IReadOnlyList<Heatmap> heatmap,
        Uri? storyboardUri)
    {
        this.Id = id;
        this.Title = title;
        this.Author = author;
        this.UploadDate = uploadDate;
        this.Description = description;
        this.Duration = duration;
        this.Thumbnails = thumbnails;
        this.Keywords = keywords;
        this.Engagement = engagement;
        this.Heatmap = heatmap;
        this.StoryboardSpecUri = storyboardUri;
    }

    /// <inheritdoc />
    public VideoId Id { get; }

    /// <inheritdoc />
    public string Url => $"https://www.youtube.com/watch?v={this.Id}";

    /// <inheritdoc />
    public string Title { get; }

    /// <inheritdoc />
    public Author Author { get; }

    /// <summary>
    /// Gets video upload date.
    /// </summary>
    public DateTimeOffset UploadDate { get; }

    /// <summary>
    /// Gets video description.
    /// </summary>
    public string Description { get; }

    /// <inheritdoc />
    public TimeSpan? Duration { get; }

    /// <inheritdoc />
    public IReadOnlyList<Thumbnail> Thumbnails { get; }

    /// <summary>
    /// Gets available search keywords for the video.
    /// </summary>
    public IReadOnlyList<string> Keywords { get; }

    /// <summary>
    /// Gets list of the most replayed sections on a video.
    /// <see cref="Heatmap"/>
    /// May not be available on all videos.
    /// </summary>
    public IReadOnlyList<Heatmap> Heatmap { get; }

    /// <summary>
    /// Gets engagement statistics for the video.
    /// </summary>
    public Engagement Engagement { get; }

    /// <summary>
    /// Gets the storyboard spec URI.
    /// </summary>
    public Uri? StoryboardSpecUri { get; }

    /// <summary>
    /// Gets the list of video storyboards.
    /// </summary>
    public IReadOnlyList<StoryboardSet> Storyboards
        => this.GetVideoStoryboards();

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"Video ({this.Title})";
}

/// <summary>
/// Extensions for <see cref="Video" />.
/// </summary>
public static class VideoExtensions
{
    public static IReadOnlyList<StoryboardSet> GetVideoStoryboards(this Video video)
    {
        var set = new List<StoryboardSet>();
        if (video.StoryboardSpecUri is null)
        {
            return set;
        }

        var stringUri = video.StoryboardSpecUri.ToString();
        var specParts = stringUri.Split('|');

        var baseUrlSpec = specParts[0].Split('$')[0] + "{0}/";
        var sgpPart = specParts[0].Split("$N")[1];

        for (var i = 1; i < specParts.Length; i++)
        {
            var part = specParts[i];
            var specs = part.Split('#');
            var width = Convert.ToInt32(specs[0]);
            var height = Convert.ToInt32(specs[1]);
            var third = Convert.ToInt32(specs[2]);
            var forth = Convert.ToInt32(specs[3]);
            var fifth = Convert.ToInt32(specs[4]);
            var sixth = specs[5];
            var slug = specs[6];
            if (slug == "default")
            {
                continue;
            }

            var signPart = specs[7];
            double amountOfBoards = (double)Math.Ceiling((decimal)(third / (forth * fifth)));
            amountOfBoards = amountOfBoards <= 0 ? 1 : amountOfBoards;
            var seconds = video.Duration?.TotalSeconds ?? 0;
            double start = 0;
            var boardDir = seconds / amountOfBoards;

            var storyboardList = new List<Drastic.YouTube.Common.Storyboard>();

            for (var x = 0; x < amountOfBoards; x++)
            {
                var baseUrl = $"{string.Format(baseUrlSpec, i - 1)}M{x}{sgpPart}&sigh={signPart}";
                storyboardList.Add(new Drastic.YouTube.Common.Storyboard(video.Id, new Uri(baseUrl), width, height, forth, fifth, start, boardDir));
                start += boardDir;
            }

            set.Add(new StoryboardSet(video.Id, storyboardList));
        }

        return set.Where(n => n.Storyboards.Any()).ToList();
    }
}