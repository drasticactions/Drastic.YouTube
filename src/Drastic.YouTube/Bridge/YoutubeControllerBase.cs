// <copyright file="YoutubeControllerBase.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Drastic.YouTube.Exceptions;

namespace Drastic.YouTube.Bridge;

internal abstract class YoutubeControllerBase
{
    internal readonly HttpClient Http;

    // This key doesn't appear to change
    protected const string ApiKey = "AIzaSyAO_FJ2SlqU8Q4STEHLGCilw_Y9_11qcW8";

    protected YoutubeControllerBase(HttpClient http) =>
        this.Http = http;

    protected async ValueTask<string> SendHttpRequestAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken = default)
    {
        // User-agent
        if (!request.Headers.Contains("User-Agent"))
        {
            request.Headers.Add(
                "User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.114 Safari/537.36");
        }

        // Set required cookies
        request.Headers.Add("Cookie", "CONSENT=YES+cb; YSC=DwKYllHNwuw");

        using var response = await this.Http.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        // Special case check for rate limiting errors
        if ((int)response.StatusCode == 429)
        {
            throw new RequestLimitExceededException(
                "Exceeded request rate limit. " +
                "Please try again in a few hours. " +
                "Alternatively, inject an instance of HttpClient that includes cookies for authenticated user.");
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Response status code does not indicate success: {(int)response.StatusCode} ({response.StatusCode})." +
                Environment.NewLine +
                "Request:" +
                Environment.NewLine +
                request);
        }

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    protected async ValueTask<string> SendHttpRequestAsync(
        string url,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        return await this.SendHttpRequestAsync(request, cancellationToken);
    }
}