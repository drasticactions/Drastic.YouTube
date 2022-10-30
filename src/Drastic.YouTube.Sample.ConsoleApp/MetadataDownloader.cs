// <copyright file="MetadataDownloader.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Text.Json;
using Drastic.YouTube.Videos;
using Sharprompt;

namespace Drastic.YouTube.Sample.ConsoleApp;

public class MetadataDownloader
{
    public YoutubeClient youtube = new YoutubeClient();

    public async Task StartAsync()
    {
        // Get the video ID
        var id = Prompt.Input<string>("Enter YouTube video ID or URL");

        var videoId = VideoId.Parse(id);

        var video = await this.youtube.Videos.GetAsync(videoId);

        var jsonOptions = new JsonSerializerOptions() { WriteIndented = true };

        var jsonString = System.Text.Json.JsonSerializer.Serialize(video, jsonOptions);

        await System.IO.File.WriteAllTextAsync($"{videoId.ToString()}.json", jsonString);

        Console.WriteLine($"Wrote {videoId.ToString()}.json");
    }
}
