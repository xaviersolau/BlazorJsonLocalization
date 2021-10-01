// ----------------------------------------------------------------------
// <copyright file="IJsonStringLocalizerFactoryInternal.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Globalization;
using Microsoft.Extensions.Localization;

namespace SoloX.BlazorJsonLocalization.Core.Impl
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
        IStringLocalizer CreateStringLocalizer(CultureInfo cultureInfo);
    }
}
