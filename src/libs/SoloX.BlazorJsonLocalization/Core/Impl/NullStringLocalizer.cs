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
        private readonly IJsonStringLocalizerFactoryInternal localizerFactory;

        /// <summary>
        /// Setup with target culture.
        /// </summary>
        /// <param name="cultureInfo">The target culture.</param>
        /// <param name="localizerFactory">Localizer Internal Factory.</param>
        public NullStringLocalizer(CultureInfo cultureInfo, IJsonStringLocalizerFactoryInternal localizerFactory)
        {
            this.localizerFactory = localizerFactory ?? throw new ArgumentNullException(nameof(localizerFactory));

            this.cultureInfo = cultureInfo;
        }

        ///<inheritdoc/>
        public LocalizedString this[string name]
            => new LocalizedString(name, name, true);

        ///<inheritdoc/>
        public LocalizedString this[string name, params object[] arguments]
            => new LocalizedString(name, string.Format(this.cultureInfo, name, arguments), true);

        ///<inheritdoc/>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return Array.Empty<LocalizedString>();
        }

#if !NET
        ///<inheritdoc/>
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return this.localizerFactory.CreateStringLocalizer(culture);
        }
#endif
    }
}
