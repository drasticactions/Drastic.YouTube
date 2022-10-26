// <copyright file="SwapScramblerOperation.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Bridge.SignatureScrambling
{
    internal class SwapScramblerOperation : IScramblerOperation
    {
        private readonly int index;

        public SwapScramblerOperation(int index) => this.index = index;

        public string Unscramble(string input) => input.SwapChars(0, this.index);

        [ExcludeFromCodeCoverage]
        public override string ToString() => $"Swap ({this.index})";
    }
}