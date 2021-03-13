// ----------------------------------------------------------------------
// <copyright file="NullStringLocalizer.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SoloX.BlazorJsonLocalization.Core.Impl
{
    /// <summary>
    /// Null string localization implementation.
    /// </summary>
    public class NullStringLocalizer : IStringLocalizer
    {
        private readonly CultureInfo cultureInfo;

        /// <summary>
        /// Setup with target culture.
        /// </summary>
        /// <param name="cultureInfo">The target culture.</param>
        public NullStringLocalizer(CultureInfo cultureInfo)
        {
            this.cultureInfo = cultureInfo;
        }

        ///<inheritdoc/>
        public LocalizedString this[string name]
            => new(name, name, true);

        ///<inheritdoc/>
        public LocalizedString this[string name, params object[] arguments]
            => new(name, string.Format(this.cultureInfo, name, arguments), true);

        ///<inheritdoc/>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return Array.Empty<LocalizedString>();
        }
    }
}
