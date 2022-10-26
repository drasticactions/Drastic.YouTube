// <copyright file="VideoSearchResult.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Drastic.YouTube.Common;
using Drastic.YouTube.Videos;

namespace Drastic.YouTube.Search;

/// <summary>
/// Metadata associated with a YouTube video returned by a search query.
/// </summary>
public class VideoSearchResult : ISearchResult, IVideo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoSearchResult"/> class.
    /// Initializes an instance of <see cref="VideoSearchResult" />.
    /// </summary>
    public VideoSearchResult(
        VideoId id,
        string title,
        Author author,
        TimeSpan? duration,
        IReadOnlyList<Thumbnail> thumbnails)
    {
        this.Id = id;
        this.Title = title;
        this.Author = author;
        this.Duration = duration;
        this.Thumbnails = thumbnails;
    }

    /// <inheritdoc />
    public VideoId Id { get; }

    /// <inheritdoc cref="IVideo.Url" />
    public string Url => $"https://www.youtube.com/watch?v={this.Id}";

    /// <inheritdoc cref="IVideo.Title" />
    public string Title { get; }

    /// <inheritdoc />
    public Author Author { get; }

    /// <inheritdoc />
    public TimeSpan? Duration { get; }

    /// <inheritdoc />
    public IReadOnlyList<Thumbnail> Thumbnails { get; }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"Video ({this.Title})";
}