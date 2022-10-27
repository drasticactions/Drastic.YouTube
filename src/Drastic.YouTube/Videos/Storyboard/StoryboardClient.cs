// <copyright file="StoryboardClient.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Drastic.YouTube.Common;
using Drastic.YouTube.Exceptions;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Videos.Storyboard;

public class StoryboardClient
{
    private readonly StoryboardController controller;

    /// <summary>
    /// Initializes a new instance of the <see cref="StoryboardClient"/> class.
    /// Initializes an instance of <see cref="StoryboardClient" />.
    /// </summary>
    public StoryboardClient(HttpClient http) =>
        this.controller = new StoryboardController(http);

    public ValueTask<IReadOnlyList<StoryboardImage>> GetStoryboardImagesAsync(
        Drastic.YouTube.Common.Storyboard storyboard,
        CancellationToken cancellationToken = default)
        => this.controller.GetStoryboardImagesAsync(storyboard, cancellationToken);
}
