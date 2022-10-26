// <copyright file="Memo.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Drastic.YouTube.Utils;

// Helper utility used to cache the result of a function
internal static class Memo
{
    public static T Cache<T>(object owner, Func<T> getValue)
    {
        var cache = For<T>.GetCache(owner);
        var key = getValue.Method.GetHashCode();

        if (cache.TryGetValue(key, out var cachedValue))
        {
            return cachedValue;
        }

        var value = getValue();
        cache[key] = value;

        return value;
    }

    private static class For<T>
    {
        private static readonly ConditionalWeakTable<object, Dictionary<int, T>> CacheManifest = new();

        public static Dictionary<int, T> GetCache(object owner) =>
            CacheManifest.GetOrCreateValue(owner);
    }
}