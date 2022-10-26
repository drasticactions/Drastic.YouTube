// <copyright file="SliceScramblerOperation.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace Drastic.YouTube.Bridge.SignatureScrambling
{
    internal class SliceScramblerOperation : IScramblerOperation
    {
        private readonly int index;

        public SliceScramblerOperation(int index) => this.index = index;

        public string Unscramble(string input) => input.Substring(this.index);

        [ExcludeFromCodeCoverage]
        public override string ToString() => $"Slice ({this.index})";
    }
}