// <copyright file="Channel.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Drastic.YouTube.Common;

namespace Drastic.YouTube.Channels;

/// <summary>
/// Metadata associated with a YouTube channel.
/// </summary>
public class Channel : IChannel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Channel"/> class.
    /// Initializes an instance of <see cref="Channel" />.
    /// </summary>
    public Channel(ChannelId id, string title, IReadOnlyList<Thumbnail> thumbnails)
    {
        this.Id = id;
        this.Title = title;
        this.Thumbnails = thumbnails;
    }

    /// <inheritdoc />
    public ChannelId Id { get; }

    /// <inheritdoc />
    public string Url => $"https://www.youtube.com/channel/{this.Id}";

    /// <inheritdoc />
    public string Title { get; }

    /// <inheritdoc />
    public IReadOnlyList<Thumbnail> Thumbnails { get; }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"Channel ({this.Title})";
}