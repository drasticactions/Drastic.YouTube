// <copyright file="Program.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Drastic.YouTube;
using Drastic.YouTube.Common;
using Drastic.YouTube.Sample.ConsoleApp;
using Drastic.YouTube.Videos;
using Drastic.YouTube.Videos.Streams;
using Sharprompt;

Console.Title = "Drastic.YouTube Demo";

var vd = new VideoDownloader();

var sd = new StoryboardDownloader();

var value = Prompt.Select<Menu>("Main Menu");

switch (value)
{
    case Menu.VideoDownload:
        await vd.StartAsync();
        break;
    case Menu.StoryboardDownloader:
        await sd.StartAsync();
        break;
}

public enum Menu
{
    /// <summary>
    /// Video Downloader.
    /// </summary>
    [Display(Name = "Video Downloader")]
    VideoDownload,

    /// <summary>
    /// Storyboard Downloader.
    /// </summary>
    [Display(Name = "Storyboard Downloader")]
    StoryboardDownloader,
}