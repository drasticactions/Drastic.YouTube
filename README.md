# Drastic.YouTube

Drastic.YouTube is a fork of [Drastic.YouTube](https://github.com/Tyrrrz/Drastic.YouTube), designed for new features while keeping a similar-ish layout and API. There is no real plan for this, outside of adding new features for myself whenever I feel like it, but if there is interest in more active development of this then let me know.

[![Version](https://img.shields.io/nuget/v/Drastic.YouTube.svg)](https://nuget.org/packages/Drastic.YouTube)
[![Downloads](https://img.shields.io/nuget/dt/Drastic.YouTube.svg)](https://nuget.org/packages/Drastic.YouTube)

## License

Following the original library, Drastic.Youtube is licensed under LGPL3. [Tyrrrz](https://tyrrrz.me), the original developer, also included a clause regarding Russia's military aggression against Ukraine. I agree and stand by that and will be including it here:

>## Terms of use <sup>[[?]](https://github.com/Tyrrrz/.github/blob/master/docs/why-so-political.md)</sup>
>By using this project or its source code, for any purpose and in any shape or form, you grant your **implicit agreement** to all the following statements:
>- You **condemn Russia and its military aggression against Ukraine**
>- You **recognize that Russia is an occupant that unlawfully ?invaded a sovereign state**
>- You **support Ukraine's territorial integrity, including its claims over temporarily occupied territories of Crimea and Donbas**
>- You **reject false narratives perpetuated by Russian state propaganda**
>To learn more about the war and how you can help, [click here](https://tyrrrz.me). Glory to Ukraine! 🇺🇦


---

(**Drastic.YouTube**) is a library that provides an interface to query metadata of YouTube videos, playlists and channels, as well as to resolve and download video streams and closed caption tracks.
Behind a layer of abstraction, the library parses raw page content and uses reverse-engineered requests to retrieve information.
As it doesn't rely on the official API, there's also no need for an API key and there are no usage quotas.

## Install

- 📦 : `dotnet add package Drastic.YouTube`

## Usage

**Drastic.YouTube** exposes its functionality through a single entry point — the `YoutubeClient` class.
Create an instance of this class and use the provided operations on `Videos`, `Playlists`, `Channels`, and `Search` properties to send requests.

### Videos

#### Retrieving video metadata

To retrieve the metadata associated with a YouTube video, call `Videos.GetAsync(...)`:

```csharp
using Drastic.YouTube;

var youtube = new YoutubeClient();

// You can specify both video ID or URL
var video = await youtube.Videos.GetAsync("https://youtube.com/watch?v=u_yIGGhubZs");

var title = video.Title; // "Collections - Blender 2.80 Fundamentals"
var author = video.Author.ChannelTitle; // "Blender"
var duration = video.Duration; // 00:07:20
```

#### Downloading video streams

Every YouTube video has a number of streams available, differing in containers, video quality, bit rate, frame rate, and other properties.
Additionally, depending on the content of the stream, the streams are further divided into 3 categories:

- Muxed streams — contain both video and audio
- Audio-only streams — contain only audio
- Video-only streams — contain only video

You can request the manifest that lists all available streams for a particular video by calling `Videos.Streams.GetManifestAsync(...)`:

```csharp
using Drastic.YouTube;

var youtube = new YoutubeClient();

var streamManifest = await youtube.Videos.Streams.GetManifestAsync(
    "https://youtube.com/watch?v=u_yIGGhubZs"
);
```

Once you get the manifest, you can filter through the streams and select the ones you're interested in:

```csharp
using Drastic.YouTube;
using Drastic.YouTube.Videos.Streams;

// ...

// Get highest quality muxed stream
var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

// ...or highest bitrate audio-only stream
var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

// ...or highest quality MP4 video-only stream
var streamInfo = streamManifest
    .GetVideoOnlyStreams()
    .Where(s => s.Container == Container.Mp4)
    .GetWithHighestVideoQuality()
```

Finally, you can resolve the actual stream represented by the specified metadata using `Videos.Streams.GetAsync(...)` or download it directly to a file with `Videos.Streams.DownloadAsync(...)`:

```csharp
// ...

// Get the actual stream
var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

// Download the stream to a file
await youtube.Videos.Streams.DownloadAsync(streamInfo, $"video.{streamInfo.Container}");
```

> **Warning**:
> Muxed streams contain both audio and video, but these streams are very limited in quality (up to 720p30).
> To download video in the highest available quality, you need to resolve the best audio-only and video-only streams separately and then mux them together.

#### Downloading closed captions

Closed captions can be downloaded in a similar way to media streams.
To get the list of available closed caption tracks, call `Videos.ClosedCaptions.GetManifestAsync(...)`:

```csharp
using Drastic.YouTube;

var youtube = new YoutubeClient();

var trackManifest = await youtube.Videos.ClosedCaptions.GetManifestAsync(
    "https://youtube.com/watch?v=u_yIGGhubZs"
);
```

Then retrieve the metadata for a particular track:

```csharp
// ...

// Find closed caption track in English
var trackInfo = trackManifest.GetByLanguage("en");
```

Finally, use `Videos.ClosedCaptions.GetAsync(...)` to get the actual content of the track:

```csharp
// ...

var track = await youtube.Videos.ClosedCaptions.GetAsync(trackInfo);

// Get the caption displayed at 0:35
var caption = track.GetByTime(TimeSpan.FromSeconds(35));
var text = caption.Text; // "collection acts as the parent collection"
```

You can also download the closed caption track in SRT file format with `Videos.ClosedCaptions.DownloadAsync(...)`:

```csharp
// ...

await youtube.Videos.ClosedCaptions.DownloadAsync(trackInfo, "cc_track.srt");
```

### Playlists

#### Retrieving playlist metadata

You can get the metadata associated with a YouTube playlist by calling `Playlists.GetAsync(...)` method:

```csharp
using Drastic.YouTube;

var youtube = new YoutubeClient();

var playlist = await youtube.Playlists.GetAsync(
    "https://youtube.com/playlist?list=PLa1F2ddGya_-UvuAqHAksYnB0qL9yWDO6"
);

var title = playlist.Title; // "First Steps - Blender 2.80 Fundamentals"
var author = playlist.Author.ChannelTitle; // "Blender"
```

#### Getting videos included in a playlist

To get the videos included in a playlist, call `Playlists.GetVideosAsync(...)`:

```csharp
using Drastic.YouTube;
using Drastic.YouTube.Common;

var youtube = new YoutubeClient();

// Get all playlist videos
var videos = await youtube.Playlists.GetVideosAsync(
    "https://youtube.com/playlist?list=PLa1F2ddGya_-UvuAqHAksYnB0qL9yWDO6"
);

// Get only the first 20 playlist videos
var videosSubset = await youtube.Playlists
    .GetVideosAsync(playlist.Id)
    .CollectAsync(20);
```

You can also enumerate the videos iteratively without waiting for the whole list to load:

```csharp
using Drastic.YouTube;

var youtube = new YoutubeClient();

await foreach (var video in youtube.Playlists.GetVideosAsync(
    "https://youtube.com/playlist?list=PLa1F2ddGya_-UvuAqHAksYnB0qL9yWDO6"
))
{
    var title = video.Title;
    var author = video.Author;
}
```

If you need precise control over how many requests you send to YouTube, use `Playlists.GetVideoBatchesAsync(...)` which returns videos wrapped in batches:

```csharp
using Drastic.YouTube;

var youtube = new YoutubeClient();

// Each batch corresponds to one request
await foreach (var batch in youtube.Playlists.GetVideoBatchesAsync(
    "https://youtube.com/playlist?list=PLa1F2ddGya_-UvuAqHAksYnB0qL9yWDO6"
))
{
    foreach (var video in batch.Items)
    {
        var title = video.Title;
        var author = video.Author;
    }
}
```

### Channels

#### Retrieving channel metadata

You can get the metadata associated with a YouTube channel by calling `Channels.GetAsync(...)` method:

```csharp
using Drastic.YouTube;

var youtube = new YoutubeClient();

var channel = await youtube.Channels.GetAsync(
    "https://youtube.com/channel/UCSMOQeBJ2RAnuFungnQOxLg"
);

var title = channel.Title; // "Blender"
```

You can also get the channel metadata by username with `Channels.GetByUserAsync(...)`:

```csharp
using Drastic.YouTube;

var youtube = new YoutubeClient();

var channel = await youtube.Channels.GetByUserAsync("https://youtube.com/user/BlenderFoundation");

var id = channel.Id; // "UCSMOQeBJ2RAnuFungnQOxLg"
```

To get the channel metadata by slug or custom URL, use `Channels.GetBySlugAsync(...)`:

```csharp
using Drastic.YouTube;

var youtube = new YoutubeClient();

var channel = await youtube.Channels.GetBySlugAsync("https://youtube.com/c/BlenderFoundation");

var id = channel.Id; // "UCSMOQeBJ2RAnuFungnQOxLg"
```

#### Getting channel uploads

To get a list of videos uploaded by a channel, call `Channels.GetUploadsAsync(...)`:

```csharp
using Drastic.YouTube;
using Drastic.YouTube.Common;

var youtube = new YoutubeClient();

var videos = await youtube.Channels.GetUploadsAsync(
    "https://youtube.com/channel/UCSMOQeBJ2RAnuFungnQOxLg"
);
```

### Searching

You can execute a search query and get its results by calling `Search.GetResultsAsync(...)`.
Each result may represent either a video, a playlist, or a channel, so you need to apply pattern matching to handle the corresponding cases:

```csharp
using Drastic.YouTube;

var youtube = new YoutubeClient();

await foreach (var result in youtube.Search.GetResultsAsync("blender tutorials"))
{
    // Use pattern matching to handle different results (videos, playlists, channels)
    switch (result)
    {
        case VideoSearchResult video:
        {
            var id = video.Id;
            var title = video.Title;
            var duration = video.Duration;
            break;
        }
        case PlaylistSearchResult playlist:
        {
            var id = playlist.Id;
            var title = playlist.Title;
            break;
        }
        case ChannelSearchResult channel:
        {
            var id = channel.Id;
            var title = channel.Title;
            break;
        }
    }
}
```

To limit results to a specific type, use `Search.GetVideosAsync(...)`, `Search.GetPlaylistsAsync(...)`, or `Search.GetChannelsAsync(...)`:

```csharp
using Drastic.YouTube;
using Drastic.YouTube.Common;

var youtube = new YoutubeClient();

var videos = await youtube.Search.GetVideosAsync("blender tutorials");
var playlists = await youtube.Search.GetPlaylistsAsync("blender tutorials");
var channels = await youtube.Search.GetChannelsAsync("blender tutorials");
```

Similarly to playlists, you can also enumerate results in batches by calling `Search.GetResultBatchesAsync(...)`:

```csharp
using Drastic.YouTube;

var youtube = new YoutubeClient();

// Each batch corresponds to one request
await foreach (var batch in youtube.Search.GetResultBatchesAsync("blender tutorials"))
{
    foreach (var result in batch.Items)
    {
        switch (result)
        {
            case VideoSearchResult videoResult:
            {
                // ...
            }
            case PlaylistSearchResult playlistResult:
            {
                // ...
            }
            case ChannelSearchResult channelResult:
            {
                // ...
            }
        }
    }
}
```