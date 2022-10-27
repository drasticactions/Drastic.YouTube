// <copyright file="Storyboard.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using Drastic.YouTube.Videos;

namespace Drastic.YouTube.Common;

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

    public IReadOnlyList<Storyboard> Storyboards { get; }
}

public partial class Storyboard
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

    public Uri Url { get; }

    public double Start { get; }

    public double Duration { get; }

    public int Width { get; }

    public int Height { get; }

    public int Columns { get; }

    public int Rows { get; }

    public override string ToString()
    {
        return $"{this.Id}_{this.Columns}_{this.Rows}";
    }
}

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

    public byte[] Image { get; }

    public double Start { get; }

    public double Duration { get; }

    public int Width { get; }

    public int Height { get; }

    public int Column { get; }

    public int Row { get; }

    public override string ToString()
    {
        return $"{this.Id.ToString()}_{this.Column}_{this.Row}";
    }
}