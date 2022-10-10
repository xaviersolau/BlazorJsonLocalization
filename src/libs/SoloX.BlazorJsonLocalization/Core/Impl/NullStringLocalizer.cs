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
    public class NullStringLocalizer : IStringLocalizer, IStringLocalizerInternal
    {
        private readonly CultureInfo cultureInfo;
        private readonly IJsonStringLocalizerFactoryInternal localizerFactory;
        private readonly bool resourceNotFound;

        /// <summary>
        /// Setup with target culture.
        /// </summary>
        /// <param name="cultureInfo">The target culture.</param>
        /// <param name="localizerFactory">Localizer Internal Factory.</param>
        /// <param name="resourceNotFound">Tells if resourceNotFound property must be set.</param>
        public NullStringLocalizer(CultureInfo cultureInfo, IJsonStringLocalizerFactoryInternal localizerFactory, bool resourceNotFound)
        {
            this.localizerFactory = localizerFactory ?? throw new ArgumentNullException(nameof(localizerFactory));

            this.cultureInfo = cultureInfo;
            this.resourceNotFound = resourceNotFound;
        }

        ///<inheritdoc/>
        public LocalizedString this[string name]
            => new LocalizedString(name, name, this.resourceNotFound);

        ///<inheritdoc/>
        public LocalizedString this[string name, params object[] arguments]
            => new LocalizedString(name, string.Format(this.cultureInfo, name, arguments), this.resourceNotFound);

        ///<inheritdoc/>
        public IStringLocalizer AsStringLocalizer => this;

        ///<inheritdoc/>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return Array.Empty<LocalizedString>();
        }

        ///<inheritdoc/>
        public LocalizedString? TryGet(string name)
        {
            return null;
        }

        ///<inheritdoc/>
        public LocalizedString? TryGet(string name, object[] arguments, CultureInfo requestedCultureInfo)
        {
            return null;
        }

#if !NET
        ///<inheritdoc/>
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return this.localizerFactory.CreateStringLocalizer(culture).AsStringLocalizer;
        }
#endif
    }
}
