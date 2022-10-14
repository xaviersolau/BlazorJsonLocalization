// ----------------------------------------------------------------------
// <copyright file="JsonLocalizationOptions.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.BlazorJsonLocalization.Core;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SoloX.BlazorJsonLocalization
{
    /// <summary>
    /// Json localization options.
    /// </summary>
    public sealed class JsonLocalizationOptions
    {
        /// <summary>
        /// Get extension options collection. (set is used by the builder)
        /// </summary>
        public IEnumerable<IExtensionOptionsContainer> ExtensionOptions { get; internal set; }
            = Array.Empty<IExtensionOptionsContainer>();

        /// <summary>
        /// Get localization fallback list.
        /// </summary>
        public IEnumerable<(string baseName, Assembly assembly)> Fallbacks { get; internal set; }
            = Array.Empty<(string baseName, Assembly assembly)>();

        /// <summary>
        /// Tells if the keys must be displayed while resources are loading asynchronously.
        /// </summary>
        public bool IsDisplayKeysWhileLoadingAsynchronouslyEnabled { get; internal set; }
    }
}
