// <copyright file="Storyboard.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using Drastic.YouTube.Videos;

namespace Drastic.YouTube.Common;

/// <summary>
/// Storyboard Set.
/// In order as parsed from YouTube.
/// </summary>
public class StoryboardSet
{
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
    public Storyboard(VideoId id, Uri uri, int width, int height, int columns, int rows, double start, double duration)
    {
        this.Id = id;
        this.Url = uri;
        this.Start = start;
        this.Duration = duration;
        this.Width = width;
        this.Height = height;
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
    /// Gets the width of each thumbnail.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets the height of each thumbnail.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Gets the total number of columns represented in the storyboard.
    /// </summary>
    public int Columns { get; }

    /// <summary>
    /// Gets the total number of rows represented in the storyboard.
    /// </summary>
    public int Rows { get; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{this.Id}_{this.Columns}_{this.Rows}";
    }
}

/// <summary>
/// Storyboard Image.
/// Tile from a <see cref="Storyboard"/>.
/// Column and Row represent the column and row where the image came from.
/// </summary>
public class StoryboardImage
{
    public StoryboardImage(VideoId id, byte[] image, int width, int height, int columns, int rows, double start, double duration)
    {
        this.Image = image;
        this.Start = start;
        this.Duration = duration;
        this.Width = width;
        this.Height = height;
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
    /// Gets the width of the thumbnail.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets the height of the thumbnail.
    /// </summary>
    public int Height { get; }

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
        return $"{this.Id.ToString()}_{this.Column}_{this.Row}";
    }
}