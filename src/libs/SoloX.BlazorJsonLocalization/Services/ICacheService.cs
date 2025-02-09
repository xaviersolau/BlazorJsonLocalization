// ----------------------------------------------------------------------
// <copyright file="ICacheService.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.BlazorJsonLocalization.Core;
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
        /// <returns>The given localizer or the one from the cache if any.</returns>
        IStringLocalizerInternal Cache(Assembly assembly, string baseName, CultureInfo? cultureInfo, IStringLocalizerInternal localizer);

        /// <summary>
        /// Try to match the cache entry.
        /// </summary>
        /// <param name="assembly">Resource assembly.</param>
        /// <param name="baseName">Resource BaseName.</param>
        /// <param name="cultureInfo">Resource CultureInfo.</param>
        /// <returns>The localizer if cached or null otherwise.</returns>
        IStringLocalizerInternal? Match(Assembly assembly, string baseName, CultureInfo? cultureInfo);

        /// <summary>
        /// Reset the cache entry.
        /// </summary>
        /// <param name="assembly">Resource assembly.</param>
        /// <param name="baseName">Resource BaseName.</param>
        /// <param name="cultureInfo">Resource CultureInfo.</param>
        /// <returns>True if the cache entry was found.</returns>
        bool Reset(Assembly assembly, string baseName, CultureInfo cultureInfo);
    }
}
