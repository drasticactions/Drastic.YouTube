// <copyright file="Language.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;

namespace Drastic.YouTube.Videos.ClosedCaptions;

/// <summary>
/// Language information.
/// </summary>
public readonly partial struct Language
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Language"/> struct.
    /// Initializes an instance of <see cref="Language" />.
    /// </summary>
    public Language(string code, string name)
    {
        this.Code = code;
        this.Name = name;
    }

    /// <summary>
    /// Gets iSO 639-1 code of the language.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets full international name of the language.
    /// </summary>
    public string Name { get; }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"{this.Code} ({this.Name})";
}

public partial struct Language : IEquatable<Language>
{
    /// <summary>
    /// Equality check.
    /// </summary>
    public static bool operator ==(Language left, Language right) => left.Equals(right);

    /// <summary>
    /// Equality check.
    /// </summary>
    public static bool operator !=(Language left, Language right) => !(left == right);

    /// <inheritdoc />
    public bool Equals(Language other) => StringComparer.OrdinalIgnoreCase.Equals(this.Code, other.Code);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Language other && this.Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(this.Code);
}