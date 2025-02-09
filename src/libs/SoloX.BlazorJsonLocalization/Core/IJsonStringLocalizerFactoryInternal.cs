// ----------------------------------------------------------------------
// <copyright file="IJsonStringLocalizerFactoryInternal.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Core.Impl;

namespace SoloX.BlazorJsonLocalization.Core
{
    /// <summary>
    /// JsonStringLocalizerFactory for internal use.
    /// </summary>
    public interface IJsonStringLocalizerFactoryInternal
    {
        /// <summary>
        /// Create a string localizer setup with the given culture info.
        /// </summary>
        /// <param name="resourceSource"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        IStringLocalizerInternal CreateStringLocalizer(StringLocalizerResourceSource resourceSource, CultureInfo cultureInfo);

        /// <summary>
        /// Create the default string localizer setup with the given culture info.
        /// </summary>
        /// <param name="resourceSource"></param>
        /// <param name="cultureInfo"></param>
        /// <param name="stringLocalizerGuid"></param>
        /// <returns></returns>
        IStringLocalizerInternal CreateDefaultStringLocalizer(StringLocalizerResourceSource resourceSource, CultureInfo cultureInfo, string? stringLocalizerGuid);

        /// <summary>
        /// Find LocalizedString through both type and culture hierarchy.
        /// </summary>
        /// <param name="stringLocalizer"></param>
        /// <param name="cultureInfo"></param>
        /// <param name="findHandler"></param>
        /// <returns></returns>
        LocalizedString? FindThroughStringLocalizerHierarchy(IStringLocalizerInternal stringLocalizer, CultureInfo cultureInfo, Func<IStringLocalizerInternal, LocalizedString?> findHandler);

        /// <summary>
        /// Process through both type and culture hierarchy.
        /// </summary>
        /// <param name="stringLocalizer"></param>
        /// <param name="cultureInfo"></param>
        /// <param name="loadParentCulture"></param>
        /// <returns></returns>
        ValueTask LoadDataThroughStringLocalizerHierarchyAsync(IStringLocalizerInternal stringLocalizer, CultureInfo cultureInfo, bool loadParentCulture);
    }

    /// <summary>
    /// IStringLocalizerInternal for internal use
    /// </summary>
    public interface IStringLocalizerInternal
    {
        /// <summary>
        /// Gets the actual IStringLocalizer interface.
        /// </summary>
        IStringLocalizer AsStringLocalizer { get; }

        /// <summary>
        /// Culture info of the localizer.
        /// </summary>
        CultureInfo CultureInfo { get; }

        /// <summary>
        /// Load data
        /// </summary>
        /// <returns>True if the data exists.</returns>
        Task<bool> LoadDataAsync();

        /// <summary>
        /// Localizer Factory internal
        /// </summary>
        IJsonStringLocalizerFactoryInternal LocalizerFactoryInternal { get; }

        /// <summary>
        /// Resource source.
        /// </summary>
        StringLocalizerResourceSource ResourceSource { get; }

        /// <summary>
        /// Try get the LocalizedString.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The localized string or null</returns>
        LocalizedString? TryGet(string name);

        /// <summary>
        /// Try get the LocalizedString.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="arguments"></param>
        /// <param name="requestedCultureInfo"></param>
        /// <returns>The localized string or null</returns>
        LocalizedString? TryGet(string name, object[] arguments, CultureInfo requestedCultureInfo);
    }
}
