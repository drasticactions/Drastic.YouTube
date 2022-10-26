// <copyright file="Author.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using Drastic.YouTube.Channels;

namespace Drastic.YouTube.Common;

/// <summary>
/// Reference to a channel that owns a specific YouTube video or playlist.
/// </summary>
public class Author
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Author"/> class.
    /// Initializes an instance of <see cref="Author" />.
    /// </summary>
    public Author(ChannelId channelId, string channelTitle)
    {
        this.ChannelId = channelId;
        this.ChannelTitle = channelTitle;
    }

    /// <summary>
    /// Gets channel ID.
    /// </summary>
    public ChannelId ChannelId { get; }

    /// <summary>
    /// Gets channel URL.
    /// </summary>
    public string ChannelUrl => $"https://www.youtube.com/channel/{this.ChannelId}";

    /// <summary>
    /// Gets channel title.
    /// </summary>
    public string ChannelTitle { get; }

    /// <inheritdoc cref="ChannelTitle" />
    [Obsolete("Use ChannelTitle instead.")]
    [ExcludeFromCodeCoverage]
    public string Title => this.ChannelTitle;

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => this.ChannelTitle;
}