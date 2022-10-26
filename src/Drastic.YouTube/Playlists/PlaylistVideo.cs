// <copyright file="PlaylistVideo.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Drastic.YouTube.Common;
using Drastic.YouTube.Videos;

namespace Drastic.YouTube.Playlists;

/// <summary>
/// Metadata associated with a YouTube video included in a playlist.
/// </summary>
public class PlaylistVideo : IVideo, IBatchItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlaylistVideo"/> class.
    /// Initializes an instance of <see cref="PlaylistVideo" />.
    /// </summary>
    public PlaylistVideo(
        PlaylistId playlistId,
        VideoId id,
        string title,
        Author author,
        TimeSpan? duration,
        IReadOnlyList<Thumbnail> thumbnails)
    {
        this.PlaylistId = playlistId;
        this.Id = id;
        this.Title = title;
        this.Author = author;
        this.Duration = duration;
        this.Thumbnails = thumbnails;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaylistVideo"/> class.
    /// Initializes an instance of <see cref="PlaylistVideo" />.
    /// </summary>
    // Binary backwards compatibility (PlaylistId was added)
    [Obsolete("Use the other constructor instead.")]
    [ExcludeFromCodeCoverage]
    public PlaylistVideo(
        VideoId id,
        string title,
        Author author,
        TimeSpan? duration,
        IReadOnlyList<Thumbnail> thumbnails)
        : this(default, id, title, author, duration, thumbnails)
    {
    }

    /// <summary>
    /// Gets iD of the playlist that contains this video.
    /// </summary>
    public PlaylistId PlaylistId { get; }

    /// <inheritdoc />
    public VideoId Id { get; }

    /// <inheritdoc />
    public string Url => $"https://www.youtube.com/watch?v={this.Id}&list={this.PlaylistId}";

    /// <inheritdoc />
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