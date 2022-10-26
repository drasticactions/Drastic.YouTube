// <copyright file="IChannel.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Drastic.YouTube.Common;

namespace Drastic.YouTube.Channels;

/// <summary>
/// Properties shared by channel metadata resolved from different sources.
/// </summary>
public interface IChannel
{
    /// <summary>
    /// Gets channel ID.
    /// </summary>
    ChannelId Id { get; }

    /// <summary>
    /// Gets channel URL.
    /// </summary>
    string Url { get; }

    /// <summary>
    /// Gets channel title.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Gets channel thumbnails.
    /// </summary>
    IReadOnlyList<Thumbnail> Thumbnails { get; }
}