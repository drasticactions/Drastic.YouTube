// <copyright file="VideoSpecs.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Drastic.YouTube.Common;
using Drastic.YouTube.Exceptions;
using Drastic.YouTube.Tests.TestData;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Drastic.YouTube.Tests;

public class VideoSpecs
{
    private readonly ITestOutputHelper testOutput;

    public VideoSpecs(ITestOutputHelper testOutput) =>
        this.testOutput = testOutput;

    [Fact]
    public async Task User_can_get_metadata_of_a_video()
    {
        // Arrange
        var youtube = new YoutubeClient();

        // Act
        var video = await youtube.Videos.GetAsync(VideoIds.ContainsDashManifest);

        // Assert
        video.Id.Value.Should().Be(VideoIds.ContainsDashManifest);
        video.Url.Should().NotBeNullOrWhiteSpace();
        video.Title.Should().Be("Aka no Ha [Another] +HDHR");
        video.Author.ChannelId.Value.Should().Be("UCEnBXANsKmyj2r9xVyKoDiQ");
        video.Author.ChannelUrl.Should().NotBeNullOrWhiteSpace();
        video.Author.ChannelTitle.Should().Be("Tyrrrz");
        video.UploadDate.Date.Should().Be(new DateTime(2017, 09, 30));
        video.Description.Should().Contain("246pp");
        video.Duration.Should().BeCloseTo(TimeSpan.FromSeconds(108), TimeSpan.FromSeconds(1));
        video.Thumbnails.Should().NotBeEmpty();
        video.Keywords.Should().BeEquivalentTo("osu", "mouse", "rhythm game");
        video.Engagement.ViewCount.Should().BeGreaterOrEqualTo(134);
        video.Engagement.LikeCount.Should().BeGreaterOrEqualTo(5);
        video.Engagement.DislikeCount.Should().BeGreaterOrEqualTo(0);
        video.Engagement.AverageRating.Should().BeGreaterOrEqualTo(0);
        video.Heatmap.Should().BeEmpty();
    }

    [Fact]
    public async Task User_can_access_heatmap()
    {
        // Arrange
        var youtube = new YoutubeClient();

        // Act
        var video = await youtube.Videos.GetAsync(VideoIds.ContainsHeatmap);

        // Assert
        video.Heatmap.Should().NotBeEmpty();
    }

    [Fact]
    public async Task User_cannot_get_metadata_of_a_private_video()
    {
        // Arrange
        var youtube = new YoutubeClient();

        // Act & assert
        var ex = await Assert.ThrowsAsync<VideoUnavailableException>(async () =>
            await youtube.Videos.GetAsync(VideoIds.Private));

        this.testOutput.WriteLine(ex.Message);
    }

    [Fact]
    public async Task User_cannot_get_metadata_of_a_non_existing_video()
    {
        // Arrange
        var youtube = new YoutubeClient();

        // Act & assert
        var ex = await Assert.ThrowsAsync<VideoUnavailableException>(async () =>
            await youtube.Videos.GetAsync(VideoIds.NonExisting));

        this.testOutput.WriteLine(ex.Message);
    }

    [Theory]
    [InlineData(VideoIds.Normal)]
    [InlineData(VideoIds.Unlisted)]
    [InlineData(VideoIds.EmbedRestrictedByAuthor)]
    [InlineData(VideoIds.EmbedRestrictedByYouTube)]
    [InlineData(VideoIds.AgeRestricted)]
    [InlineData(VideoIds.AgeRestrictedEmbedRestricted)]
    [InlineData(VideoIds.RatingDisabled)]
    public async Task User_can_get_metadata_of_any_available_video(string videoId)
    {
        // Arrange
        var youtube = new YoutubeClient();

        // Act
        var video = await youtube.Videos.GetAsync(videoId);

        // Assert
        video.Id.Value.Should().Be(videoId);
        video.Url.Should().NotBeNullOrWhiteSpace();
        video.Title.Should().NotBeNullOrWhiteSpace();
        video.Author.ChannelId.Value.Should().NotBeNullOrWhiteSpace();
        video.Author.ChannelUrl.Should().NotBeNullOrWhiteSpace();
        video.Author.ChannelTitle.Should().NotBeNullOrWhiteSpace();
        video.UploadDate.Date.Should().NotBe(default);
        video.Description.Should().NotBeNull();
        video.Duration.Should().NotBe(default);
        video.Thumbnails.Should().NotBeEmpty();
    }

    [Fact]
    public async Task User_can_get_the_highest_resolution_thumbnail_from_a_video()
    {
        // Arrange
        var youtube = new YoutubeClient();

        // Act
        var video = await youtube.Videos.GetAsync(VideoIds.Normal);
        var thumbnail = video.Thumbnails.GetWithHighestResolution();

        // Assert
        thumbnail.Url.Should().NotBeNullOrWhiteSpace();
    }
}