// ----------------------------------------------------------------------
// <copyright file="JsonLocalizationOptionsBuilderExtensions.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;

namespace SoloX.BlazorJsonLocalization.ServerSide
{
    /// <summary>
    /// Extension methods to setup embedded Json file support.
    /// </summary>
    public static class JsonLocalizationOptionsBuilderExtensions
    {
        /// <summary>
        /// Setup default Http hosted Json support.
        /// </summary>
        /// <param name="builder">The builder to setup.</param>
        /// <returns>The given builder updated with the Json Http hosted options.</returns>
        public static JsonLocalizationOptionsBuilder UseHttpHostedJson(
            this JsonLocalizationOptionsBuilder builder)
            => builder.UseHttpHostedJson(null);

        /// <summary>
        /// Setup Http hosted Json support with the given setup action.
        /// </summary>
        /// <param name="builder">The builder to setup.</param>
        /// <param name="setup">The setup action.</param>
        /// <returns>The given builder updated with the Json http hosted options.</returns>
        public static JsonLocalizationOptionsBuilder UseHttpHostedJson(
            this JsonLocalizationOptionsBuilder builder,
            Action<HttpHostedJsonLocalizationOptions>? setup)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var optExt = new HttpHostedJsonLocalizationOptions();

            setup?.Invoke(optExt);

            return builder.AddExtensionOptions(optExt);
        }
    }
}
