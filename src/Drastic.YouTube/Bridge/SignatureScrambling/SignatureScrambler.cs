// <copyright file="SignatureScrambler.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Drastic.YouTube.Bridge.SignatureScrambling
{
    internal partial class SignatureScrambler
    {
        public SignatureScrambler(IReadOnlyList<IScramblerOperation> operations) =>
            this.Operations = operations;

        private IReadOnlyList<IScramblerOperation> Operations { get; }

        public string Unscramble(string input) =>
            this.Operations.Aggregate(input, (acc, op) => op.Unscramble(acc));
    }

    internal partial class SignatureScrambler
    {
        public static SignatureScrambler Null { get; } = new(Array.Empty<IScramblerOperation>());
    }
}