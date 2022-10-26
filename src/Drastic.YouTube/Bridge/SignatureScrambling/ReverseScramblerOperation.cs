// <copyright file="ReverseScramblerOperation.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Bridge.SignatureScrambling
{
    internal class ReverseScramblerOperation : IScramblerOperation
    {
        public string Unscramble(string input) => input.Reverse();

        [ExcludeFromCodeCoverage]
        public override string ToString() => "Reverse";
    }
}