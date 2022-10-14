// ----------------------------------------------------------------------
// <copyright file="IJsonStringLocalizerFactoryInternal.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Globalization;
using Microsoft.Extensions.Localization;

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
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        IStringLocalizerInternal CreateStringLocalizer(CultureInfo cultureInfo);

        /// <summary>
        /// Process forwarder on all localizer hierarchy until we actually get a localized string.
        /// </summary>
        /// <param name="cultureInfo"></param>
        /// <param name="forward"></param>
        /// <returns></returns>
        LocalizedString? ProcessThroughStringLocalizerHierarchy(CultureInfo cultureInfo, Func<IStringLocalizerInternal, LocalizedString?> forward);
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
