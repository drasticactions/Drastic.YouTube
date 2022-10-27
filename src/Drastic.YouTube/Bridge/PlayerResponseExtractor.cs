// <copyright file="PlayerResponseExtractor.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Drastic.YouTube.Utils;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Bridge;

internal partial class PlayerResponseExtractor
{
    private readonly JsonElement content;

    public PlayerResponseExtractor(JsonElement content) => this.content = content;

    public string? TryGetVideoPlayabilityError() => Memo.Cache(this, () =>
        this.TryGetVideoPlayability()?
            .GetPropertyOrNull("reason")?
            .GetStringOrNull());

    public bool IsVideoAvailable() => Memo.Cache(this, () =>
        !string.Equals(this.TryGetVideoPlayabilityStatus(), "error", StringComparison.OrdinalIgnoreCase) &&
        this.TryGetVideoDetails() is not null);

    public bool IsVideoPlayable() => Memo.Cache(this, () =>
        string.Equals(this.TryGetVideoPlayabilityStatus(), "ok", StringComparison.OrdinalIgnoreCase));

    public string? TryGetVideoTitle() => Memo.Cache(this, () =>
        this.TryGetVideoDetails()?
            .GetPropertyOrNull("title")?
            .GetStringOrNull());

    public string? TryGetVideoChannelId() => Memo.Cache(this, () =>
        this.TryGetVideoDetails()?
            .GetPropertyOrNull("channelId")?
            .GetStringOrNull());

    public string? TryGetVideoAuthor() => Memo.Cache(this, () =>
        this.TryGetVideoDetails()?
            .GetPropertyOrNull("author")?
            .GetStringOrNull());

    public DateTimeOffset? TryGetVideoUploadDate() => Memo.Cache(this, () =>
        this.content
            .GetPropertyOrNull("microformat")?
            .GetPropertyOrNull("playerMicroformatRenderer")?
            .GetPropertyOrNull("uploadDate")?
            .GetDateTimeOffset());

    public TimeSpan? TryGetVideoDuration() => Memo.Cache(this, () =>
        this.TryGetVideoDetails()?
            .GetPropertyOrNull("lengthSeconds")?
            .GetStringOrNull()?
            .ParseDoubleOrNull()?
            .Pipe(TimeSpan.FromSeconds));

    public IReadOnlyList<ThumbnailExtractor> GetVideoThumbnails() => Memo.Cache(this, () =>
        this.TryGetVideoDetails()?
            .GetPropertyOrNull("thumbnail")?
            .GetPropertyOrNull("thumbnails")?
            .EnumerateArrayOrNull()?
            .Select(j => new ThumbnailExtractor(j))
            .ToArray() ??

        Array.Empty<ThumbnailExtractor>());

    public IReadOnlyList<string> GetVideoKeywords() => Memo.Cache(this, () =>
        this.TryGetVideoDetails()?
            .GetPropertyOrNull("keywords")?
            .EnumerateArrayOrNull()?
            .Select(j => j.GetStringOrNull())
            .WhereNotNull()
            .ToArray() ??

        Array.Empty<string>());

    public string? TryGetVideoStoryboard() => Memo.Cache(this, () =>
        this.content.
            GetPropertyOrNull("storyboards")?
            .GetPropertyOrNull("playerStoryboardSpecRenderer")?
            .GetPropertyOrNull("spec")?
            .GetStringOrNull());

    public string? TryGetVideoDescription() => Memo.Cache(this, () =>
        this.TryGetVideoDetails()?
            .GetPropertyOrNull("shortDescription")?
            .GetStringOrNull());

    public long? TryGetVideoViewCount() => Memo.Cache(this, () =>
        this.TryGetVideoDetails()?
            .GetPropertyOrNull("viewCount")?
            .GetStringOrNull()?
            .ParseLongOrNull());

