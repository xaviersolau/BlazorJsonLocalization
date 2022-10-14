// ----------------------------------------------------------------------
// <copyright file="JsonLocalizationOptionsBuilder.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.BlazorJsonLocalization.Core;
using SoloX.BlazorJsonLocalization.Core.Impl;
using SoloX.BlazorJsonLocalization.Helpers;
using System.Collections.Generic;
using System.Reflection;

namespace SoloX.BlazorJsonLocalization
{
    /// <summary>
    /// Json localization options builder.
    /// </summary>
    public sealed class JsonLocalizationOptionsBuilder
    {
        private readonly IList<IExtensionOptionsContainer> extensionOptions = new List<IExtensionOptionsContainer>();
        private readonly IList<(string baseName, Assembly assembly)> fallbackList = new List<(string baseName, Assembly assembly)>();

        private bool enableDisplayKeysWhileLoadingAsynchronously;


        /// <summary>
        /// Enable (or not) the option to return keys on localization result while resources are loading asynchronously.
        /// </summary>
        /// <param name="enable">Enable or not the option.</param>
        /// <returns>The current builder.</returns>
        public JsonLocalizationOptionsBuilder EnableDisplayKeysWhileLoadingAsynchronously(bool enable = true)
        {
            this.enableDisplayKeysWhileLoadingAsynchronously = enable;
            return this;
        }

        /// <summary>
        /// Add a fallback localizer.
        /// </summary>
        /// <typeparam name="TLocalizer">The type defining the localization.</typeparam>
        /// <returns>Self.</returns>
        public JsonLocalizationOptionsBuilder AddFallback<TLocalizer>()
        {
            var localizerType = typeof(TLocalizer);
            AddFallback(localizerType.GetBaseName(), localizerType.Assembly);
            return this;
        }

        /// <summary>
        /// Add a fallback localizer.
        /// </summary>
        /// <param name="baseName">Localization file base name.</param>
        /// <param name="assembly">Assembly where to find the localization resources.</param>
        /// <returns>Self.</returns>
        public JsonLocalizationOptionsBuilder AddFallback(string baseName, Assembly assembly)
        {
            this.fallbackList.Add((baseName, assembly));
            return this;
        }

        /// <summary>
        /// Add extension options.
        /// </summary>
        /// <typeparam name="TOptions">Type of the extension options.</typeparam>
        /// <param name="options">The options to add.</param>
        /// <returns>The current builder.</returns>
        public JsonLocalizationOptionsBuilder AddExtensionOptions<TOptions>(TOptions options)
            where TOptions : AExtensionOptions
        {
            this.extensionOptions.Add(new ExtensionOptionsContainer<TOptions>(options));
            return this;
        }

        internal void Build(JsonLocalizationOptions opt)
        {
            if (opt != null)
            {
                opt.IsDisplayKeysWhileLoadingAsynchronouslyEnabled = this.enableDisplayKeysWhileLoadingAsynchronously;
                opt.ExtensionOptions = this.extensionOptions;
                opt.Fallbacks = this.fallbackList;
            }
        }
    }
}
