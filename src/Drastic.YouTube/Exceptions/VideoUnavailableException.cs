// <copyright file="VideoUnavailableException.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace Drastic.YouTube.Exceptions;

/// <summary>
/// Exception thrown when the requested video is unavailable.
/// </summary>
public class VideoUnavailableException : VideoUnplayableException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoUnavailableException"/> class.
    /// Initializes an instance of <see cref="VideoUnavailableException" />.
    /// </summary>
    public VideoUnavailableException(string message)
        : base(message)
    {
    }
}