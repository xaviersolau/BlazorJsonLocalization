// ----------------------------------------------------------------------
// <copyright file="EmbeddedJsonLocalizationOptions.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.BlazorJsonLocalization.Core;

namespace SoloX.BlazorJsonLocalization
{
    /// <summary>
    /// JsonLocalizer Embedded extension options
    /// </summary>
    public class EmbeddedJsonLocalizationOptions : AExtensionOptions
    {
        /// <summary>
        /// Gets/Sets Path where to get the resources.
        /// </summary>
        public string ResourcesPath { get; set; } = string.Empty;
    }
}
