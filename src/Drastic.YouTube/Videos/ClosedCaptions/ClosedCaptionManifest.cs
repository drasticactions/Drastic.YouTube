// <copyright file="ClosedCaptionManifest.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Drastic.YouTube.Videos.ClosedCaptions;

/// <summary>
/// Contains information about available closed caption tracks on a YouTube video.
/// </summary>
public class ClosedCaptionManifest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClosedCaptionManifest"/> class.
    /// Initializes an instance of <see cref="ClosedCaptionManifest" />.
    /// </summary>
    public ClosedCaptionManifest(IReadOnlyList<ClosedCaptionTrackInfo> tracks)
    {
        this.Tracks = tracks;
    }

    /// <summary>
    /// Gets available closed caption tracks.
    /// </summary>
    public IReadOnlyList<ClosedCaptionTrackInfo> Tracks { get; }

    /// <summary>
    /// Gets the closed caption track in the specified language (identified by ISO-639-1 code or display name).
    /// Returns null if not found.
    /// </summary>
    /// <returns></returns>
    public ClosedCaptionTrackInfo? TryGetByLanguage(string language) =>
        this.Tracks.FirstOrDefault(t =>
            string.Equals(t.Language.Code, language, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(t.Language.Name, language, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Gets the closed caption track in the specified language (identified by ISO-639-1 code or display name).
    /// </summary>
    /// <returns></returns>
    public ClosedCaptionTrackInfo GetByLanguage(string language) =>
        this.TryGetByLanguage(language) ??
        throw new InvalidOperationException($"No closed caption track available for language '{language}'.");
}