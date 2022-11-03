// <copyright file="ClosedCaptionedClipDownloader.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Text.Json;
using Drastic.YouTube.Converter;
using Drastic.YouTube.Videos;
using Sharprompt;
using SubtitleManager;

namespace Drastic.YouTube.Sample.ConsoleApp;

public class ClosedCaptionedClipDownloader
{
    public YoutubeClient Youtube = new YoutubeClient();

    public async Task StartAsync()
    {
        // Get the video ID
        var id = Prompt.Input<string>("Enter YouTube video ID or URL");

        var videoId = VideoId.Parse(id);

        var video = await this.Youtube.Videos.GetAsync(videoId);

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

        var languages = (await this.Youtube.Videos.ClosedCaptions.GetManifestAsync(videoId)).Tracks.ToArray();

        var langs = languages.Select(n => n.ToString()).ToArray() ?? new string[0];

        var lang = Prompt.Select("Select Language", langs);

        var selLang = languages[Array.IndexOf(langs, lang)];

        var captions = await this.Youtube.Videos.ClosedCaptions.GetAsync(selLang);

        var max = captions.Captions.Count() - 1;

        var start = Prompt.Input<int>($"Enter start number (Max: {max}) ");

        if (start < 0 || start > max)
        {
            return;
        }

        var end = Prompt.Input<int>($"Enter total number of clips (Max: {max}) ");

        var filteredCaptions = captions.Captions.Skip(start).Take(end);

        var srt = new SrtSubtitle();

        var startingTime = filteredCaptions.FirstOrDefault()?.Offset ?? default(TimeSpan);

        var endingTime = filteredCaptions.LastOrDefault()?.Offset + filteredCaptions.LastOrDefault()?.Duration ?? default(TimeSpan);

        foreach (var cap in filteredCaptions)
        {
            var s = cap.Offset - startingTime;
            var e = s + cap.Duration;

            srt.Lines.Add(new SrtSubtitleLine() { Start = s, End = e, Text = cap.Text });
        }

        var realName = Path.GetFileNameWithoutExtension(fileName);

        File.WriteAllText($"{realName}.srt", srt.ToString());

        await this.Youtube.Videos.DownloadClipAsync(fileName, streamInfo, new ClipDuration(startingTime.TotalSeconds, endingTime.TotalSeconds), $"{realName}.srt");

        Console.WriteLine($"Wrote {fileName}");
    }
}