// <copyright file="ConversionRequestBuilder.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.YouTube.Converter.Utils.Extensions;
using Drastic.YouTube.Videos.Streams;

namespace Drastic.YouTube.Converter;

/// <summary>
/// Builder for <see cref="ConversionRequest" />.
/// </summary>
public partial class ConversionRequestBuilder
{
    private readonly string outputFilePath;

    private string? ffmpegCliFilePath;
    private Container? container;
    private ConversionPreset preset;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConversionRequestBuilder"/> class.
    /// Initializes an instance of <see cref="ConversionRequestBuilder" />.
    /// </summary>
    public ConversionRequestBuilder(string outputFilePath) =>
        this.outputFilePath = outputFilePath;

    /// <summary>
    /// Sets FFmpeg CLI path.
    /// </summary>
    /// <returns></returns>
    public ConversionRequestBuilder SetFFmpegPath(string path)
    {
        this.ffmpegCliFilePath = path;
        return this;
    }

    /// <summary>
    /// Sets output container.
    /// </summary>
    /// <returns></returns>
    public ConversionRequestBuilder SetContainer(Container container)
    {
        this.container = container;
        return this;
    }

    private Container GetDefaultContainer() => new(
        Path.GetExtension(this.outputFilePath).TrimStart('.').NullIfWhiteSpace() ??
        "mp4");

    /// <summary>
    /// Sets output container.
    /// </summary>
    /// <returns></returns>
    public ConversionRequestBuilder SetContainer(string container) =>
        this.SetContainer(new Container(container));

    /// <summary>
    /// Sets conversion preset.
    /// </summary>
    /// <returns></returns>
    public ConversionRequestBuilder SetPreset(ConversionPreset preset)
    {
        this.preset = preset;
        return this;
    }

    /// <summary>
    /// Builds the resulting request.
    /// </summary>
    /// <returns></returns>
    public ConversionRequest Build() => new(
        this.ffmpegCliFilePath ?? DefaultFFmpegCliPathLazy.Value,
        this.outputFilePath,
        this.container ?? this.GetDefaultContainer(),
        this.preset);
}

public partial class ConversionRequestBuilder
{
    private static readonly Lazy<string> DefaultFFmpegCliPathLazy = new(() =>

        // Try to find FFmpeg in the probe directory
        Directory
            .EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory ?? Directory.GetCurrentDirectory())
            .FirstOrDefault(f =>
                string.Equals(
                    Path.GetFileNameWithoutExtension(f),
                    "ffmpeg",
                    StringComparison.OrdinalIgnoreCase)) ??

        // Otherwise fallback to just "ffmpeg" and hope it's on the PATH
        "ffmpeg");
}