// <copyright file="ProgressMuxer.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Drastic.YouTube.Converter.Utils;

internal class ProgressMuxer
{
    private readonly IProgress<double> target;

    private readonly object @lock = new();
    private readonly Dictionary<int, double> splitWeights = new();
    private readonly Dictionary<int, double> splitValues = new();

    public ProgressMuxer(IProgress<double> target) =>
        this.target = target;

    public IProgress<double> CreateInput(double weight = 1)
    {
        lock (this.@lock)
        {
            var index = this.splitWeights.Count;

            this.splitWeights[index] = weight;
            this.splitValues[index] = 0;

            return new Progress<double>(p =>
            {
                lock (this.@lock)
                {
                    this.splitValues[index] = p;

                    var weightedSum = 0.0;
                    var weightedMax = 0.0;

                    for (var i = 0; i < this.splitWeights.Count; i++)
                    {
                        weightedSum += this.splitWeights[i] * this.splitValues[i];
                        weightedMax += this.splitWeights[i];
                    }

                    this.target.Report(weightedSum / weightedMax);
                }
            });
        }
    }
}