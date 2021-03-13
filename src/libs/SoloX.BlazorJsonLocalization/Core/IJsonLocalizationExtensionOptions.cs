// ----------------------------------------------------------------------
// <copyright file="IJsonLocalizationExtensionOptions.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.BlazorJsonLocalization.Services;
using System;

namespace SoloX.BlazorJsonLocalization.Core
{
    /// <summary>
    /// Json localization extension options.
    /// </summary>
    public interface IJsonLocalizationExtensionOptions
    {
        /// <summary>
        /// Get the Json Localization extension service.
        /// </summary>
        /// <param name="serviceProvider">The service provider where to get the service from.</param>
        /// <returns>The Json Localization extension service.</returns>
        public IJsonLocalizationExtensionService GetJsonLocalizationExtensionService(IServiceProvider serviceProvider);
    }
}
