// <copyright file="IScramblerOperation.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace Drastic.YouTube.Bridge.SignatureScrambling
{
    internal interface IScramblerOperation
    {
        string Unscramble(string input);
    }
}