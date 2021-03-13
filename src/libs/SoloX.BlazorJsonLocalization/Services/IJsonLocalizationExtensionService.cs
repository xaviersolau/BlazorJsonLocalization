// ----------------------------------------------------------------------
// <copyright file="IJsonLocalizationExtensionService.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.BlazorJsonLocalization.Core;
using SoloX.BlazorJsonLocalization.Core.Impl;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace SoloX.BlazorJsonLocalization.Services
{
    /// <summary>
    /// Base agnostic interface of the JsonLocalization extension service.
    /// </summary>
    public interface IJsonLocalizationExtensionService
    {
        /// <summary>
        /// Try to load Json resources from the given parameters.
        /// </summary>
        /// <param name="extensionOptions">The agnostic extension options.</param>
        /// <param name="assembly">The resource assembly.</param>
        /// <param name="baseName">The resource base name.</param>
        /// <param name="cultureInfo">The target culture info.</param>
        /// <param name="map">The resource map loaded if fund.</param>
        /// <returns>True if the resources have been located successfully.</returns>
        bool TryLoad(
            IJsonLocalizationExtensionOptions extensionOptions,
            Assembly assembly,
            string baseName,
            CultureInfo cultureInfo,
            out IReadOnlyDictionary<string, string> map);
    }

    /// <summary>
    /// Typed interface of the JsonLocalization extension service
    /// </summary>
    /// <typeparam name="TOptions">The extension options type</typeparam>
    public interface IJsonLocalizationExtensionService<TOptions> : IJsonLocalizationExtensionService
    {
        bool IJsonLocalizationExtensionService.TryLoad(
            IJsonLocalizationExtensionOptions extensionOptions,
            Assembly assembly,
            string baseName,
            CultureInfo cultureInfo,
            out IReadOnlyDictionary<string, string> map)
        {
            var typedExtensionOptions = (JsonLocalizationExtensionOptions<TOptions>)extensionOptions;

            return TryLoad(typedExtensionOptions.Options, assembly, baseName, cultureInfo, out map);
        }

        /// <summary>
        /// Try to load Json resources from the given parameters.
        /// </summary>
        /// <param name="options">The extension options.</param>
        /// <param name="assembly">The resource assembly.</param>
        /// <param name="baseName">The resource base name.</param>
        /// <param name="cultureInfo">The target culture info.</param>
        /// <param name="map">The resource map loaded if fund.</param>
        /// <returns>True if the resources have been located successfully.</returns>
        bool TryLoad(
            TOptions options,
            Assembly assembly,
            string baseName,
            CultureInfo cultureInfo,
            out IReadOnlyDictionary<string, string> map);
    }
}
