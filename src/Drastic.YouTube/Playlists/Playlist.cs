// <copyright file="Playlist.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Drastic.YouTube.Common;

namespace Drastic.YouTube.Playlists;

/// <summary>
/// Metadata associated with a YouTube playlist.
/// </summary>
public class Playlist : IPlaylist
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Playlist"/> class.
    /// Initializes an instance of <see cref="Playlist" />.
    /// </summary>
    public Playlist(
        PlaylistId id,
        string title,
        Author? author,
        string description,
        IReadOnlyList<Thumbnail> thumbnails)
    {
        this.Id = id;
        this.Title = title;
        this.Author = author;
        this.Description = description;
        this.Thumbnails = thumbnails;
    }

    /// <inheritdoc />
    public PlaylistId Id { get; }

    /// <inheritdoc />
    public string Url => $"https://www.youtube.com/playlist?list={this.Id}";

    /// <inheritdoc />
    public string Title { get; }

    /// <inheritdoc />
    public Author? Author { get; }

    /// <summary>
    /// Gets playlist description.
    /// </summary>
    public string Description { get; }

    /// <inheritdoc />
    public IReadOnlyList<Thumbnail> Thumbnails { get; }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"Playlist ({this.Title})";
}