// <copyright file="InitialDataExtractor.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Drastic.YouTube.Utils;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Bridge;

internal partial class InitialDataExtractor
{
    private readonly JsonElement content;

    public InitialDataExtractor(JsonElement content) => this.content = content;

    public IReadOnlyList<HeatmarkerExtractor>? TryGetHeatmap() => Memo.Cache(this, () =>
        this.content
            .GetPropertyOrNull("playerOverlays")?
            .GetPropertyOrNull("playerOverlayRenderer")?
            .GetPropertyOrNull("decoratedPlayerBarRenderer")?
            .GetPropertyOrNull("decoratedPlayerBarRenderer")?
            .GetPropertyOrNull("playerBar")?
            .GetPropertyOrNull("multiMarkersPlayerBarRenderer")?
            .GetPropertyOrNull("markersMap")?
            .EnumerateArrayOrNull()?
            .FirstOrNull()?
            .GetPropertyOrNull("value")?
            .GetPropertyOrNull("heatmap")?
            .GetPropertyOrNull("heatmapRenderer")?
            .GetPropertyOrNull("heatMarkers")?
            .EnumerateArrayOrNull()?
            .Select(j => new HeatmarkerExtractor(j))
            .ToArray() ??
        Array.Empty<HeatmarkerExtractor>());
}
