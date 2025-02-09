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
using System.Threading.Tasks;

namespace SoloX.BlazorJsonLocalization.Core.Impl
{

    /// <summary>
    /// String localizer proxy user to detect current culture info change.
    /// </summary>
    public class StringLocalizerProxy : IStringLocalizer, IStringLocalizerInternal
    {
        private readonly ILogger<StringLocalizerProxy> logger;
        private readonly ICultureInfoService cultureInfoService;

        private readonly IJsonStringLocalizerFactoryInternal localizerFactory;

        private CultureInfo cultureInfo;
        private IStringLocalizerInternal stringLocalizer;

        /// <summary>
        /// Setup the proxy with the factory create handler and the cultureInfoService.
        /// </summary>
        /// <param name="resourceSource"></param>
        /// <param name="logger">The logger to use in this class.</param>
        /// <param name="cultureInfoService">Culture info service to get current culture.</param>
        /// <param name="localizerFactory">Localizer Internal Factory.</param>
        public StringLocalizerProxy(StringLocalizerResourceSource resourceSource, ILogger<StringLocalizerProxy> logger, ICultureInfoService cultureInfoService, IJsonStringLocalizerFactoryInternal localizerFactory)
        {
            ResourceSource = resourceSource;

            this.logger = logger;

            this.localizerFactory = localizerFactory ?? throw new ArgumentNullException(nameof(localizerFactory));

            this.cultureInfoService = cultureInfoService;
            this.cultureInfo = this.cultureInfoService.CurrentUICulture;

            this.stringLocalizer = localizerFactory.CreateStringLocalizer(ResourceSource, this.cultureInfo);
        }

        /// <inheritdoc/>
        public StringLocalizerResourceSource ResourceSource { get; }

        ///<inheritdoc/>
        public LocalizedString this[string name]
            => BuildLocalizedString(l => l.TryGet(name)) ?? CurrentStringLocalizer.AsStringLocalizer[name];

        ///<inheritdoc/>
        public LocalizedString this[string name, params object[] arguments]
            => BuildLocalizedString(l => l.TryGet(name, arguments, CultureInfo)) ?? CurrentStringLocalizer.AsStringLocalizer[name, arguments];

        private LocalizedString? BuildLocalizedString(Func<IStringLocalizerInternal, LocalizedString?> forward)
        {
            return LocalizerFactoryInternal.FindThroughStringLocalizerHierarchy(CurrentStringLocalizer, CultureInfo, forward);
        }

        ///<inheritdoc/>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return CurrentStringLocalizer.AsStringLocalizer.GetAllStrings(includeParentCultures);
        }

        ///<inheritdoc/>
        public IJsonStringLocalizerFactoryInternal LocalizerFactoryInternal => this.localizerFactory;

        ///<inheritdoc/>
        public CultureInfo CultureInfo => this.cultureInfo;

        ///<inheritdoc/>
        public Task<bool> LoadDataAsync()
        {
            return CurrentStringLocalizer.LoadDataAsync();
        }

        /// <summary>
        /// Get the current associated string localizer.
        /// </summary>
        public IStringLocalizerInternal CurrentStringLocalizer
        {
            get
            {
                var targetCultureInfo = this.cultureInfoService.CurrentUICulture;

                if (this.cultureInfo != targetCultureInfo)
                {
                    this.logger.SwitchCurrentCulture(
                        this.cultureInfo,
                        targetCultureInfo);

                    // Looks like the current culture has changed so we need to switch the stringLocalizer.
                    this.cultureInfo = targetCultureInfo;
                    this.stringLocalizer = this.localizerFactory.CreateStringLocalizer(ResourceSource, this.cultureInfo);
                }

                return this.stringLocalizer;
            }
        }

#if !NET
        ///<inheritdoc/>
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return this.localizerFactory.CreateStringLocalizer(ResourceSource, culture).AsStringLocalizer;
        }
#endif

        ///<inheritdoc/>
        public IStringLocalizer AsStringLocalizer => this;

        ///<inheritdoc/>
        public LocalizedString? TryGet(string name)
        {
            return CurrentStringLocalizer.TryGet(name);
        }

        ///<inheritdoc/>
        public LocalizedString? TryGet(string name, object[] arguments, CultureInfo requestedCultureInfo)
        {
            return CurrentStringLocalizer.TryGet(name, arguments, requestedCultureInfo);
        }
    }
}
