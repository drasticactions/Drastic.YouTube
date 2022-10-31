// <copyright file="ClipDuration.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace Drastic.YouTube.Converter;

public class ClipDuration
{
    public ClipDuration(double startTime = 0, double endTime = 0)
    {
        this.StartTimeSeconds = startTime <= 0 ? 0 : startTime;
        this.EndTimeSeconds = endTime <= 0 ? 0 : endTime;
    }

    /// <summary>
    /// Gets the start time of the clip.
    /// </summary>
    public double StartTimeSeconds { get; }

    /// <summary>
    /// Gets the end time of the clip.
    /// </summary>
    public double EndTimeSeconds { get; }
}
