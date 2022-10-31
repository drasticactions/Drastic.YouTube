// <copyright file="ConversionRequest.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.YouTube.Videos.Streams;

namespace Drastic.YouTube.Converter;

/// <summary>
/// Conversion options.
/// </summary>
public class ConversionRequest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConversionRequest"/> class.
    /// Initializes an instance of <see cref="ConversionRequest" />.
    /// </summary>
    public ConversionRequest(
        string ffmpegCliFilePath,
        string outputFilePath,
        Container container,
        ConversionPreset preset)
    {
        this.FFmpegCliFilePath = ffmpegCliFilePath;
        this.OutputFilePath = outputFilePath;
        this.Container = container;
        this.Preset = preset;
    }

    /// <summary>
    /// Gets path to FFmpeg CLI.
    /// </summary>
    public string FFmpegCliFilePath { get; }

    /// <summary>
    /// Gets output file path.
    /// </summary>
    public string OutputFilePath { get; }

    /// <summary>
    /// Gets output container.
    /// </summary>
    public Container Container { get; }

    /// <summary>
    /// Gets encoder preset.
    /// </summary>
    public ConversionPreset Preset { get; }
}