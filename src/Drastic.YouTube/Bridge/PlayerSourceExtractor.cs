// <copyright file="PlayerSourceExtractor.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Text.RegularExpressions;
using Drastic.YouTube.Bridge.SignatureScrambling;
using Drastic.YouTube.Utils;
using Drastic.YouTube.Utils.Extensions;

namespace Drastic.YouTube.Bridge;

internal partial class PlayerSourceExtractor
{
    private readonly string content;

    public PlayerSourceExtractor(string content) => this.content = content;

    public string? TryGetSignatureTimestamp() => Memo.Cache(this, () =>
        Regex.Match(this.content, @"(?:signatureTimestamp|sts)\s*:\s*(?<sts>[0-9]{5})")
            .Groups[1]
            .Value
            .NullIfWhiteSpace());

    public SignatureScrambler? TryGetSignatureScrambler() => Memo.Cache(this, () =>
    {
        var scramblerBody = this.TryGetScramblerBody();
        if (string.IsNullOrWhiteSpace(scramblerBody))
        {
            return null;
        }

        var scramblerDefinition = this.TryGetScramblerDefinition();
        if (string.IsNullOrWhiteSpace(scramblerDefinition))
        {
            return null;
        }

        var operations = new List<IScramblerOperation>();

        foreach (var statement in scramblerBody.Split(";"))
        {
            // Get the name of the function called in this statement
            var calledFuncName = Regex.Match(statement, @"\w+(?:.|\[)(\""?\w+(?:\"")?)\]?\(").Groups[1].Value;
            if (string.IsNullOrWhiteSpace(calledFuncName))
            {
                continue;
            }

            // Slice
            if (Regex.IsMatch(
                scramblerDefinition,
                $@"{Regex.Escape(calledFuncName)}:\bfunction\b\([a],b\).(\breturn\b)?.?\w+\."))
            {
                var index = Regex.Match(statement, @"\(\w+,(\d+)\)").Groups[1].Value.ParseInt();
                operations.Add(new SliceScramblerOperation(index));
            }

            // Swap
            else if (Regex.IsMatch(
                scramblerDefinition,
                $@"{Regex.Escape(calledFuncName)}:\bfunction\b\(\w+\,\w\).\bvar\b.\bc=a\b"))
            {
                var index = Regex.Match(statement, @"\(\w+,(\d+)\)").Groups[1].Value.ParseInt();
                operations.Add(new SwapScramblerOperation(index));
            }

            // Reverse
            else if (Regex.IsMatch(
                scramblerDefinition,
                $@"{Regex.Escape(calledFuncName)}:\bfunction\b\(\w+\)"))
            {
                operations.Add(new ReverseScramblerOperation());
            }
        }

        return new SignatureScrambler(operations);
    });

    private string? TryGetScramblerBody() => Memo.Cache(this, () =>
        Regex.Match(
            this.content,
            @"(\w+)=function\(\w+\){(\w+)=\2\.split\(\x22{2}\);.*?return\s+\2\.join\(\x22{2}\)}")
            .Groups[0]
            .Value
            .NullIfWhiteSpace());

    private string? TryGetScramblerDefinition() => Memo.Cache(this, () =>
    {
        var body = this.TryGetScramblerBody();
        if (string.IsNullOrWhiteSpace(body))
        {
            return null;
        }

        var objName = Regex.Match(body, "([\\$_\\w]+).\\w+\\(\\w+,\\d+\\);")
            .Groups[1]
            .Value;

        if (string.IsNullOrWhiteSpace(objName))
        {
            return null;
        }

        return Regex.Match(this.content, $@"var\s+{Regex.Escape(objName)}=\{{(\w+:function\(\w+(,\w+)?\)\{{(.*?)\}}),?\}};", RegexOptions.Singleline)
            .Groups[0]
            .Value
            .NullIfWhiteSpace();
    });
}

internal partial class PlayerSourceExtractor
{
    public static PlayerSourceExtractor Create(string raw) => new(raw);
}