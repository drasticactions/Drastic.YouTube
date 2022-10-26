// <copyright file="UserName.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Text.RegularExpressions;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Channels;

/// <summary>
/// Represents a syntactically valid YouTube user name.
/// </summary>
public readonly partial struct UserName
{
    private UserName(string value) => this.Value = value;

    /// <summary>
    /// Gets raw user name value.
    /// </summary>
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString() => this.Value;
}

public partial struct UserName
{
    /// <summary>
    /// Converts string to user name.
    /// </summary>
    public static implicit operator UserName(string userNameOrUrl) => Parse(userNameOrUrl);

    /// <summary>
    /// Converts user name to string.
    /// </summary>
    public static implicit operator string(UserName userName) => userName.ToString();

    /// <summary>
    /// Attempts to parse the specified string as a YouTube user name or URL.
    /// Returns null in case of failure.
    /// </summary>
    /// <returns></returns>
    public static UserName? TryParse(string? userNameOrUrl) =>
        TryNormalize(userNameOrUrl)?.Pipe(name => new UserName(name));

    /// <summary>
    /// Parses the specified string as a YouTube user name or URL.
    /// </summary>
    /// <returns></returns>
    public static UserName Parse(string userNameOrUrl) =>
        TryParse(userNameOrUrl) ??
        throw new ArgumentException($"Invalid YouTube user name or profile URL '{userNameOrUrl}'.");

    private static bool IsValid(string userName) =>
        userName.Length <= 20 &&
        userName.All(char.IsLetterOrDigit);

    private static string? TryNormalize(string? userNameOrUrl)
    {
        if (string.IsNullOrWhiteSpace(userNameOrUrl))
        {
            return null;
        }

        // Name
        // TheTyrrr
        if (IsValid(userNameOrUrl))
        {
            return userNameOrUrl;
        }

        // URL
        // https://www.youtube.com/user/TheTyrrr
        var regularMatch = Regex.Match(userNameOrUrl, @"youtube\..+?/user/(.*?)(?:\?|&|/|$)").Groups[1].Value;
        if (!string.IsNullOrWhiteSpace(regularMatch) && IsValid(regularMatch))
        {
            return regularMatch;
        }

        // Invalid input
        return null;
    }
}

public partial struct UserName : IEquatable<UserName>
{
    /// <summary>
    /// Equality check.
    /// </summary>
    public static bool operator ==(UserName left, UserName right) => left.Equals(right);

    /// <summary>
    /// Equality check.
    /// </summary>
    public static bool operator !=(UserName left, UserName right) => !(left == right);

    /// <inheritdoc />
    public bool Equals(UserName other) => StringComparer.Ordinal.Equals(this.Value, other.Value);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is UserName other && this.Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(this.Value);
}