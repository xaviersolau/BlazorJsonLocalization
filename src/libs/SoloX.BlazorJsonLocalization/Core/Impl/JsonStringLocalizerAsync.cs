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
    public class JsonStringLocalizerAsync : IStringLocalizer, IStringLocalizerInternal
    {
        internal const string AsynchronousStringLocalizerGuidKey = nameof(AsynchronousStringLocalizerGuidKey);

        private readonly string stringLocalizerGuid = Guid.NewGuid().ToString();
        private readonly object syncObject = new object();

        private readonly Task<IReadOnlyDictionary<string, string>?> loadingTask;
        private readonly CultureInfo cultureInfo;
        private readonly IJsonStringLocalizerFactoryInternal localizerFactory;

        private bool loaded;
        private IStringLocalizerInternal stringLocalizer;

        /// <summary>
        /// Setup the asynchronous StringLocalizer.
        /// </summary>
        /// <param name="loadingTask">Asynchronous loading task.</param>
        /// <param name="cultureInfo">The associated culture info.</param>
        /// <param name="localizerFactory">Localizer Internal Factory.</param>
        /// <param name="loadingLocalizer">Localizer to use while loading asynchronously.</param>
        public JsonStringLocalizerAsync(Task<IReadOnlyDictionary<string, string>?> loadingTask, CultureInfo cultureInfo, IJsonStringLocalizerFactoryInternal localizerFactory, IStringLocalizerInternal loadingLocalizer)
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
            IsStringLocalizerGuidAndLoading(name)
            ? new LocalizedString(AsynchronousStringLocalizerGuidKey, this.stringLocalizerGuid, false)
            : LoadingForwarder((l) => l.AsStringLocalizer[name]);

        ///<inheritdoc/>
        public LocalizedString this[string name, params object[] arguments] =>
            LoadingForwarder((l) => l.AsStringLocalizer[name, arguments]);

        private bool IsStringLocalizerGuidAndLoading(string name)
        {
            if (AsynchronousStringLocalizerGuidKey == name)
            {
                lock (PendingAsyncLocalizers)
                {
                    if (PendingAsyncLocalizers.ContainsKey(this.stringLocalizerGuid))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        ///<inheritdoc/>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
            LoadingForwarder((l) => l.AsStringLocalizer.GetAllStrings(includeParentCultures));

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
            return IsStringLocalizerGuidAndLoading(name)
                ? new LocalizedString(AsynchronousStringLocalizerGuidKey, this.stringLocalizerGuid, false)
                : LoadingForwarder((l) => l.TryGet(name));
        }

        ///<inheritdoc/>
        public LocalizedString? TryGet(string name, object[] arguments, CultureInfo requestedCultureInfo)
        {
            return LoadingForwarder((l) => l.TryGet(name, arguments, requestedCultureInfo));
        }


        private TData LoadingForwarder<TData>(Func<IStringLocalizerInternal, TData> handler)
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
            var guidFund = true;
            while (guidFund)
            {
                string guid;
                JsonStringLocalizerAsync? stringLocalizer;
                Task loadingTask;

                lock (PendingAsyncLocalizers)
                {
                    var localizedGuid = localizer[AsynchronousStringLocalizerGuidKey];
                    guidFund = !localizedGuid.ResourceNotFound;

                    guid = localizedGuid.Value;
                    if (PendingAsyncLocalizers.TryGetValue(guid, out stringLocalizer))
                    {
                        loadingTask = stringLocalizer.LoadDataAsync(loadParentCulture).AsTask();

                        LoadingTasks.Add(guid, loadingTask);
                        PendingAsyncLocalizers.Remove(guid);
                    }
                    else
                    {
                        if (!LoadingTasks.TryGetValue(guid, out loadingTask))
                        {
                            continue;
                        }

                        if (loadingTask.Status == TaskStatus.RanToCompletion)
                        {
                            LoadingTasks.Remove(guid);
                            continue;
                        }
                    }
                }
                await loadingTask.ConfigureAwait(false);

                lock (PendingAsyncLocalizers)
                {
                    LoadingTasks.Remove(guid);
                }
            }
        }

        private async ValueTask LoadDataAsync(bool loadParentCulture)
        {
            if (loadParentCulture && this.cultureInfo.Parent != null && !object.ReferenceEquals(this.cultureInfo, this.cultureInfo.Parent))
            {
                var parentLocalizer = this.localizerFactory.CreateStringLocalizer(this.cultureInfo.Parent);
                if (parentLocalizer != null)
                {
                    await parentLocalizer.AsStringLocalizer.LoadAsync().ConfigureAwait(false);
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
