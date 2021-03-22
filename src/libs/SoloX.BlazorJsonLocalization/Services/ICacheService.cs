// ----------------------------------------------------------------------
// <copyright file="ICacheService.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Reflection;

namespace SoloX.BlazorJsonLocalization.Services
{
    /// <summary>
    /// Cache service interface.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Cache the given localizer instance.
        /// </summary>
        /// <param name="assembly">Resource assembly.</param>
        /// <param name="baseName">Resource BaseName.</param>
        /// <param name="cultureInfo">Resource CultureInfo.</param>
        /// <param name="localizer">Localizer instance to cache.</param>
        void Cache(Assembly assembly, string baseName, CultureInfo cultureInfo, IStringLocalizer localizer);

        /// <summary>
        /// Try to match the cache entry.
        /// </summary>
        /// <param name="assembly">Resource assembly.</param>
        /// <param name="baseName">Resource BaseName.</param>
        /// <param name="cultureInfo">Resource CultureInfo.</param>
        /// <returns>The localizer if cached or null otherwise.</returns>
        IStringLocalizer? Match(Assembly assembly, string baseName, CultureInfo cultureInfo);
    }
}
