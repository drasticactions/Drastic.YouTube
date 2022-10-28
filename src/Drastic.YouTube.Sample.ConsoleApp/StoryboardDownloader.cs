// <copyright file="StoryboardDownloader.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.IO;
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

        var storyboardList = vid.Storyboards.Select(n => n.ToString()).ToArray();

        var quality = Prompt.Select("Select Storyboard", storyboardList);

        var storyboardSet = vid.Storyboards[Array.IndexOf(storyboardList, quality)];

        Directory.CreateDirectory(videoId);

        foreach (var storyboard in storyboardSet.Storyboards)
        {
            Console.WriteLine($"Downloading {storyboard.Url}");
            var images = await this.youtube.Videos.Storyboard.GetStoryboardImagesAsync(storyboard);
            foreach (var image in images)
            {
                var file = Path.Combine(videoId.ToString(), $"{image.ToString()}.jpg");
                Console.WriteLine($"Writing {file}");
                await File.WriteAllBytesAsync(file, image.Image);
            }
        }

        Console.WriteLine("Done!");
    }
}