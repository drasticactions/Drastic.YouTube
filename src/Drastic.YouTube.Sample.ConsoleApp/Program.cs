// <copyright file="Program.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.YouTube;
using Drastic.YouTube.Common;
using Drastic.YouTube.Sample.ConsoleApp;
using Drastic.YouTube.Videos;
using Drastic.YouTube.Videos.Streams;

Console.Title = "Drastic.YouTube Demo";

var youtube = new YoutubeClient();

// Get the video ID
Console.Write("Enter YouTube video ID or URL: ");

var videoId = VideoId.Parse(Console.ReadLine() ?? string.Empty);

var vid = await youtube.Videos.GetAsync(videoId);

var storyboards = vid.GetVideoStoryboards();

var test = storyboards.LastOrDefault();

// Get available streams and choose the best muxed (audio + video) stream
var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoId);

var streamInfo = streamManifest.GetMuxedStreams().TryGetWithHighestVideoQuality();

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
    await youtube.Videos.Streams.DownloadAsync(streamInfo, fileName, progress);
}

Console.WriteLine("Done");

Console.WriteLine($"Video saved to '{fileName}'");
