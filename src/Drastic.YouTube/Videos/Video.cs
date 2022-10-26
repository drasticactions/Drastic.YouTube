// <copyright file="Video.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
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
        IReadOnlyList<Heatmap> heatmap)
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

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"Video ({this.Title})";
}