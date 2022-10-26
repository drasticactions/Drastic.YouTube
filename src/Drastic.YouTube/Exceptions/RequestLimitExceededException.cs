// <copyright file="RequestLimitExceededException.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace Drastic.YouTube.Exceptions;

/// <summary>
/// Exception thrown when YouTube denies a request because the client has exceeded rate limit.
/// </summary>
public class RequestLimitExceededException : DrasticYouTubeException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequestLimitExceededException"/> class.
    /// Initializes an instance of <see cref="RequestLimitExceededException" />.
    /// </summary>
    public RequestLimitExceededException(string message)
        : base(message)
    {
    }
}