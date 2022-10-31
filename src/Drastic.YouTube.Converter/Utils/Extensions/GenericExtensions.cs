// <copyright file="GenericExtensions.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;

namespace Drastic.YouTube.Converter.Utils.Extensions;

internal static class GenericExtensions
{
    public static TOut Pipe<TIn, TOut>(this TIn input, Func<TIn, TOut> transform) => transform(input);

    public static T Clamp<T>(this T value, T min, T max)
        where T : IComparable<T>
        =>
        value.CompareTo(min) <= 0
            ? min
            : value.CompareTo(max) >= 0
                ? max
                : value;
}