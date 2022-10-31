// <copyright file="DownloadClip.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Text.Json;
using Drastic.YouTube.Converter;
using Drastic.YouTube.Videos;
using Sharprompt;

namespace Drastic.YouTube.Sample.ConsoleApp;

public class DownloadClip
{
    public YoutubeClient Youtube = new YoutubeClient();

    public async Task StartAsync()
    {
        // Get the video ID
        var id = Prompt.Input<string>("Enter YouTube video ID or URL");

        var videoId = VideoId.Parse(id);

        var video = await this.Youtube.Videos.GetAsync(videoId);

        var startTime = Prompt.Input<int>("Enter Clip Start Time", 0);

        var ending = video.Duration?.TotalSeconds ?? 0;

        var endTime = Prompt.Input<int>("Enter Clip End Time", (int)ending, ending.ToString());

        var clip = new ClipDuration(startTime, endTime);

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

        fileName = Prompt.Input<string>("Enter Filename", fileName, fileName);

        await this.Youtube.Videos.DownloadClipAsync(fileName, streamInfo, clip);

        Console.WriteLine($"Wrote {fileName}");
    }
}