    public string? TryGetPreviewVideoId() => Memo.Cache(this, () =>
        this.TryGetVideoPlayability()?
            .GetPropertyOrNull("errorScreen")?
            .GetPropertyOrNull("playerLegacyDesktopYpcTrailerRenderer")?
            .GetPropertyOrNull("trailerVideoId")?
            .GetStringOrNull() ??

        this.TryGetVideoPlayability()?
            .GetPropertyOrNull("errorScreen")?
            .GetPropertyOrNull("ypcTrailerRenderer")?
            .GetPropertyOrNull("playerVars")?
            .GetStringOrNull()?
            .Pipe(Url.SplitQuery)
            .GetValueOrDefault("video_id") ??

        this.TryGetVideoPlayability()?
            .GetPropertyOrNull("errorScreen")?
            .GetPropertyOrNull("ypcTrailerRenderer")?
            .GetPropertyOrNull("playerResponse")?
            .GetStringOrNull()?

            // YouTube uses weird base64-like encoding here that I don't know how to deal with.
            // It's supposed to have JSON inside, but if extracted as is, it contains garbage.
            // Luckily, some of the text gets decoded correctly, which is enough for us to
            // extract the preview video ID using regex.
            .Replace('-', '+')
            .Replace('_', '/')
            .Pipe(Convert.FromBase64String)
            .Pipe(Encoding.UTF8.GetString)
            .Pipe(s => Regex.Match(s, @"video_id=(.{11})").Groups[1].Value)
            .NullIfWhiteSpace());

    public string? TryGetDashManifestUrl() => Memo.Cache(this, () =>
        this.TryGetStreamingData()?
            .GetPropertyOrNull("dashManifestUrl")?
            .GetStringOrNull());

    public string? TryGetHlsManifestUrl() => Memo.Cache(this, () =>
        this.TryGetStreamingData()?
            .GetPropertyOrNull("hlsManifestUrl")?
            .GetStringOrNull());

    public IReadOnlyList<IStreamInfoExtractor> GetStreams() => Memo.Cache(this, () =>
    {
        var result = new List<IStreamInfoExtractor>();

        var muxedStreams = this.TryGetStreamingData()?
            .GetPropertyOrNull("formats")?
            .EnumerateArrayOrNull()?
            .Select(j => new PlayerStreamInfoExtractor(j));

        if (muxedStreams is not null)
        {
            result.AddRange(muxedStreams);
        }

        var adaptiveStreams = this.TryGetStreamingData()?
            .GetPropertyOrNull("adaptiveFormats")?
            .EnumerateArrayOrNull()?
            .Select(j => new PlayerStreamInfoExtractor(j));

        if (adaptiveStreams is not null)
        {
            result.AddRange(adaptiveStreams);
        }

        return result;
    });

    public IReadOnlyList<PlayerClosedCaptionTrackInfoExtractor> GetClosedCaptionTracks() => Memo.Cache(this, () =>
        this.content
            .GetPropertyOrNull("captions")?
            .GetPropertyOrNull("playerCaptionsTracklistRenderer")?
            .GetPropertyOrNull("captionTracks")?
            .EnumerateArrayOrNull()?
            .Select(j => new PlayerClosedCaptionTrackInfoExtractor(j))
            .ToArray() ??

        Array.Empty<PlayerClosedCaptionTrackInfoExtractor>());

    /// <inheritdoc/>
    public override string ToString()
    {
        return this.content.ToString();
    }

    private JsonElement? TryGetStreamingData() => Memo.Cache(this, () =>
    this.content.GetPropertyOrNull("streamingData"));

    private JsonElement? TryGetVideoPlayability() => Memo.Cache(this, () =>
    this.content.GetPropertyOrNull("playabilityStatus"));

    private string? TryGetVideoPlayabilityStatus() => Memo.Cache(this, () =>
        this.TryGetVideoPlayability()?
            .GetPropertyOrNull("status")?
            .GetStringOrNull());

    private JsonElement? TryGetVideoDetails() => Memo.Cache(this, () =>
    this.content.GetPropertyOrNull("videoDetails"));
}

internal partial class PlayerResponseExtractor
{
    public static PlayerResponseExtractor Create(string raw) => new(Json.Parse(raw));
}