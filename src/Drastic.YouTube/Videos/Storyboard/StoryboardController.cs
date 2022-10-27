// <copyright file="StoryboardController.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Attributes;
using Drastic.YouTube.Bridge;
using Drastic.YouTube.Common;
using Drastic.YouTube.Utils;
using Drastic.YouTube.Utils.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;

namespace Drastic.YouTube.Videos.Storyboard;

internal class StoryboardController : VideoController
{
    public StoryboardController(HttpClient http)
        : base(http)
    {
    }

    public async ValueTask<IReadOnlyList<StoryboardImage>> GetStoryboardImagesAsync(
        Drastic.YouTube.Common.Storyboard storyboard,
        CancellationToken cancellationToken = default)
    {
        var list = new List<StoryboardImage>();
        var imageBytes = await this.Http.GetByteArrayAsync(storyboard.Url.ToString(), cancellationToken);
        if (imageBytes.Length <= 0)
        {
            return list;
        }

        using var image = Image.Load<Rgba32>(imageBytes);

        var col = 1;
        var row = 1;
        var start = storyboard.Start;
        var totalTiles = (image.Width / storyboard.Width) * (image.Height / storyboard.Height);
        var duration = storyboard.Duration / totalTiles;
        for (var i = 1; i <= totalTiles; i++)
        {
            var x = (storyboard.Width * col) - storyboard.Width;
            var y = (storyboard.Height * row) - storyboard.Height;
            var thumbnail = Extract(image, new Rectangle(x, y, storyboard.Width, storyboard.Height));
            using var ms = new MemoryStream();
            await thumbnail.SaveAsJpegAsync(ms);
            list.Add(new StoryboardImage(storyboard.Id, ms.ToArray(), thumbnail.Width, thumbnail.Height, col, row, start, duration));
            col += 1;
            start = start + duration;
            if (col > storyboard.Columns)
            {
                col = 1;
                row += 1;
            }
        }

        return list;
    }

    private static Image<Rgba32> Extract(Image<Rgba32> sourceImage, Rectangle sourceArea)
    {
        Image<Rgba32> targetImage = new(sourceArea.Width, sourceArea.Height);
        int height = sourceArea.Height;
        sourceImage.ProcessPixelRows(targetImage, (sourceAccessor, targetAccessor) =>
        {
            for (int i = 0; i < height; i++)
            {
                Span<Rgba32> sourceRow = sourceAccessor.GetRowSpan(sourceArea.Y + i);
                Span<Rgba32> targetRow = targetAccessor.GetRowSpan(i);

                sourceRow.Slice(sourceArea.X, sourceArea.Width).CopyTo(targetRow);
            }
        });

        return targetImage;
    }
}
