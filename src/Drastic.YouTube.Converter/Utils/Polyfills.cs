// <copyright file="Polyfills.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET461
using System.Collections.Generic;
using System.Linq;

internal static class PolyfillExtensions
{
    public static IEnumerable<(TFirst left, TSecond right)> Zip<TFirst, TSecond>(
        this IEnumerable<TFirst> first,
        IEnumerable<TSecond> second) =>
        first.Zip(second, (x, y) => (x, y));
}
#endif