// ----------------------------------------------------------------------
// <copyright file="StringLocalizerProxy.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SoloX.BlazorJsonLocalization.Services;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SoloX.BlazorJsonLocalization.Core.Impl
{
    /// <summary>
    /// String localizer proxy user to detect current culture info change.
    /// </summary>
    public class StringLocalizerProxy : IStringLocalizer
    {
        private readonly ILogger<StringLocalizerProxy> logger;
        private readonly ICultureInfoService cultureInfoService;
        private readonly IJsonStringLocalizerFactoryInternal localizerFactory;

        private CultureInfo cultureInfo;
        private IStringLocalizer stringLocalizer;

        /// <summary>
        /// Setup the proxy with the factory create handler and the cultureInfoService.
        /// </summary>
        /// <param name="logger">The logger to use in this class.</param>
        /// <param name="cultureInfoService">Culture info service to get current culture.</param>
        /// <param name="localizerFactory">Localizer Internal Factory.</param>
        public StringLocalizerProxy(ILogger<StringLocalizerProxy> logger, ICultureInfoService cultureInfoService, IJsonStringLocalizerFactoryInternal localizerFactory)
        {
            this.logger = logger;

            this.localizerFactory = localizerFactory ?? throw new ArgumentNullException(nameof(localizerFactory));

            this.cultureInfoService = cultureInfoService;

            this.cultureInfo = this.cultureInfoService.CurrentUICulture;

            this.stringLocalizer = localizerFactory.CreateStringLocalizer(this.cultureInfo);
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
                    this.logger.LogDebug("Current UI culture changed: from {0} to {1}.",
                        this.cultureInfo,
                        this.cultureInfoService.CurrentUICulture
                        );
                    // Looks like the current culture has changed so we need to switch the stringLocalizer.
                    this.cultureInfo = this.cultureInfoService.CurrentUICulture;
                    this.stringLocalizer = this.localizerFactory.CreateStringLocalizer(this.cultureInfo);
                }

                return this.stringLocalizer;
            }
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
