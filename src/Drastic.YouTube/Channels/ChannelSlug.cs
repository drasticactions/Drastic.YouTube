// <copyright file="ChannelSlug.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Text.RegularExpressions;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Channels;

/// <summary>
/// Represents a syntactically valid YouTube channel slug.
/// </summary>
public readonly partial struct ChannelSlug
{
    private ChannelSlug(string value) => this.Value = value;

    /// <summary>
    /// Gets raw slug value.
    /// </summary>
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString() => this.Value;
}

public readonly partial struct ChannelSlug
{
    /// <summary>
    /// Converts string to channel slug.
    /// </summary>
    public static implicit operator ChannelSlug(string channelSlugOrUrl) => Parse(channelSlugOrUrl);

    /// <summary>
    /// Converts channel slug to string.
    /// </summary>
    public static implicit operator string(ChannelSlug channelSlug) => channelSlug.ToString();

    /// <summary>
    /// Attempts to parse the specified string as a YouTube channel slug or custom URL.
    /// Returns null in case of failure.
    /// </summary>
    /// <returns></returns>
    public static ChannelSlug? TryParse(string? channelSlugOrUrl) =>
        TryNormalize(channelSlugOrUrl)?.Pipe(slug => new ChannelSlug(slug));

    /// <summary>
    /// Parses the specified string as a YouTube channel slug or custom url.
    /// </summary>
    /// <returns></returns>
    public static ChannelSlug Parse(string channelSlugOrUrl) =>
        TryParse(channelSlugOrUrl) ??
        throw new ArgumentException($"Invalid YouTube channel slug or custom URL '{channelSlugOrUrl}'.");

    private static bool IsValid(string channelSlug) =>
        channelSlug.All(char.IsLetterOrDigit);

    private static string? TryNormalize(string? channelSlugOrUrl)
    {
        if (string.IsNullOrWhiteSpace(channelSlugOrUrl))
        {
            return null;
        }

        // Slug
        // peepthisout
        if (IsValid(channelSlugOrUrl))
        {
            return channelSlugOrUrl;
        }

        // URL
        // https://www.youtube.com/c/peepthisout
        var regularMatch = Regex.Match(channelSlugOrUrl, @"youtube\..+?/c/(.*?)(?:\?|&|/|$)").Groups[1].Value;
        if (!string.IsNullOrWhiteSpace(regularMatch) && IsValid(regularMatch))
        {
            return regularMatch;
        }

        // Invalid input
        return null;
    }
}