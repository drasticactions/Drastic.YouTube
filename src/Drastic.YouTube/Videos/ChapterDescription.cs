// <copyright file="ChapterDescription.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.YouTube.Common;

namespace Drastic.YouTube.Videos;

/// <summary>
/// Chapter Descriptions.
/// </summary>
public class ChapterDescription
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChapterDescription"/> class.
    /// </summary>
    /// <param name="title">Chapter Title.</param>
    /// <param name="time">Timestamp Marker.</param>
    /// <param name="thumbnails">Thumbnails.</param>
    public ChapterDescription(string title, long time, IReadOnlyList<Thumbnail> thumbnails)
    {
        this.Title = title;
        this.TimeRangeStartMillis = time;
        this.Thumbnails = thumbnails;
    }

    /// <summary>
    /// Gets the chapter title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets time Range Starting in Milliseconds.
    /// </summary>
    public long TimeRangeStartMillis { get; }

    /// <summary>
    /// Gets the chapter description thumbnails.
    /// </summary>
    public IReadOnlyList<Thumbnail> Thumbnails { get; }
}
