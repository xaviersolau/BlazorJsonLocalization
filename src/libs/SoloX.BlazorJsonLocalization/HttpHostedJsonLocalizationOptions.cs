// ----------------------------------------------------------------------
// <copyright file="HttpHostedJsonLocalizationOptions.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.BlazorJsonLocalization.Core;
using System;
using System.Reflection;

namespace SoloX.BlazorJsonLocalization
{
    /// <summary>
    /// JsonLocalizer Http Hosted extension options
    /// </summary>
    public class HttpHostedJsonLocalizationOptions : AJsonExtensionOptions
    {
        /// <summary>
        /// Gets/Sets Path where to get the resources.
        /// </summary>
        public string ResourcesPath { get; set; } = string.Empty;

        /// <summary>
        /// Set the delegate for custom file naming
        /// </summary>
        public Func<string, string, Uri> NamingPolicy { get; set; } =
            (basePath, cultureName) => string.IsNullOrEmpty(cultureName)
             ? new Uri($"{basePath}.json", UriKind.Relative)
             : new Uri($"{basePath}-{cultureName}.json", UriKind.Relative);

        /// <summary>
        /// Adds a querystring version to the resource path. Use it for cache busting
        /// </summary>
        public string Version { get; set; } = string.Empty;

        /// <summary>
        /// Gets/Sets the application assembly.
        /// </summary>
        public Assembly? ApplicationAssembly { get; set; }
    }
}
