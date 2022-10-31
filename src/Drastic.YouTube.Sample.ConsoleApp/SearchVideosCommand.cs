// <copyright file="SearchVideosCommand.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using Drastic.YouTube.Videos;
using Drastic.YouTube.Videos.Streams;
using Sharprompt;

namespace Drastic.YouTube.Sample.ConsoleApp;

public class SearchVideosCommand
{
    public YoutubeClient Youtube = new YoutubeClient();

    public async Task StartAsync()
    {
        // Get the search param.
        var searchParam = Prompt.Input<string>("Enter search term");

        var search = this.Youtube.Search.GetVideosAsync(searchParam);

        // Get first ten videos.
        var vids = await search.Take(10).ToListAsync();

        for (int i = 0; i < vids.Count; i++)
        {
            Search.VideoSearchResult vid = vids[i];
            Console.WriteLine($"{i + 1}. {vid.Title}");
            Console.WriteLine("Thumbnails:");

            foreach (var thumbnail in vid.Thumbnails)
            {
                Console.WriteLine(thumbnail);
            }
        }
    }
}