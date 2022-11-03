// <copyright file="Converter.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using CliWrap.Builders;
using Drastic.YouTube.Converter.Utils;
using Drastic.YouTube.Converter.Utils.Extensions;
using Drastic.YouTube.Videos;
using Drastic.YouTube.Videos.ClosedCaptions;
using Drastic.YouTube.Videos.Streams;

namespace Drastic.YouTube.Converter;

internal partial class Converter
{
    private readonly VideoClient videoClient;
    private readonly FFmpeg ffmpeg;
    private readonly ConversionPreset preset;

    public Converter(VideoClient videoClient, FFmpeg ffmpeg, ConversionPreset preset)
    {
        this.videoClient = videoClient;
        this.ffmpeg = ffmpeg;
        this.preset = preset;
    }

    public async ValueTask ProcessAsync(
        string filePath,
        Container container,
        IReadOnlyList<IStreamInfo> streamInfos,
        IReadOnlyList<ClosedCaptionTrackInfo> closedCaptionTrackInfos,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (!streamInfos.Any())
        {
            throw new InvalidOperationException("No streams provided.");
        }

        if (streamInfos.Count > 2)
        {
            throw new InvalidOperationException("Too many streams provided.");
        }

        var progressMuxer = progress?.Pipe(p => new ProgressMuxer(p));
        var streamDownloadProgress = progressMuxer?.CreateInput();
        var subtitleDownloadProgress = progressMuxer?.CreateInput(0.01);
        var conversionProgress = progressMuxer?.CreateInput(
            streamInfos.All(s => s.Container == container)
                ? 0.05 // transcoding is not required
                : 10); // transcoding is required

        var streamInputs = new List<StreamInput>(streamInfos.Count);
        var subtitleInputs = new List<SubtitleInput>(closedCaptionTrackInfos.Count);

        try
        {
            await this.PopulateStreamInputsAsync(
                filePath,
                streamInfos,
                streamInputs,
                streamDownloadProgress,
                cancellationToken);

            await this.PopulateSubtitleInputsAsync(
                filePath,
                closedCaptionTrackInfos,
                subtitleInputs,
                subtitleDownloadProgress,
                cancellationToken);

            await this.ProcessAsync(
                filePath,
                container,
                streamInputs,
                subtitleInputs,
                conversionProgress,
                cancellationToken);
        }
        finally
        {
            foreach (var inputStream in streamInputs)
            {
                inputStream.Dispose();
            }

            foreach (var inputClosedCaptionTrack in subtitleInputs)
            {
                inputClosedCaptionTrack.Dispose();
            }
        }
    }

    /// <summary>
    /// Process a clip of a video.
    /// This only uses the MP4 Muxed containers.
    /// </summary>
    /// <returns></returns>
    public async ValueTask ProcessClipAsync(
        string filePath,
        MuxedStreamInfo stream,
        ClipDuration clipDuration,
        string subtitlePath = "",
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var arguments = new ArgumentsBuilder();

        var result = this.AddClipDuration(clipDuration);

        if (!string.IsNullOrEmpty(subtitlePath))
        {
            arguments.Add("-vf").Add("subtitles=" + subtitlePath);
        }

        // Misc settings
        arguments
            .Add("-threads").Add(Environment.ProcessorCount)
            .Add("-nostdin")
            .Add("-y");

        // TODO: CLIWrap has a weird bug where it doesn't handle the URL parameter correctly
        // When passing it to the process handler. It may be a MacOS only issue though,
        // This is a cheap hack to get it working.
        var arg = $"{result} -i \"{stream.Url.ToString()}\" {arguments.Build()} {filePath}";

        await this.ffmpeg.ExecuteAsync(arg, progress, cancellationToken);
    }

