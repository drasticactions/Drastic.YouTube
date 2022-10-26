// <copyright file="Container.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;

namespace Drastic.YouTube.Videos.Streams;

/// <summary>
/// Stream container.
/// </summary>
public readonly partial struct Container
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Container"/> struct.
    /// Initializes an instance of <see cref="Container" />.
    /// </summary>
    public Container(string name) => this.Name = name;

    /// <summary>
    /// Gets container name (e.g. mp4, webm, etc).
    /// Can be used as file extension.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets a value indicating whether whether this container is a known audio-only container.
    /// </summary>
    /// <remarks>
    /// This property only refers to the container's capabilities and not its actual contents.
    /// If the container IS audio-only, it DOES NOT contain any video streams.
    /// If the container IS NOT audio-only, it MAY contain video streams, but is not required to.
    /// </remarks>
    public bool IsAudioOnly =>
        string.Equals(this.Name, "mp3", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(this.Name, "m4a", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(this.Name, "wav", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(this.Name, "wma", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(this.Name, "ogg", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(this.Name, "aac", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(this.Name, "opus", StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc />
    public override string ToString() => this.Name;
}

public partial struct Container
{
    /// <summary>
    /// Gets mPEG-2 Audio Layer III (mp3).
    /// </summary>
    public static Container Mp3 { get; } = new("mp3");

    /// <summary>
    /// Gets mPEG-4 Part 14 (mp4).
    /// </summary>
    public static Container Mp4 { get; } = new("mp4");

    /// <summary>
    /// Gets web Media (webm).
    /// </summary>
    public static Container WebM { get; } = new("webm");

    /// <summary>
    /// Gets 3rd Generation Partnership Project (3gpp).
    /// </summary>
    public static Container Tgpp { get; } = new("3gpp");
}

public partial struct Container : IEquatable<Container>
{
    /// <summary>
    /// Equality check.
    /// </summary>
    public static bool operator ==(Container left, Container right) => left.Equals(right);

    /// <summary>
    /// Equality check.
    /// </summary>
    public static bool operator !=(Container left, Container right) => !(left == right);

    /// <inheritdoc />
    public bool Equals(Container other) => StringComparer.OrdinalIgnoreCase.Equals(this.Name, other.Name);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Container other && this.Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(this.Name);
}