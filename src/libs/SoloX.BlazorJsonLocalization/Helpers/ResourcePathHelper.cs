// ----------------------------------------------------------------------
// <copyright file="ResourcePathHelper.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Reflection;

namespace SoloX.BlazorJsonLocalization.Helpers
{
    /// <summary>
    /// Resource path helpers
    /// </summary>
    public static class ResourcePathHelper
    {
        /// <summary>
        /// Compute the resource base path from the given assembly/baseName.
        /// </summary>
        /// <param name="assembly">The assembly declaring resource</param>
        /// <param name="baseName">The baseName of the resource to load.</param>
        /// <param name="rootNameSpace">The root namespace if not the assembly name.</param>
        /// <returns>The converted resource base path to load.</returns>
        public static string ComputeBasePath(Assembly assembly, string baseName, string rootNameSpace)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }
            if (baseName == null)
            {
                throw new ArgumentNullException(nameof(baseName));
            }

            var basePath = baseName;

            if (!string.IsNullOrEmpty(rootNameSpace)
                && baseName.StartsWith($"{rootNameSpace}.", StringComparison.Ordinal))
            {
                basePath = baseName.Substring(rootNameSpace.Length + 1);
            }

            basePath = basePath.Replace('.', '/');

            return basePath;
        }

        /// <summary>
        /// Default naming policy for Http Hosted Json files.
        /// </summary>
        /// <param name="basePath">The base resource path of the json file.</param>
        /// <param name="cultureName">The culture name (if any).</param>
        /// <returns></returns>
        public static Uri DefaultHttpHostedJsonNamingPolicy(string basePath, string cultureName)
        {
            return string.IsNullOrEmpty(cultureName)
                ? new Uri($"{basePath}.json", UriKind.Relative)
                : new Uri($"{basePath}.{cultureName}.json", UriKind.Relative);
        }

        /// <summary>
        /// Default naming policy for Embedded Json files.
        /// </summary>
        /// <param name="basePath">The base resource path of the json file.</param>
        /// <param name="cultureName">The culture name (if any).</param>
        /// <returns></returns>
        public static string DefaultEmbeddedJsonNamingPolicy(string basePath, string cultureName)
        {
            return string.IsNullOrEmpty(cultureName)
                ? $"{basePath}.json"
                : $"{basePath}.{cultureName}.json";
        }
    }
}
