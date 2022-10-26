// <copyright file="PlaylistSearchResult.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Drastic.YouTube.Common;
using Drastic.YouTube.Playlists;

namespace Drastic.YouTube.Search;

/// <summary>
/// Metadata associated with a YouTube playlist returned by a search query.
/// </summary>
public class PlaylistSearchResult : ISearchResult, IPlaylist
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlaylistSearchResult"/> class.
    /// Initializes an instance of <see cref="PlaylistSearchResult" />.
    /// </summary>
    public PlaylistSearchResult(
        PlaylistId id,
        string title,
        Author? author,
        IReadOnlyList<Thumbnail> thumbnails)
    {
        this.Id = id;
        this.Title = title;
        this.Author = author;
        this.Thumbnails = thumbnails;
    }

    /// <inheritdoc />
    public PlaylistId Id { get; }

    /// <inheritdoc cref="IPlaylist.Url" />
    public string Url => $"https://www.youtube.com/playlist?list={this.Id}";

    /// <inheritdoc cref="IPlaylist.Title" />
    public string Title { get; }

    /// <inheritdoc />
    public Author? Author { get; }

    /// <inheritdoc />
    public IReadOnlyList<Thumbnail> Thumbnails { get; }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"Playlist ({this.Title})";
}