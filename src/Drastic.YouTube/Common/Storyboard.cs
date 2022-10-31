// <copyright file="Storyboard.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using Drastic.YouTube.Videos;

namespace Drastic.YouTube.Common;

/// <summary>
/// Storyboard Set.
/// In order as parsed from YouTube.
/// </summary>
public class StoryboardSet
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StoryboardSet"/> class.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="sb"></param>
    public StoryboardSet(VideoId id, IReadOnlyList<Storyboard> sb)
    {
        this.Id = id;
        this.Storyboards = sb;
    }

    /// <summary>
    /// Gets video ID.
    /// </summary>
    public VideoId Id { get; }

    /// <summary>
    /// Gets the list of storyboards.
    /// </summary>
    public IReadOnlyList<Storyboard> Storyboards { get; }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public override string ToString() => this.Storyboards.Any() ? $"Thumbnail Size ({this.Storyboards.First().Resolution.Width}:{this.Storyboards.First().Resolution.Height})" : "Empty Storyboard Set";
}

/// <summary>
/// Storyboard.
/// Thumbnails used by YouTube for its video scrubber.
/// Each is represented by a image broken up by tiles.
/// These can be seperated by using the columns/rows and dividing
/// by the width and height parameters, which represent the size of the thumbnail.
/// </summary>
public class Storyboard
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Storyboard"/> class.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="uri"></param>
    /// <param name="resolution"></param>
    /// <param name="columns"></param>
    /// <param name="rows"></param>
    /// <param name="start"></param>
    /// <param name="duration"></param>
    public Storyboard(VideoId id, Uri uri, Resolution resolution, int columns, int rows, double start, double duration)
    {
        this.Id = id;
        this.Url = uri;
        this.Start = start;
        this.Duration = duration;
        this.Resolution = resolution;
        this.Columns = columns;
        this.Rows = rows;
    }

    /// <summary>
    /// Gets video ID.
    /// </summary>
    public VideoId Id { get; }

    /// <summary>
    /// Gets the URL of the full storyboard image.
    /// </summary>
    public Uri Url { get; }

    /// <summary>
    /// Gets the start time of the storyboard.
    /// Used for parsing where the individual tiles
    /// start when shown in the thumbnail view.
    /// </summary>
    public double Start { get; }

    /// <summary>
    /// Gets the duration of each thumbnail.
    /// </summary>
    public double Duration { get; }

    /// <summary>
    /// Gets the resolution of the thumbnails.
    /// </summary>
    public Resolution Resolution { get; }

    /// <summary>
    /// Gets the total number of columns represented in the storyboard.
    /// </summary>
    public int Columns { get; }

    /// <summary>
    /// Gets the total number of rows represented in the storyboard.
    /// </summary>
    public int Rows { get; }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"Thumbnail Size ({this.Resolution.Width}:{this.Resolution.Height})";
}

/// <summary>
/// Storyboard Image.
/// Tile from a <see cref="Storyboard"/>.
/// Column and Row represent the column and row where the image came from.
/// </summary>
public class StoryboardImage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StoryboardImage"/> class.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="image"></param>
    /// <param name="resolution"></param>
    /// <param name="columns"></param>
    /// <param name="rows"></param>
    /// <param name="start"></param>
    /// <param name="duration"></param>
    public StoryboardImage(VideoId id, byte[] image, Resolution resolution, int columns, int rows, double start, double duration)
    {
        this.Id = id;
        this.Image = image;
        this.Start = start;
        this.Duration = duration;
        this.Resolution = resolution;
        this.Column = columns;
        this.Row = rows;
    }

    /// <summary>
    /// Gets video ID.
    /// </summary>
    public VideoId Id { get; }

    /// <summary>
    /// Gets the tiled storyboard image.
    /// Formated as JPEG.
    /// </summary>
    public byte[] Image { get; }

    /// <summary>
    /// Gets the start of the thumbnail.
    /// </summary>
    public double Start { get; }

    /// <summary>
    /// Gets the duration of how long the thumbnail should be shown.
    /// </summary>
    public double Duration { get; }

    /// <summary>
    /// Gets the resolution of the thumbnail.
    /// </summary>
    public Resolution Resolution { get; }

    /// <summary>
    /// Gets the column location of where the tile came from the base storyboard image.
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// Gets the row location of where the tile came from the base storyboard image.
    /// </summary>
    public int Row { get; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{this.Id.ToString()}_{this.Start}_{this.Duration}_{this.Column}_{this.Row}";
    }
}