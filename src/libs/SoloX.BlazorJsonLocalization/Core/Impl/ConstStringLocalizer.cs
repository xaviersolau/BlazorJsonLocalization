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
using System.Globalization;

namespace SoloX.BlazorJsonLocalization.Core.Impl
{
    /// <summary>
    /// Constant string localizer.
    /// </summary>
    public class ConstStringLocalizer : AStringLocalizerInternal, IStringLocalizer
    {
        private readonly string constValue;
        private readonly bool resourceNotFound;

        /// <summary>
        /// Setup the ConstStringLocalizer with the constant value
        /// </summary>
        /// <param name="resourceSource"></param>
        /// <param name="cultureInfo">The target culture.</param>
        /// <param name="localizerFactory">Localizer Internal Factory.</param>
        /// <param name="constValue">Constant value to use.</param>
        /// <param name="stringLocalizerGuid">String localizer identity to use.</param>
        public ConstStringLocalizer(StringLocalizerResourceSource resourceSource, CultureInfo cultureInfo, IJsonStringLocalizerFactoryInternal localizerFactory, string constValue, bool resourceNotFound, string? stringLocalizerGuid)
            : base(resourceSource, cultureInfo, localizerFactory, stringLocalizerGuid)
        {
            this.constValue = constValue;
            this.resourceNotFound = resourceNotFound;
        }

        ///<inheritdoc/>
        public LocalizedString this[string name]
            => TryGet(name)!;

        ///<inheritdoc/>
        public LocalizedString this[string name, params object[] arguments]
            => TryGet(name, arguments, CultureInfo)!;

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
            return new LocalizedString(name, this.constValue, this.resourceNotFound);
        }

        ///<inheritdoc/>
        public override LocalizedString? TryGet(string name, object[] arguments, CultureInfo requestedCultureInfo)
        {
            return new LocalizedString(name, this.constValue, this.resourceNotFound);
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
