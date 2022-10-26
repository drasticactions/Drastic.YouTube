// <copyright file="ClosedCaptionTrack.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Drastic.YouTube.Videos.ClosedCaptions;

/// <summary>
/// Contains closed captions in a specific language.
/// </summary>
public class ClosedCaptionTrack
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClosedCaptionTrack"/> class.
    /// Initializes an instance of <see cref="ClosedCaptionTrack" />.
    /// </summary>
    public ClosedCaptionTrack(IReadOnlyList<ClosedCaption> captions)
    {
        this.Captions = captions;
    }

    /// <summary>
    /// Gets closed captions included in the track.
    /// </summary>
    public IReadOnlyList<ClosedCaption> Captions { get; }

    /// <summary>
    /// Gets the caption displayed at the specified point in time.
    /// Returns null if not found.
    /// </summary>
    /// <returns></returns>
    public ClosedCaption? TryGetByTime(TimeSpan time) =>
        this.Captions.FirstOrDefault(c => time >= c.Offset && time <= c.Offset + c.Duration);

    /// <summary>
    /// Gets the caption displayed at the specified point in time.
    /// </summary>
    /// <returns></returns>
    public ClosedCaption GetByTime(TimeSpan time) =>
        this.TryGetByTime(time) ??
        throw new InvalidOperationException($"No closed caption found at {time}.");
}