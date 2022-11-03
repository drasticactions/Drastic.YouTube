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

Console.Clear();

Console.Title = "Drastic.YouTube Demo";

var vd = new VideoDownloader();

var sd = new StoryboardDownloader();

var search = new SearchVideosCommand();

var rt = new RichThumbnailDownloader();

var meta = new MetadataDownloader();

var clip = new DownloadClip();

var ccclip = new ClosedCaptionedClipDownloader();

var value = Prompt.Select<Menu>("Main Menu");

switch (value)
{
    case Menu.VideoDownload:
        await vd.StartAsync();
        break;
    case Menu.StoryboardDownloader:
        await sd.StartAsync();
        break;
    case Menu.SearchVideos:
        await search.StartAsync();
        break;
    case Menu.DownloadRichThumbnail:
        await rt.StartAsync();
        break;
    case Menu.DownloadMetadata:
        await meta.StartAsync();
        break;
    case Menu.DownloadClip:
        await clip.StartAsync();
        break;
    case Menu.ClosedCaptionedClipDownload:
        await ccclip.StartAsync();
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

    /// <summary>
    /// Search Videos.
    /// </summary>
    [Display(Name = "Search Videos")]
    SearchVideos,

    /// <summary>
    /// Download Rich Thumbnail.
    /// </summary>
    [Display(Name = "Download Rich Thumbnail")]
    DownloadRichThumbnail,

    /// <summary>
    /// Download Metadata.
    /// </summary>
    [Display(Name = "Download Metadata")]
    DownloadMetadata,

    /// <summary>
    /// Download Clip.
    /// </summary>
    [Display(Name = "Download Clip")]
    DownloadClip,

    /// <summary>
    /// Download closed captioned Clip.
    /// </summary>
    [Display(Name = "Download Closed Caption Clip")]
    ClosedCaptionedClipDownload,
}