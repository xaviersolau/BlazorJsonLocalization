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
    public class JsonStringLocalizer : AStringLocalizerInternal, IStringLocalizer
    {
        private readonly IReadOnlyDictionary<string, string> stringMap;

        /// <summary>
        /// Setup with the given string map.
        /// </summary>
        /// <param name="resourceSource"></param>
        /// <param name="stringMap">The string map to use to resolve localization.</param>
        /// <param name="cultureInfo">The associated culture info.</param>
        /// <param name="localizerFactory">Localizer Internal Factory.</param>
        /// <param name="stringLocalizerGuid">String localizer guid if instantiated from JsonStringLocalizerAsync.</param>
        public JsonStringLocalizer(
            StringLocalizerResourceSource resourceSource,
            IReadOnlyDictionary<string, string> stringMap,
            CultureInfo cultureInfo,
            IJsonStringLocalizerFactoryInternal localizerFactory,
            string? stringLocalizerGuid = null)
            : base(resourceSource, cultureInfo, localizerFactory, stringLocalizerGuid)
        {
            this.stringMap = stringMap;
        }

        ///<inheritdoc/>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var result = this.stringMap.Select(s => new LocalizedString(s.Key, s.Value));

            if (includeParentCultures && this.CultureInfo.Parent != null && !object.ReferenceEquals(CultureInfo, CultureInfo.Parent))
            {
                var parentLocalizer = LocalizerFactoryInternal.CreateStringLocalizer(ResourceSource, CultureInfo.Parent);
                if (parentLocalizer != null)
                {
                    result = result.Concat(parentLocalizer.AsStringLocalizer.GetAllStrings(true));
                }
            }

            return result;
        }

        ///<inheritdoc/>
        public LocalizedString this[string name]
            => BuildLocalizedString(l => l.TryGet(name)) ?? DefaultLocalizer.AsStringLocalizer[name];

        ///<inheritdoc/>
        public LocalizedString this[string name, params object[] arguments]
            => BuildLocalizedString(l => l.TryGet(name, arguments, CultureInfo)) ?? DefaultLocalizer.AsStringLocalizer[name, arguments];

        private LocalizedString? BuildLocalizedString(Func<IStringLocalizerInternal, LocalizedString?> forward)
        {
            return LocalizerFactoryInternal.FindThroughStringLocalizerHierarchy(this, CultureInfo, forward);
        }

#if !NET
        ///<inheritdoc/>
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return LocalizerFactoryInternal.CreateStringLocalizer(ResourceSource, culture).AsStringLocalizer;
        }
#endif

        ///<inheritdoc/>
        public override IStringLocalizer AsStringLocalizer => this;

        ///<inheritdoc/>
        protected override LocalizedString? TryGetInternal(string name)
        {
            if (this.stringMap.TryGetValue(name, out var value))
            {
                return new LocalizedString(name, value);
            }

            return null;
        }

        ///<inheritdoc/>
        public override LocalizedString? TryGet(string name, object[] arguments, CultureInfo requestedCultureInfo)
        {
            if (this.stringMap.TryGetValue(name, out var value))
            {
                return new LocalizedString(name, string.Format(requestedCultureInfo, value, arguments));
            }

            return null;
        }
    }
}
