// ----------------------------------------------------------------------
// <copyright file="JsonStringLocalizer.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Localization;

namespace SoloX.BlazorJsonLocalization.Core.Impl
{
    /// <summary>
    /// IStringLocalizer implementation.
    /// </summary>
    public class JsonStringLocalizer : IStringLocalizer
    {
        private readonly IReadOnlyDictionary<string, string> stringMap;
        private readonly CultureInfo cultureInfo;
        private readonly IJsonStringLocalizerFactoryInternal localizerFactory;

        /// <summary>
        /// Setup with the given string map.
        /// </summary>
        /// <param name="stringMap">The string map to use to resolve localization.</param>
        /// <param name="cultureInfo">The associated culture info.</param>
        /// <param name="localizerFactory">Localizer Internal Factory.</param>
        public JsonStringLocalizer(IReadOnlyDictionary<string, string> stringMap, CultureInfo cultureInfo, IJsonStringLocalizerFactoryInternal localizerFactory)
        {
            this.stringMap = stringMap;
            this.cultureInfo = cultureInfo;

            this.localizerFactory = localizerFactory ?? throw new ArgumentNullException(nameof(localizerFactory));
        }

        ///<inheritdoc/>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var result = this.stringMap.Select(s => new LocalizedString(s.Key, s.Value));

            if (includeParentCultures && this.cultureInfo.Parent != null && !object.ReferenceEquals(this.cultureInfo, this.cultureInfo.Parent))
            {
                var parentLocalizer = this.localizerFactory.CreateStringLocalizer(this.cultureInfo.Parent);
                if (parentLocalizer != null)
                {
                    result = result.Concat(parentLocalizer.GetAllStrings(true));
                }
            }

            return result;
        }

        ///<inheritdoc/>
        public LocalizedString this[string name]
            => BuildLocalizedString(name, s => s, l => l[name]);

        ///<inheritdoc/>
        public LocalizedString this[string name, params object[] arguments]
            => BuildLocalizedString(name, s => string.Format(this.cultureInfo, s, arguments), l => l[name, arguments]);

        private LocalizedString BuildLocalizedString(string name, Func<string, string> format, Func<IStringLocalizer, LocalizedString> forward)
        {
            if (this.stringMap.TryGetValue(name, out var value))
            {
                return new LocalizedString(name, format(value));
            }
            else if (this.cultureInfo.Parent != null && !object.ReferenceEquals(this.cultureInfo, this.cultureInfo.Parent))
            {
                var parentLocalizer = this.localizerFactory.CreateStringLocalizer(this.cultureInfo.Parent);
                if (parentLocalizer != null)
                {
                    return forward(parentLocalizer);
                }
            }

            return new LocalizedString(name, format(name), true);
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
