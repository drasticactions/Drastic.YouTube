// <copyright file="ChannelSpecs.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Linq;
using System.Threading.Tasks;
using Drastic.YouTube.Common;
using Drastic.YouTube.Tests.TestData;
using FluentAssertions;
using Xunit;

namespace Drastic.YouTube.Tests;

public class ChannelSpecs
{
    [Fact]
    public async Task User_can_get_metadata_of_a_channel()
    {
        // Arrange
        var youtube = new YoutubeClient();

        // Act
        var channel = await youtube.Channels.GetAsync(ChannelIds.Normal);

        // Assert
        channel.Id.Value.Should().Be(ChannelIds.Normal);
        channel.Url.Should().NotBeNullOrWhiteSpace();
        channel.Title.Should().Be("Tyrrrz");
        channel.Thumbnails.Should().NotBeEmpty();
    }

    [Fact]
    public async Task User_can_get_metadata_of_a_channel_by_user_name()
    {
        // Arrange
        var youtube = new YoutubeClient();

        // Act
        var channel = await youtube.Channels.GetByUserAsync(UserNames.Normal);

        // Assert
        channel.Id.Value.Should().Be("UCEnBXANsKmyj2r9xVyKoDiQ");
        channel.Url.Should().NotBeNullOrWhiteSpace();
        channel.Title.Should().Be("Tyrrrz");
        channel.Thumbnails.Should().NotBeEmpty();
    }

    [Fact]
    public async Task User_can_get_metadata_of_a_channel_by_slug()
    {
        // Arrange
        var youtube = new YoutubeClient();

        // Act
        var channel = await youtube.Channels.GetBySlugAsync(ChannelSlugs.Normal);

        // Assert
        channel.Id.Value.Should().Be("UCSli-_XJrdRwRoPw8DXRiyw");
        channel.Url.Should().NotBeNullOrWhiteSpace();
        channel.Title.Should().Be("Меланія Подоляк");
        channel.Thumbnails.Should().NotBeEmpty();
    }

    [Fact]
    public async Task User_can_get_videos_uploaded_by_a_channel()
    {
        // Arrange
        var youtube = new YoutubeClient();

        // Act
        var videos = await youtube.Channels.GetUploadsAsync(ChannelIds.Normal);

        // Assert
        videos.Should().HaveCountGreaterOrEqualTo(68);
        videos.Select(v => v.Author.ChannelId).Should().OnlyContain(i => i == ChannelIds.Normal);
    }
}