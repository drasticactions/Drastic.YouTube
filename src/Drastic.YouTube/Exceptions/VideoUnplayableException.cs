// <copyright file="VideoUnplayableException.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace Drastic.YouTube.Exceptions;

/// <summary>
/// Exception thrown when the requested video is unplayable.
/// </summary>
public class VideoUnplayableException : DrasticYouTubeException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoUnplayableException"/> class.
    /// Initializes an instance of <see cref="VideoUnplayableException" />.
    /// </summary>
    public VideoUnplayableException(string message)
        : base(message)
    {
    }
}