// ----------------------------------------------------------------------
// <copyright file="JsonLocalizationExtensionOptions.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using SoloX.BlazorJsonLocalization.Services;
using System;

namespace SoloX.BlazorJsonLocalization.Core.Impl
{
    /// <summary>
    /// Typed JsonLocalisation extension options.
    /// </summary>
    /// <typeparam name="TOptions">The extension options type.</typeparam>
    public sealed class JsonLocalizationExtensionOptions<TOptions> : IJsonLocalizationExtensionOptions
    {
        /// <summary>
        /// Setup the JsonLocalizationExtensionOptions with the given options.
        /// </summary>
        /// <param name="options">The extension options</param>
        public JsonLocalizationExtensionOptions(TOptions options)
        {
            Options = options;
        }

        /// <summary>
        /// Get the typed extension options.
        /// </summary>
        public TOptions Options { get; }

        /// <summary>
        /// Get the appropriate Json localization extension service given the options type from
        /// the given service provider.
        /// </summary>
        /// <param name="serviceProvider">The service provider to get the extension service from.</param>
        /// <returns>The extension service.</returns>
        public IJsonLocalizationExtensionService GetJsonLocalizationExtensionService(
            IServiceProvider serviceProvider)
        {
            var optionsService = serviceProvider
                .GetRequiredService<IJsonLocalizationExtensionService<TOptions>>();

            return optionsService;
        }
    }
}
