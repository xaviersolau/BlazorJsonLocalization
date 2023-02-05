// ----------------------------------------------------------------------
// <copyright file="StringLocalizerExtensions.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Core.Impl;

namespace SoloX.BlazorJsonLocalization
{
    /// <summary>
    /// StringLocalizer extension methods.
    /// </summary>
    public static class StringLocalizerExtensions
    {
        /// <summary>
        /// Get the localized string resource as a HTML Markup String in order to render it as HTML content.
        /// </summary>
        /// <param name="localizer">The localizer instance where to get the string resource from.</param>
        /// <param name="key">The string resource key.</param>
        /// <param name="arguments">Optional arguments</param>
        /// <returns>The HTML Markup String.</returns>
        public static MarkupString Html(this IStringLocalizer localizer, string key, params object[] arguments)
        {
            if (localizer == null)
            {
                throw new ArgumentNullException(nameof(localizer));
            }
            var txt = localizer[key, arguments];
            return new MarkupString(txt ?? string.Empty);
        }

        /// <summary>
        /// Make sure all asynchronous localizer loading is completed.
        /// </summary>
        /// <param name="localizer">Localizer to load.</param>
        /// <param name="loadParentCulture">If true (false by default) the LoadAsync method will load the parent culture.</param>
        /// <returns>The value task to await.</returns>
        /// <remarks>The parent culture is actually used as fall back if a key is not found.</remarks>
        public static ValueTask LoadAsync(this IStringLocalizer localizer, bool loadParentCulture = false)
        {
            if (localizer == null)
            {
                throw new ArgumentNullException(nameof(localizer));
            }

            return JsonStringLocalizerAsync.LoadAsync(localizer, loadParentCulture);
        }

        /// <summary>
        /// Create a SubLocalizer instance.
        /// </summary>
        /// <param name="localizer">Localizer to get the sub localizer from.</param>
        /// <returns>The sub-localizer.</returns>
        /// <remarks>The sub localizer can be used to access structured Json object.</remarks>
        public static IStringLocalizer GetSubLocalizer(this IStringLocalizer localizer, params string[] structuredKey)
        {
            if (localizer == null)
            {
                throw new ArgumentNullException(nameof(localizer));
            }

            if (structuredKey == null || structuredKey.Length == 0)
            {
                return localizer;
            }

            return new SubStringLocalizer(localizer, string.Join(Key.Separator, structuredKey) + Key.Separator);
        }
    }
}
