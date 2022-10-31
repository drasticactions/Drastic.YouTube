// <copyright file="FFmpegFixture.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;
using Xunit;

namespace Drastic.YouTube.Converter.Tests.Fixtures;

public partial class FFmpegFixture : IAsyncLifetime
{
    // Allow this fixture to be reused across multiple tests
    private static readonly SemaphoreSlim Lock = new(1, 1);

    public string FilePath { get; } = Path.Combine(
        Path.GetDirectoryName(typeof(FFmpegFixture).Assembly.Location) ?? Directory.GetCurrentDirectory(),
        GetFFmpegFileName());

    public async Task InitializeAsync()
    {
        await Lock.WaitAsync();

        try
        {
            if (File.Exists(this.FilePath))
            {
                return;
            }

            await this.DownloadFFmpegAsync();
        }
        finally
        {
            Lock.Release();
        }
    }

    private async ValueTask EnsureFFmpegHasExecutePermissionAsync()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }

        await Cli.Wrap("/bin/bash")
            .WithArguments(new[] { "-c", $"chmod +x {this.FilePath}" })
            .ExecuteAsync();
    }

    private async ValueTask DownloadFFmpegAsync()
    {
        using var httpClient = new HttpClient();

        await using var zipStream = await httpClient.GetStreamAsync(GetFFmpegDownloadUrl());
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);

        var entry = zip.GetEntry(GetFFmpegFileName());

        if (entry is null)
        {
            throw new FileNotFoundException("Downloaded archive doesn't contain FFmpeg.");
        }

        await using var entryStream = entry.Open();
        await using var fileStream = File.Create(this.FilePath);
        await entryStream.CopyToAsync(fileStream);

        await this.EnsureFFmpegHasExecutePermissionAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;
}

public partial class FFmpegFixture
{
    private static string GetFFmpegFileName() =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "ffmpeg.exe"
            : "ffmpeg";

    private static string GetFFmpegDownloadUrl()
    {
        static string GetPlatformMoniker()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "win";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "linux";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "osx";
            }

            throw new NotSupportedException("Unsupported OS platform.");
        }

        static string GetArchitectureMoniker()
        {
            if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
            {
                return "64";
            }

            if (RuntimeInformation.ProcessArchitecture == Architecture.X86)
            {
                return "86";
            }

            if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
            {
                return "arm-64";
            }

            if (RuntimeInformation.ProcessArchitecture == Architecture.Arm)
            {
                return "arm";
            }

            throw new NotSupportedException("Unsupported architecture.");
        }

        const string version = "4.4.1";
        var plat = GetPlatformMoniker();
        var arch = GetArchitectureMoniker();

        return $"https://github.com/vot/ffbinaries-prebuilt/releases/download/v{version}/ffmpeg-{version}-{plat}-{arch}.zip";
    }
}