// <copyright file="SubtitleSpecs.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drastic.YouTube.Converter.Tests.Fixtures;
using Drastic.YouTube.Converter.Tests.Utils;
using Drastic.YouTube.Videos.Streams;
using FluentAssertions;
using Xunit;

namespace Drastic.YouTube.Converter.Tests;

public class SubtitleSpecs : IClassFixture<TempOutputFixture>, IClassFixture<FFmpegFixture>
{
    private readonly TempOutputFixture tempOutputFixture;
    private readonly FFmpegFixture ffmpegFixture;

    public SubtitleSpecs(
        TempOutputFixture tempOutputFixture,
        FFmpegFixture ffmpegFixture)
    {
        this.tempOutputFixture = tempOutputFixture;
        this.ffmpegFixture = ffmpegFixture;
    }

    [Fact]
    public async Task User_can_download_a_video_with_subtitles_into_a_single_mp4_file()
    {
        // Arrange
        var youtube = new YoutubeClient();
        var outputFilePath = this.tempOutputFixture.GetTempFilePath();

        var streamManifest = await youtube.Videos.Streams.GetManifestAsync("YltHGKX80Y8");
        var trackManifest = await youtube.Videos.ClosedCaptions.GetManifestAsync("YltHGKX80Y8");

        var streamInfos = new[]
        {
            streamManifest.GetVideoStreams().OrderBy(s => s.Size).First(s => s.Container == Container.Mp4),
        };

        var trackInfos = trackManifest.Tracks;

        // Act
        await youtube.Videos.DownloadAsync(
            streamInfos,
            trackInfos,
            new ConversionRequestBuilder(outputFilePath)
                .SetFFmpegPath(this.ffmpegFixture.FilePath)
                .SetContainer("mp4")
                .Build());

        // Assert
        MediaFormat.IsMp4File(outputFilePath).Should().BeTrue();

        foreach (var trackInfo in trackInfos)
        {
            FileEx.ContainsBytes(outputFilePath, Encoding.ASCII.GetBytes(trackInfo.Language.Name)).Should().BeTrue();
        }
    }

    [Fact]
    public async Task User_can_download_a_video_with_subtitles_into_a_single_webm_file()
    {
        // Arrange
        var youtube = new YoutubeClient();
        var outputFilePath = this.tempOutputFixture.GetTempFilePath();

        var streamManifest = await youtube.Videos.Streams.GetManifestAsync("YltHGKX80Y8");
        var trackManifest = await youtube.Videos.ClosedCaptions.GetManifestAsync("YltHGKX80Y8");

        var streamInfos = new[]
        {
            streamManifest.GetVideoStreams().OrderBy(s => s.Size).First(s => s.Container == Container.WebM),
        };

        var trackInfos = trackManifest.Tracks;

        // Act
        await youtube.Videos.DownloadAsync(
            streamInfos,
            trackInfos,
            new ConversionRequestBuilder(outputFilePath)
                .SetFFmpegPath(this.ffmpegFixture.FilePath)
                .SetContainer("webm")
                .Build());

        // Assert
        MediaFormat.IsWebMFile(outputFilePath).Should().BeTrue();

        foreach (var trackInfo in trackInfos)
        {
            FileEx.ContainsBytes(outputFilePath, Encoding.ASCII.GetBytes(trackInfo.Language.Name)).Should().BeTrue();
        }
    }
}