// <copyright file="HeatmarkerExtractor.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Text.Json;
using Drastic.YouTube.Utils;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Bridge;

internal class HeatmarkerExtractor
{
    private readonly JsonElement content;

    public HeatmarkerExtractor(JsonElement content) => this.content = content;

    public long? TryGetTimeRangeStartMillis() => Memo.Cache(this, () =>
        this.content.GetPropertyOrNull("heatMarkerRenderer")?.GetPropertyOrNull("timeRangeStartMillis")?.GetInt64OrNull());

    public long? TryGetMarkerRangeStartMillis() => Memo.Cache(this, () =>
        this.content.GetPropertyOrNull("heatMarkerRenderer")?.GetPropertyOrNull("markerDurationMillis")?.GetInt64OrNull());

    public decimal? TryGetHeatMarkerIntensityScoreNormalized() => Memo.Cache(this, () =>
        this.content.GetPropertyOrNull("heatMarkerRenderer")?.GetPropertyOrNull("heatMarkerIntensityScoreNormalized")?.GetDecimal());
}
