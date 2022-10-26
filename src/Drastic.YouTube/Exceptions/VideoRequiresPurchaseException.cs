// <copyright file="VideoRequiresPurchaseException.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.YouTube.Videos;

namespace Drastic.YouTube.Exceptions;

/// <summary>
/// Exception thrown when the requested video requires purchase.
/// </summary>
public class VideoRequiresPurchaseException : VideoUnplayableException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoRequiresPurchaseException"/> class.
    /// Initializes an instance of <see cref="VideoRequiresPurchaseException" />.
    /// </summary>
    public VideoRequiresPurchaseException(string message, VideoId previewVideoId)
        : base(message) =>
        this.PreviewVideoId = previewVideoId;

    /// <summary>
    /// Gets iD of a free preview video which is used as promotion for the original video.
    /// </summary>
    public VideoId PreviewVideoId { get; }
}