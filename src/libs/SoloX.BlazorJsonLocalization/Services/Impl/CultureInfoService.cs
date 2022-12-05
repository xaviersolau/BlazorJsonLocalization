// ----------------------------------------------------------------------
// <copyright file="CultureInfoService.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Globalization;

namespace SoloX.BlazorJsonLocalization.Services.Impl
{
    /// <summary>
    /// CultureInfo service implementation.
    /// </summary>
    public class CultureInfoService : ICultureInfoService
    {
        ///<inheritdoc/>
        public CultureInfo CurrentUICulture
        {
            get
            {
                return CultureInfo.CurrentUICulture;
            }
        }
    }
}
