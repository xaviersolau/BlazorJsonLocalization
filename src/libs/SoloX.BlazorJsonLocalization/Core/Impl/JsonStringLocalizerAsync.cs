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
    public class JsonStringLocalizerAsync : IStringLocalizer
    {
        private readonly string stringLocalizerGuid = Guid.NewGuid().ToString();
        private readonly object syncObject = new object();

        private readonly Task<IReadOnlyDictionary<string, string>?> loadingTask;
        private readonly CultureInfo cultureInfo;
        private readonly IJsonStringLocalizerFactoryInternal localizerFactory;

        private bool loaded;
        private IStringLocalizer stringLocalizer;

        /// <summary>
        /// Setup the asynchronous StringLocalizer.
        /// </summary>
        /// <param name="loadingTask">Asynchronous loading task.</param>
        /// <param name="cultureInfo">The associated culture info.</param>
        /// <param name="localizerFactory">Localizer Internal Factory.</param>
        /// <param name="loadingLocalizer">Localizer to use while loading asynchronously.</param>
        public JsonStringLocalizerAsync(Task<IReadOnlyDictionary<string, string>?> loadingTask, CultureInfo cultureInfo, IJsonStringLocalizerFactoryInternal localizerFactory, IStringLocalizer loadingLocalizer)
        {
            this.loadingTask = loadingTask;
            this.cultureInfo = cultureInfo;
            this.loaded = false;

            this.localizerFactory = localizerFactory ?? throw new ArgumentNullException(nameof(localizerFactory));

            if (this.loadingTask.Status == TaskStatus.RanToCompletion)
            {
                var map = this.loadingTask.Result;

                if (map != null)
                {
                    this.stringLocalizer = new JsonStringLocalizer(map, this.cultureInfo, this.localizerFactory);
                }
                else
                {
                    this.stringLocalizer = new NullStringLocalizer(cultureInfo, this.localizerFactory, true);
                }

                this.loaded = true;
            }
            else
            {
                this.stringLocalizer = loadingLocalizer;

                lock (PendingAsyncLocalizers)
                {
                    PendingAsyncLocalizers.Add(this.stringLocalizerGuid, this);
                }
            }
        }

        ///<inheritdoc/>
        public LocalizedString this[string name] =>
            name == nameof(this.stringLocalizerGuid)
            ? new LocalizedString(nameof(this.stringLocalizerGuid), this.stringLocalizerGuid, false)
            : LoadingForwarder((l) => l[name]);

        ///<inheritdoc/>
        public LocalizedString this[string name, params object[] arguments] =>
            name == nameof(this.stringLocalizerGuid)
            ? new LocalizedString(nameof(this.stringLocalizerGuid), this.stringLocalizerGuid, false)
            : LoadingForwarder((l) => l[name, arguments]);

        ///<inheritdoc/>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
            LoadingForwarder((l) => l.GetAllStrings(includeParentCultures));

#if !NET
        ///<inheritdoc/>
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return this.localizerFactory.CreateStringLocalizer(culture);
        }
#endif

        private TData LoadingForwarder<TData>(Func<IStringLocalizer, TData> handler)
        {
            lock (this.syncObject)
            {
                if (!this.loaded)
                {
                    var completed = false;
                    lock (PendingAsyncLocalizers)
                    {
                        if (this.loadingTask.Status == TaskStatus.RanToCompletion && PendingAsyncLocalizers.ContainsKey(this.stringLocalizerGuid))
                        {
                            PendingAsyncLocalizers.Remove(this.stringLocalizerGuid);
                            completed = true;
                        }
                    }

                    if (completed)
                    {
                        var map = this.loadingTask.Result;

                        if (map != null)
                        {
                            this.stringLocalizer = new JsonStringLocalizer(map, this.cultureInfo, this.localizerFactory);
                        }

                        this.loaded = true;
                    }
                }

                return handler(this.stringLocalizer);
            }
        }

        private static readonly IDictionary<string, JsonStringLocalizerAsync> PendingAsyncLocalizers =
            new Dictionary<string, JsonStringLocalizerAsync>();

        private static readonly IDictionary<string, Task> LoadingTasks =
            new Dictionary<string, Task>();

        internal static async ValueTask LoadAsync(IStringLocalizer localizer, bool loadParentCulture)
        {
            JsonStringLocalizerAsync? stringLocalizer;

            Task loadingTask;

            lock (PendingAsyncLocalizers)
            {
                var guid = localizer[nameof(stringLocalizerGuid)].Value;
                if (!PendingAsyncLocalizers.TryGetValue(guid, out stringLocalizer))
                {
                    if (!LoadingTasks.TryGetValue(guid, out loadingTask))
                    {
                        return;
                    }

                    if (loadingTask.Status == TaskStatus.RanToCompletion)
                    {
                        LoadingTasks.Remove(guid);
                    }
                }
                else
                {
                    loadingTask = stringLocalizer.LoadDataAsync(loadParentCulture).AsTask();

                    LoadingTasks.Add(guid, loadingTask);
                    PendingAsyncLocalizers.Remove(guid);
                }
            }
            await loadingTask.ConfigureAwait(false);
        }

        private async ValueTask LoadDataAsync(bool loadParentCulture)
        {
            if (loadParentCulture && this.cultureInfo.Parent != null && !object.ReferenceEquals(this.cultureInfo, this.cultureInfo.Parent))
            {
                var parentLocalizer = this.localizerFactory.CreateStringLocalizer(this.cultureInfo.Parent);
                if (parentLocalizer != null)
                {
                    await parentLocalizer.LoadAsync().ConfigureAwait(false);
                }
            }

            var map = await this.loadingTask.ConfigureAwait(false);

            lock (this.syncObject)
            {
                if (map != null)
                {
                    this.stringLocalizer = new JsonStringLocalizer(map, this.cultureInfo, this.localizerFactory);
                }

                this.loaded = true;
            }
        }
    }
}
