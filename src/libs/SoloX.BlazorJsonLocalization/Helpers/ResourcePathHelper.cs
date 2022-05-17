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
        /// <returns>The converted resource base path to load.</returns>
        public static string ComputeBasePath(Assembly assembly, string baseName)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }
            if (baseName == null)
            {
                throw new ArgumentNullException(nameof(baseName));
            }

            var assemblyName = assembly.GetName().Name;
            var basePath = baseName;

            if (!string.IsNullOrEmpty(assemblyName)
                && baseName.StartsWith($"{assemblyName}.", StringComparison.Ordinal))
            {
                basePath = baseName.Remove(0, assemblyName.Length + 1);
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
             : new Uri($"{basePath}-{cultureName}.json", UriKind.Relative);
        }
    }
}
