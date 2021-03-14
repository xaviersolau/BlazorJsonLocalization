// ----------------------------------------------------------------------
// <copyright file="StringLocalizerExtensions.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

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
        /// <typeparam name="T">String localizer component type.</typeparam>
        /// <param name="localizer">The localizer instance where to get the string resource from.</param>
        /// <param name="key">The string resource key.</param>
        /// <param name="arguments">Optional arguments</param>
        /// <returns>The HTML Markup String.</returns>
        public static MarkupString Html<T>(this IStringLocalizer<T> localizer, string key, params object[] arguments)
        {
            if (localizer == null)
            {
                throw new ArgumentNullException(nameof(localizer));
            }
            var txt = localizer[key, arguments];
            return new MarkupString(txt ?? string.Empty);
        }
    }
}
