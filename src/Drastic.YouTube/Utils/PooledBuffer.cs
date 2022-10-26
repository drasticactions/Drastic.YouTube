// <copyright file="PooledBuffer.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Buffers;

namespace Drastic.YouTube.Utils;

internal readonly struct PooledBuffer<T> : IDisposable
{
    public PooledBuffer(int minimumLength) =>
        this.Array = ArrayPool<T>.Shared.Rent(minimumLength);

    public T[] Array { get; }

    public void Dispose() =>
        ArrayPool<T>.Shared.Return(this.Array);
}

internal static class PooledBuffer
{
    public static PooledBuffer<byte> ForStream() => new(81920);
}