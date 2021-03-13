// ----------------------------------------------------------------------
// <copyright file="ICultureInfoService.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Globalization;

namespace SoloX.BlazorJsonLocalization.Services
{
    /// <summary>
    /// CultureInfo service to provide abstraction.
    /// </summary>
    public interface ICultureInfoService
    {
        /// <summary>
        /// Get the current UI cultureInfo.
        /// </summary>
        CultureInfo CurrentUICulture { get; }
    }
}
