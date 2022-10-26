// <copyright file="ChannelId.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Text.RegularExpressions;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Channels;

/// <summary>
/// Represents a syntactically valid YouTube channel ID.
/// </summary>
public readonly partial struct ChannelId
{
    private ChannelId(string value) => this.Value = value;

    /// <summary>
    /// Gets raw ID value.
    /// </summary>
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString() => this.Value;
}

public partial struct ChannelId
{
    /// <summary>
    /// Converts string to ID.
    /// </summary>
    public static implicit operator ChannelId(string channelIdOrUrl) => Parse(channelIdOrUrl);

    /// <summary>
    /// Converts ID to string.
    /// </summary>
    public static implicit operator string(ChannelId channelId) => channelId.ToString();

    /// <summary>
    /// Attempts to parse the specified string as a YouTube channel ID or URL.
    /// Returns null in case of failure.
    /// </summary>
    /// <returns></returns>
    public static ChannelId? TryParse(string? channelIdOrUrl) =>
        TryNormalize(channelIdOrUrl)?.Pipe(id => new ChannelId(id));

    /// <summary>
    /// Parses the specified string as a YouTube channel ID or URL.
    /// </summary>
    /// <returns></returns>
    public static ChannelId Parse(string channelIdOrUrl) =>
        TryParse(channelIdOrUrl) ??
        throw new ArgumentException($"Invalid YouTube channel ID or URL '{channelIdOrUrl}'.");

    private static bool IsValid(string channelId) =>
        channelId.StartsWith("UC", StringComparison.Ordinal) &&
        channelId.Length == 24 &&
        channelId.All(c => char.IsLetterOrDigit(c) || c is '_' or '-');

    private static string? TryNormalize(string? channelIdOrUrl)
    {
        if (string.IsNullOrWhiteSpace(channelIdOrUrl))
        {
            return null;
        }

        // Id
        // UC3xnGqlcL3y-GXz5N3wiTJQ
        if (IsValid(channelIdOrUrl))
        {
            return channelIdOrUrl;
        }

        // URL
        // https://www.youtube.com/channel/UC3xnGqlcL3y-GXz5N3wiTJQ
        var regularMatch = Regex.Match(channelIdOrUrl, @"youtube\..+?/channel/(.*?)(?:\?|&|/|$)").Groups[1].Value;
        if (!string.IsNullOrWhiteSpace(regularMatch) && IsValid(regularMatch))
        {
            return regularMatch;
        }

        // Invalid input
        return null;
    }
}

public partial struct ChannelId : IEquatable<ChannelId>
{
    /// <summary>
    /// Equality check.
    /// </summary>
    public static bool operator ==(ChannelId left, ChannelId right) => left.Equals(right);

    /// <summary>
    /// Equality check.
    /// </summary>
    public static bool operator !=(ChannelId left, ChannelId right) => !(left == right);

    /// <inheritdoc />
    public bool Equals(ChannelId other) => StringComparer.Ordinal.Equals(this.Value, other.Value);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is ChannelId other && this.Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(this.Value);
}