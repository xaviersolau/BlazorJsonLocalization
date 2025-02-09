// ----------------------------------------------------------------------
// <copyright file="JsonStringLocalizerAsync.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace SoloX.BlazorJsonLocalization.Core.Impl
{
    /// <summary>
    /// IStringLocalizer asynchronous implementation.
    /// </summary>
    public class JsonStringLocalizerAsync : AStringLocalizerInternal, IStringLocalizer
    {
        private readonly object syncObject = new object();

        private readonly Task<IReadOnlyDictionary<string, string>?> loadingTask;

        private bool loaded;

        private JsonStringLocalizer? stringLocalizer;

        /// <summary>
        /// Setup the asynchronous StringLocalizer.
        /// </summary>
        /// <param name="resourceSource">Resource source data.</param>
        /// <param name="loadingTask">Asynchronous loading task.</param>
        /// <param name="cultureInfo">The associated culture info.</param>
        /// <param name="localizerFactory">Localizer Internal Factory.</param>
        public JsonStringLocalizerAsync(
            StringLocalizerResourceSource resourceSource,
            Task<IReadOnlyDictionary<string, string>?> loadingTask,
            CultureInfo cultureInfo,
            IJsonStringLocalizerFactoryInternal localizerFactory)
            : base(resourceSource, cultureInfo, localizerFactory, null)
        {
            this.loadingTask = loadingTask;
            this.loaded = false;

            if (this.loadingTask.Status == TaskStatus.RanToCompletion)
            {
                var map = this.loadingTask.Result;

                if (map != null)
                {
                    this.stringLocalizer = new JsonStringLocalizer(resourceSource, map, CultureInfo, LocalizerFactoryInternal, StringLocalizerGuid);
                }

                this.loaded = true;
            }
        }

        /// <summary>
        /// Tells if the localization resources are loaded.
        /// </summary>
        public bool IsLoaded => this.loaded;

        ///<inheritdoc/>
        public LocalizedString this[string name]
            => BuildLocalizedString(l => l.TryGet(name)) ?? DefaultLocalizer.AsStringLocalizer[name];

        ///<inheritdoc/>
        public LocalizedString this[string name, params object[] arguments]
            => BuildLocalizedString(l => l.TryGet(name, arguments, CultureInfo)) ?? DefaultLocalizer.AsStringLocalizer[name, arguments];

        private LocalizedString? BuildLocalizedString(Func<IStringLocalizerInternal, LocalizedString?> forward)
        {
            TryCompleteLoad();

            if (this.stringLocalizer == null)
            {
                return null;
            }

            return LocalizerFactoryInternal.FindThroughStringLocalizerHierarchy(this.stringLocalizer, CultureInfo, forward);
        }

        ///<inheritdoc/>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
            this.stringLocalizer?.AsStringLocalizer.GetAllStrings(includeParentCultures) ?? [];

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
            TryCompleteLoad();

            return this.stringLocalizer?.TryGet(name);
        }

        ///<inheritdoc/>
        public override LocalizedString? TryGet(string name, object[] arguments, CultureInfo requestedCultureInfo)
        {
            TryCompleteLoad();

            return this.stringLocalizer?.TryGet(name, arguments, requestedCultureInfo);
        }

        ///<inheritdoc/>
        public override async Task<bool> LoadDataAsync()
        {
            var map = await this.loadingTask.ConfigureAwait(false);

            TryCompleteLoad();

            return map != null;
        }

        private void TryCompleteLoad()
        {
            lock (this.syncObject)
            {
                if (!this.loaded)
                {
                    var completed = this.loadingTask.Status == TaskStatus.RanToCompletion;

                    if (completed)
                    {
                        var map = this.loadingTask.Result;

                        if (map != null)
                        {
                            this.stringLocalizer = new JsonStringLocalizer(ResourceSource, map, CultureInfo, LocalizerFactoryInternal, StringLocalizerGuid);
                        }

                        this.loaded = true;
                    }
                }
            }
        }
    }
}
