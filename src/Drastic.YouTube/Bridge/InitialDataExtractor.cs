// <copyright file="InitialDataExtractor.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Text.Json;
using Drastic.YouTube.Utils;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Bridge;

internal partial class InitialDataExtractor
{
    private readonly JsonElement content;

    public InitialDataExtractor(JsonElement content) => this.content = content;

    public JsonElement.ArrayEnumerator? TryGetMarkersMap() => Memo.Cache(this, () =>
        this.content
            .GetPropertyOrNull("playerOverlays")?
            .GetPropertyOrNull("playerOverlayRenderer")?
            .GetPropertyOrNull("decoratedPlayerBarRenderer")?
            .GetPropertyOrNull("decoratedPlayerBarRenderer")?
            .GetPropertyOrNull("playerBar")?
            .GetPropertyOrNull("multiMarkersPlayerBarRenderer")?
            .GetPropertyOrNull("markersMap")?
            .EnumerateArrayOrNull());

    public IReadOnlyList<HeatmarkerExtractor>? TryGetHeatmap() => Memo.Cache(this, () =>
        this.TryGetMarkersMap()?
            .Where(n =>
                n.GetPropertyOrNull("key")
                ?.GetStringOrNull() == "HEATSEEKER")
            .FirstOrNull()?
            .GetPropertyOrNull("value")?
            .GetPropertyOrNull("heatmap")?
            .GetPropertyOrNull("heatmapRenderer")?
            .GetPropertyOrNull("heatMarkers")?
            .EnumerateArrayOrNull()?
            .Select(j => new HeatmarkerExtractor(j))
            .ToArray() ??
            Array.Empty<HeatmarkerExtractor>());

    public IReadOnlyList<ChapterRendererExtractor>? TryGetChapters() => Memo.Cache(this, () =>
     this.TryGetMarkersMap()?
         .Where(n =>
             n.GetPropertyOrNull("key")
             ?.GetStringOrNull() == "DESCRIPTION_CHAPTERS").FirstOrNull()?
         .GetPropertyOrNull("value")?
         .GetPropertyOrNull("chapters")?
         .EnumerateArrayOrNull()?
         .Select(j => new ChapterRendererExtractor(j))
         .ToArray() ??
         Array.Empty<ChapterRendererExtractor>());

    /// <inheritdoc/>
    public override string ToString()
    {
        return this.content.ToString();
    }
}
