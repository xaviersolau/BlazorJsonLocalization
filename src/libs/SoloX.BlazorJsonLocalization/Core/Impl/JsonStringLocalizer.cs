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
    public class JsonStringLocalizer : IStringLocalizer, IStringLocalizerInternal
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
                    result = result.Concat(parentLocalizer.AsStringLocalizer.GetAllStrings(true));
                }
            }

            return result;
        }

        ///<inheritdoc/>
        public LocalizedString this[string name]
            => BuildLocalizedString(l => l.TryGet(name)) ?? new LocalizedString(name, name, true);

        ///<inheritdoc/>
        public LocalizedString this[string name, params object[] arguments]
            => BuildLocalizedString(l => l.TryGet(name, arguments, this.cultureInfo)) ?? new LocalizedString(name, string.Format(this.cultureInfo, name, arguments), true);

        private LocalizedString? BuildLocalizedString(Func<IStringLocalizerInternal, LocalizedString?> forward)
        {
            var localizedString = forward(this);

            if (localizedString != null)
            {
                return localizedString;
            }

            return this.localizerFactory.ProcessThroughStringLocalizerHierarchy(this.cultureInfo, forward);
        }

#if !NET
        ///<inheritdoc/>
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return this.localizerFactory.CreateStringLocalizer(culture).AsStringLocalizer;
        }
#endif

        ///<inheritdoc/>
        public IStringLocalizer AsStringLocalizer => this;

        ///<inheritdoc/>
        public LocalizedString? TryGet(string name)
        {
            if (this.stringMap.TryGetValue(name, out var value))
            {
                return new LocalizedString(name, value);
            }
            else if (this.cultureInfo.Parent != null
                && !object.ReferenceEquals(this.cultureInfo, this.cultureInfo.Parent)
                && JsonStringLocalizerAsync.AsynchronousStringLocalizerGuidKey != name)
            {
                var parentLocalizer = this.localizerFactory.CreateStringLocalizer(this.cultureInfo.Parent);
                return parentLocalizer.TryGet(name);
            }

            return null;
        }

        ///<inheritdoc/>
        public LocalizedString? TryGet(string name, object[] arguments, CultureInfo requestedCultureInfo)
        {
            if (this.stringMap.TryGetValue(name, out var value))
            {
                return new LocalizedString(name, string.Format(requestedCultureInfo, value, arguments));
            }
            else if (this.cultureInfo.Parent != null
                && !object.ReferenceEquals(this.cultureInfo, this.cultureInfo.Parent)
                && JsonStringLocalizerAsync.AsynchronousStringLocalizerGuidKey != name)
            {
                var parentLocalizer = this.localizerFactory.CreateStringLocalizer(this.cultureInfo.Parent);
                return parentLocalizer.TryGet(name, arguments, requestedCultureInfo);
            }

            return null;
        }
    }
}
