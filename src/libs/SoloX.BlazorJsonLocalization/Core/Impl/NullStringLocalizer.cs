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
    public class NullStringLocalizer : AStringLocalizerInternal, IStringLocalizer
    {
        private readonly bool resourceNotFound;

        /// <summary>
        /// Setup with target culture.
        /// </summary>
        /// <param name="cultureInfo">The target culture.</param>
        /// <param name="localizerFactory">Localizer Internal Factory.</param>
        /// <param name="resourceNotFound">Tells if resourceNotFound property must be set.</param>
        /// <param name="stringLocalizerGuid">String localizer identity to use.</param>
        public NullStringLocalizer(StringLocalizerResourceSource resourceSource, CultureInfo cultureInfo, IJsonStringLocalizerFactoryInternal localizerFactory, bool resourceNotFound, string? stringLocalizerGuid)
            : base(resourceSource, cultureInfo, localizerFactory, stringLocalizerGuid)
        {
            this.resourceNotFound = resourceNotFound;
        }

        ///<inheritdoc/>
        public LocalizedString this[string name]
            => BuildLocalizedString(l => l.TryGet(name)) ?? new LocalizedString(name, name, this.resourceNotFound);

        ///<inheritdoc/>
        public LocalizedString this[string name, params object[] arguments]
            => BuildLocalizedString(l => l.TryGet(name, arguments, CultureInfo)) ?? new LocalizedString(name, string.Format(CultureInfo, name, arguments), this.resourceNotFound);

        private LocalizedString? BuildLocalizedString(Func<IStringLocalizerInternal, LocalizedString?> forward)
        {
            return LocalizerFactoryInternal.FindThroughStringLocalizerHierarchy(this, CultureInfo, forward);
        }

        ///<inheritdoc/>
        public override IStringLocalizer AsStringLocalizer => this;

        ///<inheritdoc/>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return Array.Empty<LocalizedString>();
        }

        ///<inheritdoc/>
        protected override LocalizedString? TryGetInternal(string name)
        {
            return this.resourceNotFound ? null : new LocalizedString(name, name, this.resourceNotFound);
        }

        ///<inheritdoc/>
        public override LocalizedString? TryGet(string name, object[] arguments, CultureInfo requestedCultureInfo)
        {
            return this.resourceNotFound ? null : new LocalizedString(name, string.Format(CultureInfo, name, arguments), this.resourceNotFound);
        }

#if !NET
        ///<inheritdoc/>
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return LocalizerFactoryInternal.CreateStringLocalizer(ResourceSource, culture).AsStringLocalizer;
        }
#endif
    }
}
