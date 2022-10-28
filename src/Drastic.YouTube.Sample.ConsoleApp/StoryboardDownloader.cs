// <copyright file="StoryboardDownloader.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using Drastic.YouTube.Videos;
using Drastic.YouTube.Videos.Streams;
using Sharprompt;

namespace Drastic.YouTube.Sample.ConsoleApp;

public class StoryboardDownloader
{
    public YoutubeClient youtube = new YoutubeClient();

    public async Task StartAsync()
    {
        // Get the video ID
        var id = Prompt.Input<string>("Enter YouTube video ID or URL");

        var videoId = VideoId.Parse(id);

        var vid = await youtube.Videos.GetAsync(videoId);
    }
}