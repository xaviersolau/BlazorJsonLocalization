// ----------------------------------------------------------------------
// <copyright file="StringLocalizerProxy.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace SoloX.BlazorJsonLocalization.Core.Impl
{
    /// <summary>
    /// String localizer proxy user to detect current culture info change.
    /// </summary>
    public class StringLocalizerProxy : IStringLocalizer
    {
        private readonly ICultureInfoService cultureInfoService;
        private readonly string baseName;
        private readonly Assembly assembly;
        private readonly Func<string, Assembly, CultureInfo, IStringLocalizer> createHandler;

        private CultureInfo cultureInfo;
        private IStringLocalizer stringLocalizer;

        /// <summary>
        /// Setup the proxy with the factory create handler and the cultureInfoService.
        /// </summary>
        /// <param name="cultureInfoService">Culture info service to get current culture.</param>
        /// <param name="createHandler">Factory create handler.</param>
        /// <param name="assembly">Localization assembly.</param>
        /// <param name="baseName">Localization base name.</param>
        public StringLocalizerProxy(ICultureInfoService cultureInfoService, Func<string, Assembly, CultureInfo, IStringLocalizer> createHandler, Assembly assembly, string baseName)
        {
            if (createHandler == null)
            {
                throw new ArgumentNullException(nameof(createHandler));
            }

            this.cultureInfoService = cultureInfoService;
            this.createHandler = createHandler;

            this.baseName = baseName;
            this.assembly = assembly;

            this.cultureInfo = this.cultureInfoService.CurrentUICulture;

            this.stringLocalizer = createHandler(baseName, assembly, this.cultureInfo);
        }

        ///<inheritdoc/>
        public LocalizedString this[string name] => CurrentStringLocalizer[name];

        ///<inheritdoc/>
        public LocalizedString this[string name, params object[] arguments] => CurrentStringLocalizer[name, arguments];

        ///<inheritdoc/>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return CurrentStringLocalizer.GetAllStrings(includeParentCultures);
        }

        /// <summary>
        /// Get the current associated string localizer.
        /// </summary>
        public IStringLocalizer CurrentStringLocalizer
        {
            get
            {
                if (this.cultureInfo != this.cultureInfoService.CurrentUICulture)
                {
                    // Looks like the current culture has changed so we need to switch the stringLocalizer.
                    this.cultureInfo = this.cultureInfoService.CurrentUICulture;
                    this.stringLocalizer = this.createHandler(this.baseName, this.assembly, this.cultureInfo);
                }

                return this.stringLocalizer;
            }
        }
    }
}
