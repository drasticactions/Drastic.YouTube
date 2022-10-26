// <copyright file="Engagement.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace Drastic.YouTube.Videos;

/// <summary>
/// Engagement statistics.
/// </summary>
public class Engagement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Engagement"/> class.
    /// Initializes an instance of <see cref="Engagement" />.
    /// </summary>
    public Engagement(long viewCount, long likeCount, long dislikeCount)
    {
        this.ViewCount = viewCount;
        this.LikeCount = likeCount;
        this.DislikeCount = dislikeCount;
    }

    /// <summary>
    /// Gets view count.
    /// </summary>
    public long ViewCount { get; }

    /// <summary>
    /// Gets like count.
    /// </summary>
    public long LikeCount { get; }

    /// <summary>
    /// Gets dislike count.
    /// </summary>
    /// <remarks>
    /// YouTube no longer supports dislikes, so this value is always 0.
    /// </remarks>
    public long DislikeCount { get; }

    /// <summary>
    /// Gets average rating.
    /// </summary>
    /// <remarks>
    /// YouTube no longer supports dislikes, so this value is always 5.
    /// </remarks>
    public double AverageRating => this.LikeCount + this.DislikeCount != 0
        ? 1 + (4.0 * this.LikeCount / (this.LikeCount + this.DislikeCount))
        : 0; // avoid division by 0

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"Rating: {this.AverageRating:N1}";
}