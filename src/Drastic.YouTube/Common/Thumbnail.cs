// <copyright file="Thumbnail.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Drastic.YouTube.Exceptions;
using Drastic.YouTube.Utils;
using Drastic.YouTube.Videos;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

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
    public Thumbnail(string url, Resolution resolution, ThumbnailType type = ThumbnailType.Normal)
    {
        this.Url = url;
        this.Resolution = resolution;
        this.Type = type;
    }

    /// <summary>
    /// Gets thumbnail URL.
    /// </summary>
    public string Url { get; }

    /// <summary>
    /// Gets thumbnail type.
    /// </summary>
    public ThumbnailType Type { get; }

    /// <summary>
    /// Gets thumbnail resolution.
    /// </summary>
    public Resolution Resolution { get; }

    /// <summary>
    /// Download Thumbnail.
    /// </summary>
    /// <returns>Byte Array of Thumbnail.</returns>
    public async ValueTask<byte[]> DownloadAsync()
        => await Http.Client.GetByteArrayAsync(this.Url);

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"Thumbnail ({this.Resolution}) ({this.Type}) ({this.Url})";
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

    public static async ValueTask<byte[]> ToJpegAsync(this Thumbnail thumbnail)
    {
        using var bytes = await Http.Client.GetStreamAsync(thumbnail.Url);
        using var image = await Image.LoadAsync(bytes);
        using var ms = new MemoryStream();
        await image.SaveAsJpegAsync(ms);
        return ms.ToArray();
    }

    public static async ValueTask<byte[]> ToGifAsync(this Thumbnail thumbnail)
    {
        if (thumbnail.Type is ThumbnailType.Rich)
        {
            throw new DrasticYouTubeException("Does not support WebP Animations. Use DownloadAsync and convert it yourself.");
        }

        using var bytes = await Http.Client.GetStreamAsync(thumbnail.Url);
        using var image = await Image.LoadAsync(bytes);
        using var ms = new MemoryStream();
        await image.SaveAsGifAsync(ms);
        return ms.ToArray();
    }

    public static async ValueTask<byte[]> ToPngAsync(this Thumbnail thumbnail)
    {
        using var bytes = await Http.Client.GetStreamAsync(thumbnail.Url);
        using var image = await Image.LoadAsync(bytes);
        using var ms = new MemoryStream();
        await image.SaveAsPngAsync(ms);
        return ms.ToArray();
    }
}

/// <summary>
/// Thumbnail Type.
/// </summary>
public enum ThumbnailType
{
    /// <summary>
    /// Normal, static, thumbnail.
    /// </summary>
    Normal,

    /// <summary>
    /// Animated Thumbnail.
    /// </summary>
    Rich,
}