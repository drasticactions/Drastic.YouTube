// <copyright file="ISearchResult.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.YouTube.Common;

namespace Drastic.YouTube.Search;

/// <summary>
/// <p>
///     Abstract result returned by a search query.
///     Use pattern matching to handle specific instances of this type.
/// </p>
/// <p>
///     Can be either one of the following:
///     <list type="bullet">
///         <item><see cref="VideoSearchResult" /></item>
///         <item><see cref="PlaylistSearchResult" /></item>
///         <item><see cref="ChannelSearchResult" /></item>
///     </list>
/// </p>
/// </summary>
public interface ISearchResult : IBatchItem
{
    /// <summary>
    /// Gets result URL.
    /// </summary>
    string Url { get; }

    /// <summary>
    /// Gets result title.
    /// </summary>
    string Title { get; }
}