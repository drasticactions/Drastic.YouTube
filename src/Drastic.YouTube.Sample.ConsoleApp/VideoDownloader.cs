// <copyright file="VideoDownloader.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using Drastic.YouTube.Videos;
using Drastic.YouTube.Videos.Streams;
using Sharprompt;

namespace Drastic.YouTube.Sample.ConsoleApp;

public class VideoDownloader
{
    public YoutubeClient Youtube = new YoutubeClient();

    public async Task StartAsync()
    {
        // Get the video ID
        var id = Prompt.Input<string>("Enter YouTube video ID or URL");

        var videoId = VideoId.Parse(id);

        var vid = await this.Youtube.Videos.GetAsync(videoId);

        // Get available streams and choose the best muxed (audio + video) stream
        var streamManifest = await this.Youtube.Videos.Streams.GetManifestAsync(videoId);

        var streams = streamManifest.GetMuxedStreams().ToArray();

        if (!streams.Any())
        {
            // No streams, bail.
            return;
        }

        var vidTypes = streams.Select(n => n.ToString()).ToArray() ?? new string[0];

        var quality = Prompt.Select("Select Muxed Video Quality", vidTypes);

        var streamInfo = streams[Array.IndexOf(vidTypes, quality)];

        if (streamInfo is null)
        {
            // Available streams vary depending on the video and it's possible
            // there may not be any muxed streams at all.
            // See the readme to learn how to handle adaptive streams.
            Console.Error.WriteLine("This video has no muxed streams.");
            return;
        }

        // Download the stream
        var fileName = $"{videoId}.{streamInfo.Container.Name}";

        Console.Write(
            $"Downloading stream: {streamInfo.VideoQuality.Label} / {streamInfo.Container.Name}... ");

        using (var progress = new ConsoleProgress())
        {
            await this.Youtube.Videos.Streams.DownloadAsync(streamInfo, fileName, progress);
        }

        Console.WriteLine("Done");

        Console.WriteLine($"Video saved to '{fileName}'");
    }
}