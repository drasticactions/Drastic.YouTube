// <copyright file="RichThumbnailDownloader.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using Drastic.YouTube.Common;
using Drastic.YouTube.Videos;
using Sharprompt;

namespace Drastic.YouTube.Sample.ConsoleApp;

public class RichThumbnailDownloader
{
    public YoutubeClient Youtube = new YoutubeClient();

    public async Task StartAsync()
    {
        // Get the video ID
        var id = Prompt.Input<string>("Enter YouTube video ID or URL");

        var videoId = VideoId.Parse(id);

        var thumbnail = await this.Youtube.GetRichThumbnailForVideoAsync(videoId);

        if (thumbnail is null)
        {
            Console.WriteLine("Could not find rich thumbnail");
            return;
        }

        Console.WriteLine(thumbnail.ToString());

        var filename = $"{videoId.ToString()}_rich.webp";

        var webp = await thumbnail.DownloadAsync();

        await File.WriteAllBytesAsync(filename, webp);

        Console.WriteLine($"Saved as {filename}");
    }
}