// ----------------------------------------------------------------------
// <copyright file="EmbeddedJsonLocalizationOptions.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.BlazorJsonLocalization.Core;
using System.Reflection;

namespace SoloX.BlazorJsonLocalization
{
    /// <summary>
    /// JsonLocalizer Embedded extension options
    /// </summary>
    public class EmbeddedJsonLocalizationOptions : AJsonExtensionOptions
    {
        /// <summary>
        /// Naming policy handler used to compute the actual embedded Json file to fetch.
        /// </summary>
        /// <param name="basePath">The base resource path of the json file.</param>
        /// <param name="cultureName">The culture name (if any).</param>
        /// <returns>The actual embedded resource name to load.</returns>
        public delegate string? NamingPolicyHandler(string basePath, string cultureName);

        /// <summary>
        /// Assembly Root NameSpace resolver handler.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public delegate string RootNameSpaceResolverHandler(Assembly assembly);

        /// <summary>
        /// Gets/Sets Path where to get the resources.
        /// </summary>
        public string ResourcesPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets/Sets Assembly Root NameSpace resolver.
        /// </summary>
        /// <remarks>
        /// By default the root name space will be resolved as the assembly name.
        /// </remarks>
        public RootNameSpaceResolverHandler? RootNameSpaceResolver { get; set; }

        /// <summary>
        /// Set the delegate for custom file naming.
        /// For example:
        /// (basePath, cultureName) => $"{basePath}{(string.IsNullOrEmpty(cultureName) ? string.Empty : $"-{cultureName}")}.json";
        /// </summary>
        public NamingPolicyHandler? NamingPolicy { get; set; }
    }
}
