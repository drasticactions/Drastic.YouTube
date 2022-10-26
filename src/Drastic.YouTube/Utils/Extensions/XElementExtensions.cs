// <copyright file="XElementExtensions.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Linq;
using System.Xml.Linq;

namespace Drastic.YouTube.Utils.Extensions;

internal static class XElementExtensions
{
    public static XElement StripNamespaces(this XElement element)
    {
        // Adapted from http://stackoverflow.com/a/1147012
        var result = new XElement(element);

        foreach (var descendantElement in result.DescendantsAndSelf())
        {
            descendantElement.Name = XNamespace.None.GetName(descendantElement.Name.LocalName);

            descendantElement.ReplaceAttributes(
                descendantElement
                    .Attributes()
                    .Where(a => !a.IsNamespaceDeclaration)
                    .Where(a => a.Name.Namespace != XNamespace.Xml && a.Name.Namespace != XNamespace.Xmlns)
                    .Select(a => new XAttribute(XNamespace.None.GetName(a.Name.LocalName), a.Value)));
        }

        return result;
    }
}