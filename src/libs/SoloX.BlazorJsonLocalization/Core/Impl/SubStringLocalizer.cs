// ----------------------------------------------------------------------
// <copyright file="SubStringLocalizer.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
#if !NET
using System.Globalization;
#endif

namespace SoloX.BlazorJsonLocalization.Core.Impl
{
    /// <summary>
    /// Sub string localizer implement a localizer proxy that add a key prefix when accessing a localized resource.
    /// </summary>
    public class SubStringLocalizer<T> : IStringLocalizer<T>
    {
        private readonly IStringLocalizer localizer;
        private readonly string structuredKeyPrefix;

        /// <summary>
        /// Setup instance.
        /// </summary>
        /// <param name="localizer">Base localizer reference.</param>
        /// <param name="structuredKeyPrefix">Structured key to use as prefix.</param>
        public SubStringLocalizer(IStringLocalizer localizer, string structuredKeyPrefix)
        {
            this.localizer = localizer;
            this.structuredKeyPrefix = structuredKeyPrefix;
        }

        ///<inheritdoc/>
        public LocalizedString this[string name] => this.localizer[this.structuredKeyPrefix + name];

        ///<inheritdoc/>
        public LocalizedString this[string name, params object[] arguments] => this.localizer[this.structuredKeyPrefix + name, arguments];

        ///<inheritdoc/>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return this.localizer.GetAllStrings(includeParentCultures).Where(s => s.Name.StartsWith(this.structuredKeyPrefix, StringComparison.InvariantCulture));
        }

#if !NET
        ///<inheritdoc/>
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new SubStringLocalizer<T>(this.localizer.WithCulture(culture), this.structuredKeyPrefix);
        }
#endif
    }
}
