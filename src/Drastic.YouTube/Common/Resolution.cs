// <copyright file="Resolution.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;

namespace Drastic.YouTube.Common;

/// <summary>
/// Resolution of an image or a video.
/// </summary>
public readonly partial struct Resolution
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Resolution"/> struct.
    /// Initializes an instance of <see cref="Resolution" />.
    /// </summary>
    public Resolution(int width, int height)
    {
        this.Width = width;
        this.Height = height;
    }

    /// <summary>
    /// Gets canvas width (in pixels).
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets canvas height (in pixels).
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Gets canvas area (width multiplied by height).
    /// </summary>
    public int Area => this.Width * this.Height;

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"{this.Width}x{this.Height}";
}

public partial struct Resolution : IEquatable<Resolution>
{
    /// <summary>
    /// Equality check.
    /// </summary>
    public static bool operator ==(Resolution left, Resolution right) => left.Equals(right);

    /// <summary>
    /// Equality check.
    /// </summary>
    public static bool operator !=(Resolution left, Resolution right) => !(left == right);

    /// <inheritdoc />
    public bool Equals(Resolution other) => this.Width == other.Width && this.Height == other.Height;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Resolution other && this.Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(this.Width, this.Height);
}