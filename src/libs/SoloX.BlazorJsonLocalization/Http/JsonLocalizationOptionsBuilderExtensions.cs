// ----------------------------------------------------------------------
// <copyright file="JsonLocalizationOptionsBuilderExtensions.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;

#if !NET6_0_OR_GREATER
using ArgumentNullException = SoloX.BlazorJsonLocalization.Helpers.ArgumentNullExceptionLegacy;
#endif

namespace SoloX.BlazorJsonLocalization.Http
{
    /// <summary>
    /// Extension methods to setup Http hosted Json file support.
    /// </summary>
    public static class JsonLocalizationOptionsBuilderExtensions
    {
        /// <summary>
        /// Setup default Http hosted Json support.
        /// </summary>
        /// <param name="builder">The builder to setup.</param>
        /// <returns>The given builder updated with the Json Http hosted options.</returns>
        public static JsonLocalizationOptionsBuilder UseHttpHostedJson<TOptions>(
            this JsonLocalizationOptionsBuilder builder)
            where TOptions : HttpHostedJsonLocalizationOptions, new()
            => builder.UseHttpHostedJson<TOptions>(null);

        /// <summary>
        /// Setup Http hosted Json support with the given setup action.
        /// </summary>
        /// <param name="builder">The builder to setup.</param>
        /// <param name="setup">The setup action.</param>
        /// <returns>The given builder updated with the Json http hosted options.</returns>
        public static JsonLocalizationOptionsBuilder UseHttpHostedJson<TOptions>(
            this JsonLocalizationOptionsBuilder builder,
            Action<TOptions>? setup)
            where TOptions : HttpHostedJsonLocalizationOptions, new()
        {
            ArgumentNullException.ThrowIfNull(builder, nameof(builder));

            var optExt = new TOptions();

            setup?.Invoke(optExt);

            return builder.AddExtensionOptions(optExt);
        }
    }
}
