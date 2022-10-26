// <copyright file="ClosedCaptionPart.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;

namespace Drastic.YouTube.Videos.ClosedCaptions;

/// <summary>
/// Individual closed caption part contained within a track.
/// </summary>
public class ClosedCaptionPart
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClosedCaptionPart"/> class.
    /// Initializes an instance of <see cref="ClosedCaptionPart" />.
    /// </summary>
    public ClosedCaptionPart(string text, TimeSpan offset)
    {
        this.Text = text;
        this.Offset = offset;
    }

    /// <summary>
    /// Gets text displayed by the caption part.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets time at which the caption part starts being displayed (relative to the caption's own offset).
    /// </summary>
    public TimeSpan Offset { get; }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => this.Text;
}