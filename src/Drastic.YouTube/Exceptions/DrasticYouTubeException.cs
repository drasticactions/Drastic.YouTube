// <copyright file="DrasticYouTubeException.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;

namespace Drastic.YouTube.Exceptions;

/// <summary>
/// Exception thrown within the library.
/// </summary>
public class DrasticYouTubeException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrasticYouTubeException"/> class.
    /// Initializes an instance of <see cref="DrasticYouTubeException" />.
    /// </summary>
    /// <param name="message"></param>
    public DrasticYouTubeException(string message)
        : base(message)
    {
    }
}