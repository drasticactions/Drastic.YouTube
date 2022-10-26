// <copyright file="Bitrate.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;

namespace Drastic.YouTube.Videos.Streams;

/// <summary>
/// Bitrate.
/// </summary>
public readonly partial struct Bitrate
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Bitrate"/> struct.
    /// Initializes an instance of <see cref="Bitrate" />.
    /// </summary>
    public Bitrate(long bitsPerSecond) => this.BitsPerSecond = bitsPerSecond;

    /// <summary>
    /// Gets bitrate in bits per second.
    /// </summary>
    public long BitsPerSecond { get; }

    /// <summary>
    /// Gets bitrate in kilobits per second.
    /// </summary>
    public double KiloBitsPerSecond => this.BitsPerSecond / 1024.0;

    /// <summary>
    /// Gets bitrate in megabits per second.
    /// </summary>
    public double MegaBitsPerSecond => this.KiloBitsPerSecond / 1024.0;

    /// <summary>
    /// Gets bitrate in gigabits per second.
    /// </summary>
    public double GigaBitsPerSecond => this.MegaBitsPerSecond / 1024.0;

    /// <inheritdoc />
    public override string ToString() => $"{this.GetLargestWholeNumberValue():0.##} {this.GetLargestWholeNumberSymbol()}";

    private string GetLargestWholeNumberSymbol()
    {
        if (Math.Abs(this.GigaBitsPerSecond) >= 1)
        {
            return "Gbit/s";
        }

        if (Math.Abs(this.MegaBitsPerSecond) >= 1)
        {
            return "Mbit/s";
        }

        if (Math.Abs(this.KiloBitsPerSecond) >= 1)
        {
            return "Kbit/s";
        }

        return "Bit/s";
    }

    private double GetLargestWholeNumberValue()
    {
        if (Math.Abs(this.GigaBitsPerSecond) >= 1)
        {
            return this.GigaBitsPerSecond;
        }

        if (Math.Abs(this.MegaBitsPerSecond) >= 1)
        {
            return this.MegaBitsPerSecond;
        }

        if (Math.Abs(this.KiloBitsPerSecond) >= 1)
        {
            return this.KiloBitsPerSecond;
        }

        return this.BitsPerSecond;
    }
}

public partial struct Bitrate : IComparable<Bitrate>, IEquatable<Bitrate>
{
    /// <summary>
    /// Equality check.
    /// </summary>
    public static bool operator ==(Bitrate left, Bitrate right) => left.Equals(right);

    /// <summary>
    /// Equality check.
    /// </summary>
    public static bool operator !=(Bitrate left, Bitrate right) => !(left == right);

    /// <summary>
    /// Comparison.
    /// </summary>
    public static bool operator <(Bitrate left, Bitrate right) => left.CompareTo(right) < 0;

    /// <summary>
    /// Comparison.
    /// </summary>
    public static bool operator >(Bitrate left, Bitrate right) => left.CompareTo(right) > 0;

    /// <inheritdoc />
    public int CompareTo(Bitrate other) => this.BitsPerSecond.CompareTo(other.BitsPerSecond);

    /// <inheritdoc />
    public bool Equals(Bitrate other) => this.CompareTo(other) == 0;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Bitrate other && this.Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(this.BitsPerSecond);
}