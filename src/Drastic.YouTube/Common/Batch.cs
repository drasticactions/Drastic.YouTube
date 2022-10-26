// <copyright file="Batch.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Common;

/// <summary>
/// Generic collection of items returned by a single request.
/// </summary>
public class Batch<T>
    where T : IBatchItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Batch{T}"/> class.
    /// Initializes an instance of <see cref="Batch{T}" />.
    /// </summary>
    public Batch(IReadOnlyList<T> items) => this.Items = items;

    /// <summary>
    /// Gets items included in the batch.
    /// </summary>
    public IReadOnlyList<T> Items { get; }
}

internal static class Batch
{
    public static Batch<T> Create<T>(IReadOnlyList<T> items)
        where T : IBatchItem
        => new(items);
}

internal static class BatchExtensions
{
    public static IAsyncEnumerable<T> FlattenAsync<T>(this IAsyncEnumerable<Batch<T>> source)
        where T : IBatchItem => source.SelectManyAsync(b => b.Items);
}