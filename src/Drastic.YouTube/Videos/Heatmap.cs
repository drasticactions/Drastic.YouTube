// <copyright file="Heatmap.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace Drastic.YouTube.Videos;

/// <summary>
/// Heatmap statistics.
/// </summary>
public class Heatmap
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Heatmap"/> class.
    /// Initializes an instance of <see cref="Heatmap" />.
    /// </summary>
    public Heatmap(long timeRangeStartMillis, long markerDurationMillis, decimal heatMarkerIntensityScoreNormalized)
    {
        this.TimeRangeStartMillis = timeRangeStartMillis;
        this.MarkerDurationMillis = markerDurationMillis;
        this.HeatMarkerIntensityScoreNormalized = heatMarkerIntensityScoreNormalized;
    }

    /// <summary>
    /// Gets time Range Starting in Milliseconds.
    /// </summary>
    public long TimeRangeStartMillis { get; }

    /// <summary>
    /// Gets marker Duration starting in Milliseconds.
    /// </summary>
    public long MarkerDurationMillis { get; }

    /// <summary>
    /// Gets heat Marker Insensity Score, normalized.
    /// Range from 0 to 1.
    /// </summary>
    public decimal HeatMarkerIntensityScoreNormalized { get; }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"Heatmap: {this.TimeRangeStartMillis},{this.MarkerDurationMillis},{this.HeatMarkerIntensityScoreNormalized}";
}
