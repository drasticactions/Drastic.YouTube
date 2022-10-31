// <copyright file="StringExtensions.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace Drastic.YouTube.Converter.Utils.Extensions;

internal static class StringExtensions
{
    public static string? NullIfWhiteSpace(this string s) =>
        !string.IsNullOrWhiteSpace(s)
            ? s
            : null;
}