    private async ValueTask ProcessAsync(
        string filePath,
        Container container,
        IReadOnlyList<StreamInput> streamInputs,
        IReadOnlyList<SubtitleInput> subtitleInputs,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var arguments = new ArgumentsBuilder();

        // Stream inputs
        foreach (var streamInput in streamInputs)
        {
            arguments.Add("-i").Add(streamInput.FilePath);
        }

        // Subtitle inputs
        foreach (var subtitleInput in subtitleInputs)
        {
            arguments.Add("-i").Add(subtitleInput.FilePath);
        }

        // Format
        arguments.Add("-f").Add(container.Name);

        // Preset
        arguments.Add("-preset").Add(this.preset);

        // Mapping
        for (var i = 0; i < streamInputs.Count + subtitleInputs.Count; i++)
        {
            arguments.Add("-map").Add(i);
        }

        // Avoid transcoding if possible
        if (streamInputs.All(s => s.Info.Container == container))
        {
            arguments
                .Add("-c:a").Add("copy")
                .Add("-c:v").Add("copy");
        }

        // MP4: specify subtitle codec manually, otherwise they're not injected
        if (container == Container.Mp4 && subtitleInputs.Any())
        {
            arguments.Add("-c:s").Add("mov_text");
        }

        // MP3: specify bitrate manually, otherwise the metadata will contain wrong duration
        // https://superuser.com/questions/892996/ffmpeg-is-doubling-audio-length-when-extracting-from-video
        if (container == Container.Mp3)
        {
            arguments.Add("-b:a").Add("165k");
        }

        // Inject language metadata for subtitles
        for (var i = 0; i < subtitleInputs.Count; i++)
        {
            arguments
                .Add($"-metadata:s:s:{i}")
                .Add($"language={subtitleInputs[i].Info.Language.Code}")
                .Add($"-metadata:s:s:{i}")
                .Add($"title={subtitleInputs[i].Info.Language.Name}");
        }

        // Misc settings
        arguments
            .Add("-threads").Add(Environment.ProcessorCount)
            .Add("-nostdin")
            .Add("-y");

        // Output
        arguments.Add(filePath);

        // Run FFmpeg
        await this.ffmpeg.ExecuteAsync(arguments.Build(), progress, cancellationToken);
    }

    private async ValueTask PopulateStreamInputsAsync(
        string baseFilePath,
        IReadOnlyList<IStreamInfo> streamInfos,
        ICollection<StreamInput> streamInputs,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var progressMuxer = progress?.Pipe(p => new ProgressMuxer(p));
        var progresses = streamInfos.Select(s => progressMuxer?.CreateInput(s.Size.MegaBytes)).ToArray();

        var lastIndex = 0;

        foreach (var (streamInfo, streamProgress) in streamInfos.Zip(progresses))
        {
            var streamInput = new StreamInput(
                streamInfo,
                $"{baseFilePath}.stream-{lastIndex++}.tmp");

            streamInputs.Add(streamInput);

            await this.videoClient.Streams.DownloadAsync(
                streamInfo,
                streamInput.FilePath,
                streamProgress,
                cancellationToken);
        }

        progress?.Report(1);
    }

    private async ValueTask PopulateSubtitleInputsAsync(
        string baseFilePath,
        IReadOnlyList<ClosedCaptionTrackInfo> closedCaptionTrackInfos,
        ICollection<SubtitleInput> subtitleInputs,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var progressMuxer = progress?.Pipe(p => new ProgressMuxer(p));
        var progresses = closedCaptionTrackInfos.Select(_ => progressMuxer?.CreateInput()).ToArray();

        var lastIndex = 0;

        foreach (var (trackInfo, trackProgress) in closedCaptionTrackInfos.Zip(progresses))
        {
            var subtitleInput = new SubtitleInput(
                trackInfo,
                $"{baseFilePath}.subtitles-{lastIndex++}.tmp");

            subtitleInputs.Add(subtitleInput);

            await this.videoClient.ClosedCaptions.DownloadAsync(
                trackInfo,
                subtitleInput.FilePath,
                trackProgress,
                cancellationToken);
        }

        progress?.Report(1);
    }

    private string AddClipDuration(ClipDuration? clipDuration)
    {
        var arguments = new ArgumentsBuilder();
        if (clipDuration is not null)
        {
            var startTime = clipDuration.StartTimeSeconds < 0 ? 0 : clipDuration.StartTimeSeconds;
            var endTime = clipDuration.EndTimeSeconds < 0 ? 0 : clipDuration.EndTimeSeconds;

            if (endTime < startTime)
            {
                endTime = startTime;
            }

            arguments
                .Add("-ss").Add((int)startTime);
            arguments
                .Add("-to").Add((int)endTime);

            return arguments.Build();
        }

        return string.Empty;
    }
}

internal partial class Converter
{
    private class StreamInput : IDisposable
    {
        public StreamInput(IStreamInfo info, string filePath)
        {
            this.Info = info;
            this.FilePath = filePath;
        }

        public IStreamInfo Info { get; }

        public string FilePath { get; }

        public void Dispose()
        {
            try
            {
                File.Delete(this.FilePath);
            }
            catch
            {
                // Ignore
            }
        }
    }

    private class SubtitleInput : IDisposable
    {
        public SubtitleInput(ClosedCaptionTrackInfo info, string filePath)
        {
            this.Info = info;
            this.FilePath = filePath;
        }

        public ClosedCaptionTrackInfo Info { get; }

        public string FilePath { get; }

        public void Dispose()
        {
            try
            {
                File.Delete(this.FilePath);
            }
            catch
            {
                // Ignore
            }
        }
    }
}