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

        /// <summary>
        /// Setup with the given string map.
        /// </summary>
        /// <param name="stringMap">The string map to use to resolve localization.</param>
        /// <param name="cultureInfo">The associated culture info.</param>
        public JsonStringLocalizer(IReadOnlyDictionary<string, string> stringMap, CultureInfo cultureInfo)
        {
            this.stringMap = stringMap;
            this.cultureInfo = cultureInfo;
        }

        ///<inheritdoc/>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return this.stringMap.Select(s => new LocalizedString(s.Key, s.Value));
        }

        ///<inheritdoc/>
        public LocalizedString this[string name]
            => BuildLocalizedString(name, s => s);

        ///<inheritdoc/>
        public LocalizedString this[string name, params object[] arguments]
            => BuildLocalizedString(name, s => string.Format(this.cultureInfo, s, arguments));

        private LocalizedString BuildLocalizedString(string name, Func<string, string> format)
        {
            if (this.stringMap.TryGetValue(name, out var value))
            {
                return new(name, format(value));
            }
            else
            {
                return new(name, format(name), true);
            }
        }
    }
}
