// <copyright file="FileSize.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;

namespace Drastic.YouTube.Videos.Streams;

// Loosely based on https://github.com/omar/ByteSize (MIT license)

/// <summary>
/// File size.
/// </summary>
public readonly partial struct FileSize
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileSize"/> struct.
    /// Initializes an instance of <see cref="FileSize" />.
    /// </summary>
    public FileSize(long bytes) => this.Bytes = bytes;

    /// <summary>
    /// Gets size in bytes.
    /// </summary>
    public long Bytes { get; }

    /// <summary>
    /// Gets size in kilobytes.
    /// </summary>
    public double KiloBytes => this.Bytes / 1024.0;

    /// <summary>
    /// Gets size in megabytes.
    /// </summary>
    public double MegaBytes => this.KiloBytes / 1024.0;

    /// <summary>
    /// Gets size in gigabytes.
    /// </summary>
    public double GigaBytes => this.MegaBytes / 1024.0;

    /// <inheritdoc />
    public override string ToString() => $"{this.GetLargestWholeNumberValue():0.##} {this.GetLargestWholeNumberSymbol()}";

    private string GetLargestWholeNumberSymbol()
    {
        if (Math.Abs(this.GigaBytes) >= 1)
        {
            return "GB";
        }

        if (Math.Abs(this.MegaBytes) >= 1)
        {
            return "MB";
        }

        if (Math.Abs(this.KiloBytes) >= 1)
        {
            return "KB";
        }

        return "B";
    }

    private double GetLargestWholeNumberValue()
    {
        if (Math.Abs(this.GigaBytes) >= 1)
        {
            return this.GigaBytes;
        }

        if (Math.Abs(this.MegaBytes) >= 1)
        {
            return this.MegaBytes;
        }

        if (Math.Abs(this.KiloBytes) >= 1)
        {
            return this.KiloBytes;
        }

        return this.Bytes;
    }
}

public partial struct FileSize : IComparable<FileSize>, IEquatable<FileSize>
{
    /// <summary>
    /// Equality check.
    /// </summary>
    public static bool operator ==(FileSize left, FileSize right) => left.Equals(right);

    /// <summary>
    /// Equality check.
    /// </summary>
    public static bool operator !=(FileSize left, FileSize right) => !(left == right);

    /// <summary>
    /// Comparison.
    /// </summary>
    public static bool operator >(FileSize left, FileSize right) => left.CompareTo(right) > 0;

    /// <summary>
    /// Comparison.
    /// </summary>
    public static bool operator <(FileSize left, FileSize right) => left.CompareTo(right) < 0;

    /// <inheritdoc />
    public int CompareTo(FileSize other) => this.Bytes.CompareTo(other.Bytes);

    /// <inheritdoc />
    public bool Equals(FileSize other) => this.CompareTo(other) == 0;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is FileSize other && this.Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(this.Bytes);
}