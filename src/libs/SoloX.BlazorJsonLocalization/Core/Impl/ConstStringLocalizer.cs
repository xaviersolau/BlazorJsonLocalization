// ----------------------------------------------------------------------
// <copyright file="ConstStringLocalizer.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;

#if !NET
using System.Globalization;
#endif

namespace SoloX.BlazorJsonLocalization.Core.Impl
{
    /// <summary>
    /// Constant string localizer.
    /// </summary>
    public class ConstStringLocalizer : IStringLocalizer
    {
        private readonly string constValue;
        private readonly IJsonStringLocalizerFactoryInternal localizerFactory;

        /// <summary>
        /// Setup the ConstStringLocalizer with the constant value
        /// </summary>
        /// <param name="constValue">Constant value to use.</param>
        /// <param name="localizerFactory">Localizer Internal Factory.</param>
        public ConstStringLocalizer(string constValue, IJsonStringLocalizerFactoryInternal localizerFactory)
        {
            this.localizerFactory = localizerFactory ?? throw new ArgumentNullException(nameof(localizerFactory));

            this.constValue = constValue;
        }

        ///<inheritdoc/>
        public LocalizedString this[string name] => new LocalizedString(name, this.constValue);

        ///<inheritdoc/>
        public LocalizedString this[string name, params object[] arguments] => new LocalizedString(name, this.constValue);

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
