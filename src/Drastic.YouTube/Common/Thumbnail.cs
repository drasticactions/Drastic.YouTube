// <copyright file="Thumbnail.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Drastic.YouTube.Videos;

namespace Drastic.YouTube.Common;

/// <summary>
/// Thumbnail image.
/// </summary>
public partial class Thumbnail
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Thumbnail"/> class.
    /// Initializes an instance of <see cref="Thumbnail" />.
    /// </summary>
    public Thumbnail(string url, Resolution resolution)
    {
        this.Url = url;
        this.Resolution = resolution;
    }

    /// <summary>
    /// Gets thumbnail URL.
    /// </summary>
    public string Url { get; }

    /// <summary>
    /// Gets thumbnail resolution.
    /// </summary>
    public Resolution Resolution { get; }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"Thumbnail ({this.Resolution})";
}

public partial class Thumbnail
{
    internal static IReadOnlyList<Thumbnail> GetDefaultSet(VideoId videoId) => new[]
    {
        new Thumbnail($"https://img.youtube.com/vi/{videoId}/default.jpg", new Resolution(120, 90)),
        new Thumbnail($"https://img.youtube.com/vi/{videoId}/mqdefault.jpg", new Resolution(320, 180)),
        new Thumbnail($"https://img.youtube.com/vi/{videoId}/hqdefault.jpg", new Resolution(480, 360)),
    };
}

/// <summary>
/// Extensions for <see cref="Thumbnail" />.
/// </summary>
public static class ThumbnailExtensions
{
    /// <summary>
    /// Gets the thumbnail with the highest resolution (by area).
    /// Returns null if the sequence is empty.
    /// </summary>
    /// <returns></returns>
    public static Thumbnail? TryGetWithHighestResolution(this IEnumerable<Thumbnail> thumbnails) =>
        thumbnails.OrderByDescending(t => t.Resolution.Area).FirstOrDefault();

    /// <summary>
    /// Gets the thumbnail with the highest resolution (by area).
    /// </summary>
    /// <returns></returns>
    public static Thumbnail GetWithHighestResolution(this IEnumerable<Thumbnail> thumbnails) =>
        thumbnails.TryGetWithHighestResolution() ??
        throw new InvalidOperationException("Input thumbnail collection is empty.");
}