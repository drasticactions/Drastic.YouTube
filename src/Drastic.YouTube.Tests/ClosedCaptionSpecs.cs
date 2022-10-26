using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Drastic.YouTube.Tests.Fixtures;
using Drastic.YouTube.Tests.TestData;

namespace Drastic.YouTube.Tests;

public class ClosedCaptionSpecs : IClassFixture<TempOutputFixture>
{
    private readonly TempOutputFixture _tempOutputFixture;

    public ClosedCaptionSpecs(TempOutputFixture tempOutputFixture) =>
        _tempOutputFixture = tempOutputFixture;

    [Fact]
    public async Task User_can_get_the_list_of_available_closed_caption_tracks_on_a_video()
    {
        // Arrange
        var youtube = new YoutubeClient();

        // Act
        var manifest = await youtube.Videos.ClosedCaptions.GetManifestAsync(VideoIds.ContainsClosedCaptions);

        // Assert
        manifest.Tracks.Should().HaveCountGreaterOrEqualTo(3);

        manifest.Tracks.Should().Contain(t =>
            t.Language.Code == "en" &&
            t.Language.Name == "English (auto-generated)" &&
            t.IsAutoGenerated
        );

        manifest.Tracks.Should().Contain(t =>
            t.Language.Code == "en-US" &&
            t.Language.Name == "English (United States) - Captions" &&
            !t.IsAutoGenerated
        );

        manifest.Tracks.Should().Contain(t =>
            t.Language.Code == "es-419" &&
            t.Language.Name == "Spanish (Latin America)" &&
            !t.IsAutoGenerated
        );
    }

    [Fact]
    public async Task User_can_get_a_specific_closed_caption_track_from_a_video_with_broken_autogenerated_captions()
    {
        // Arrange
        var youtube = new YoutubeClient();

        // Act
        var manifest = await youtube.Videos.ClosedCaptions.GetManifestAsync(VideoIds.ContainsBrokenClosedCaptions);
        var trackInfo = manifest.GetByLanguage("en");

        var track = await youtube.Videos.ClosedCaptions.GetAsync(trackInfo);

        // Assert
        track.Captions.Should().HaveCountGreaterOrEqualTo(2000);
    }

    [Fact]
    public async Task User_can_get_a_specific_closed_caption_track_from_a_video()
    {
        // Arrange
        var youtube = new YoutubeClient();

        // Act
        var manifest = await youtube.Videos.ClosedCaptions.GetManifestAsync(VideoIds.ContainsClosedCaptions);
        var trackInfo = manifest.GetByLanguage("en-US");

        var track = await youtube.Videos.ClosedCaptions.GetAsync(trackInfo);

        // Assert
        track.Captions.Should().HaveCountGreaterOrEqualTo(500);
    }

    [Fact]
    public async Task User_can_get_an_individual_closed_caption_from_a_video()
    {
        // Arrange
        var youtube = new YoutubeClient();

        // Act
        var manifest = await youtube.Videos.ClosedCaptions.GetManifestAsync(VideoIds.ContainsClosedCaptions);
        var trackInfo = manifest.GetByLanguage("en-US");

        var track = await youtube.Videos.ClosedCaptions.GetAsync(trackInfo);

        var caption = track.GetByTime(TimeSpan.FromSeconds(641));

        // Assert
        caption.Text.Should().Be("as I said in that, and I've kind of");
    }

    [Fact]
    public async Task User_can_get_an_individual_closed_caption_part_from_a_video()
    {
        // Arrange
        var youtube = new YoutubeClient();

        // Act
        var manifest = await youtube.Videos.ClosedCaptions.GetManifestAsync(VideoIds.ContainsClosedCaptions);
        var trackInfo = manifest.GetByLanguage("en");

        var track = await youtube.Videos.ClosedCaptions.GetAsync(trackInfo);

        var captionPart = track
            .GetByTime(TimeSpan.FromSeconds(641))
            .GetPartByTime(TimeSpan.FromSeconds(0.15));

        // Assert
        captionPart.Text.Should().Be("know");
    }

    [Fact]
    public async Task User_can_download_a_specific_closed_caption_track_from_a_video()
    {
        // Arrange
        var filePath = _tempOutputFixture.GetTempFilePath();
        var youtube = new YoutubeClient();

        // Act
        var manifest = await youtube.Videos.ClosedCaptions.GetManifestAsync(VideoIds.ContainsClosedCaptions);
        var trackInfo = manifest.GetByLanguage("en-US");

        await youtube.Videos.ClosedCaptions.DownloadAsync(trackInfo, filePath);

        // Assert
        var fileInfo = new FileInfo(filePath);
        fileInfo.Exists.Should().BeTrue();
        fileInfo.Length.Should().BeGreaterThan(0);
    }
}