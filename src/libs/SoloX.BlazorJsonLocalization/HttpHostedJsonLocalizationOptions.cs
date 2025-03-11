// ----------------------------------------------------------------------
// <copyright file="HttpHostedJsonLocalizationOptions.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.BlazorJsonLocalization.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SoloX.BlazorJsonLocalization
{
    /// <summary>
    /// JsonLocalizer Http Hosted extension options
    /// </summary>
    public class HttpHostedJsonLocalizationOptions : AJsonExtensionOptions
    {
        /// <summary>
        /// Naming policy handler used to compute the actual Http hosted Json file to fetch.
        /// </summary>
        /// <param name="basePath">The base resource path of the json file.</param>
        /// <param name="cultureName">The culture name (if any).</param>
        /// <returns>The actual Uri to load.</returns>
        public delegate Uri NamingPolicyHandler(string basePath, string cultureName);

        /// <summary>
        /// Gets/Sets Path where to get the resources.
        /// </summary>
        public string ResourcesPath { get; set; } = string.Empty;

        /// <summary>
        /// Set the delegate for custom file naming.
        /// For example:
        /// (basePath, cultureName) => new Uri($"{basePath}{(string.IsNullOrEmpty(cultureName) ? string.Empty : $"-{cultureName}")}.json", UriKind.Relative);
        /// </summary>
        public NamingPolicyHandler? NamingPolicy { get; set; }

        /// <summary>
        /// Gets/Sets the application assembly. It will set/reset ApplicationAssemblies or it will get the first from ApplicationAssemblies.
        /// </summary>
        public Assembly? ApplicationAssembly
        {
            get
            {
                return ApplicationAssemblies.FirstOrDefault();
            }
            set
            {
                ApplicationAssemblies = value != null ? [value] : Array.Empty<Assembly>();
            }
        }

        /// <summary>
        /// Gets/Sets the application assemblies.
        /// </summary>
        public IEnumerable<Assembly> ApplicationAssemblies { get; set; } = Array.Empty<Assembly>();
    }
}
