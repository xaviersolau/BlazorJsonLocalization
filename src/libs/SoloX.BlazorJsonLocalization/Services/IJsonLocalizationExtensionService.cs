// ----------------------------------------------------------------------
// <copyright file="IJsonLocalizationExtensionService.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.BlazorJsonLocalization.Core;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;

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
        /// <returns>The resource map loaded if fund. Null otherwise.</returns>
        ValueTask<IReadOnlyDictionary<string, string>?> TryLoadAsync(
            AExtensionOptions extensionOptions,
            Assembly assembly,
            string baseName,
            CultureInfo cultureInfo);
    }

    /// <summary>
    /// Typed interface of the JsonLocalization extension service
    /// </summary>
    /// <typeparam name="TOptions">The extension options type</typeparam>
    public interface IJsonLocalizationExtensionService<TOptions> : IJsonLocalizationExtensionService
        where TOptions : AExtensionOptions
    {
        ///<inheritdoc/>
        ValueTask<IReadOnlyDictionary<string, string>?> IJsonLocalizationExtensionService.TryLoadAsync(
            AExtensionOptions extensionOptions,
            Assembly assembly,
            string baseName,
            CultureInfo cultureInfo)
        {
            var typedExtensionOptions = (TOptions)extensionOptions;

            return TryLoadAsync(typedExtensionOptions, assembly, baseName, cultureInfo);
        }

        /// <summary>
        /// Try to load Json resources from the given parameters.
        /// </summary>
        /// <param name="options">The extension options.</param>
        /// <param name="assembly">The resource assembly.</param>
        /// <param name="baseName">The resource base name.</param>
        /// <param name="cultureInfo">The target culture info.</param>
        /// <returns>The resource map loaded if fund. Null otherwise.</returns>
        ValueTask<IReadOnlyDictionary<string, string>?> TryLoadAsync(
            TOptions options,
            Assembly assembly,
            string baseName,
            CultureInfo cultureInfo);
    }
}
