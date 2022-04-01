// ----------------------------------------------------------------------
// <copyright file="HttpHostedJsonLocalizationOptions.cs" company="Xavier Solau">
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
    /// JsonLocalizer Http Hosted extension options
    /// </summary>
    public class HttpHostedJsonLocalizationOptions : AJsonExtensionOptions
    {
        /// <summary>
        /// Gets/Sets Path where to get the resources.
        /// </summary>
        public string ResourcesPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets/Sets the separator for the culture. Defaults to '-'
        /// </summary>
        public string CultureSeparator { get; set; } = "-";

        /// <summary>
        /// Gets/Sets the application assembly.
        /// </summary>
        public Assembly? ApplicationAssembly { get; set; }
    }
}
