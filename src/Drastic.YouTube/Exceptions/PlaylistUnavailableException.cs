// <copyright file="PlaylistUnavailableException.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace Drastic.YouTube.Exceptions;

/// <summary>
/// Exception thrown when the requested playlist is unavailable.
/// </summary>
public class PlaylistUnavailableException : DrasticYouTubeException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlaylistUnavailableException"/> class.
    /// Initializes an instance of <see cref="PlaylistUnavailableException" />.
    /// </summary>
    public PlaylistUnavailableException(string message)
        : base(message)
    {
    }
}