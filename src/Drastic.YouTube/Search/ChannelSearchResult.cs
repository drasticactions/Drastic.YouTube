// <copyright file="ChannelSearchResult.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Drastic.YouTube.Channels;
using Drastic.YouTube.Common;

namespace Drastic.YouTube.Search;

/// <summary>
/// Metadata associated with a YouTube channel returned by a search query.
/// </summary>
public class ChannelSearchResult : ISearchResult, IChannel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChannelSearchResult"/> class.
    /// Initializes an instance of <see cref="ChannelSearchResult" />.
    /// </summary>
    public ChannelSearchResult(ChannelId id, string title, IReadOnlyList<Thumbnail> thumbnails)
    {
        this.Id = id;
        this.Title = title;
        this.Thumbnails = thumbnails;
    }

    /// <inheritdoc />
    public ChannelId Id { get; }

    /// <inheritdoc cref="IChannel.Url" />
    public string Url => $"https://www.youtube.com/channel/{this.Id}";

    /// <inheritdoc cref="IChannel.Title" />
    public string Title { get; }

    /// <inheritdoc />
    public IReadOnlyList<Thumbnail> Thumbnails { get; }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"Channel ({this.Title})";
}