// <copyright file="TempOutputFixture.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.IO;

namespace Drastic.YouTube.Converter.Tests.Fixtures;

public class TempOutputFixture : IDisposable
{
    public TempOutputFixture() => Directory.CreateDirectory(this.DirPath);

    public string DirPath { get; } = Path.Combine(
        Path.GetDirectoryName(typeof(TempOutputFixture).Assembly.Location) ?? Directory.GetCurrentDirectory(),
        "Temp",
        Guid.NewGuid().ToString());

    public string GetTempFilePath(string fileName) => Path.Combine(this.DirPath, fileName);

    public string GetTempFilePath() => this.GetTempFilePath(Guid.NewGuid().ToString());

    public void Dispose()
    {
        try
        {
            Directory.Delete(this.DirPath, true);
        }
        catch (DirectoryNotFoundException)
        {
        }
    }
}