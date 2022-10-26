// <copyright file="ClosedCaption.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Drastic.YouTube.Videos.ClosedCaptions;

/// <summary>
/// Individual closed caption contained within a track.
/// </summary>
public class ClosedCaption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClosedCaption"/> class.
    /// Initializes an instance of <see cref="ClosedCaption" />.
    /// </summary>
    public ClosedCaption(
        string text,
        TimeSpan offset,
        TimeSpan duration,
        IReadOnlyList<ClosedCaptionPart> parts)
    {
        this.Text = text;
        this.Offset = offset;
        this.Duration = duration;
        this.Parts = parts;
    }

    /// <summary>
    /// Gets text displayed by the caption.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets time at which the caption starts being displayed.
    /// </summary>
    public TimeSpan Offset { get; }

    /// <summary>
    /// Gets duration of time for which the caption is displayed.
    /// </summary>
    public TimeSpan Duration { get; }

    /// <summary>
    /// Gets caption parts (usually representing individual words).
    /// </summary>
    /// <remarks>
    /// May be empty because not all captions have parts.
    /// </remarks>
    public IReadOnlyList<ClosedCaptionPart> Parts { get; }

    /// <summary>
    /// Gets the caption part displayed at the specified point in time, relative to the caption's own offset.
    /// Returns null if not found.
    /// </summary>
    /// <returns></returns>
    public ClosedCaptionPart? TryGetPartByTime(TimeSpan time) =>
        this.Parts.FirstOrDefault(p => p.Offset >= time);

    /// <summary>
    /// Gets the caption part displayed at the specified point in time, relative to the caption's own offset.
    /// </summary>
    /// <returns></returns>
    public ClosedCaptionPart GetPartByTime(TimeSpan time) =>
        this.TryGetPartByTime(time) ??
        throw new InvalidOperationException($"No closed caption part found at {time}.");

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => this.Text;
}