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

namespace SoloX.BlazorJsonLocalization
{
    /// <summary>
    /// Extension methods to setup embedded Json file support.
    /// </summary>
    public static class JsonLocalizationOptionsBuilderExtensions
    {
        /// <summary>
        /// Setup default embedded Json support.
        /// </summary>
        /// <param name="builder">The builder to setup.</param>
        /// <returns>The given builder updated with the Json embedded options.</returns>
        public static JsonLocalizationOptionsBuilder UseEmbeddedJson(
            this JsonLocalizationOptionsBuilder builder)
            => builder.UseEmbeddedJson(null);

        /// <summary>
        /// Setup embedded Json support with the given setup action.
        /// </summary>
        /// <param name="builder">The builder to setup.</param>
        /// <param name="setup">The setup action.</param>
        /// <returns>The given builder updated with the Json embedded options.</returns>
        public static JsonLocalizationOptionsBuilder UseEmbeddedJson(
            this JsonLocalizationOptionsBuilder builder,
            Action<EmbeddedJsonLocalizationOptions>? setup)
        {
            ArgumentNullException.ThrowIfNull(builder, nameof(builder));

            var optExt = new EmbeddedJsonLocalizationOptions();

            setup?.Invoke(optExt);

            return builder.AddExtensionOptions(optExt);
        }
    }
}
