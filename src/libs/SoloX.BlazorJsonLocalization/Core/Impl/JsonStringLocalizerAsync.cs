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

        private bool loaded;
        private IStringLocalizer stringLocalizer;

        /// <summary>
        /// Setup the asynchronous StringLocalizer.
        /// </summary>
        /// <param name="loadingTask">Asynchronous loading task.</param>
        /// <param name="cultureInfo">The associated culture info.</param>
        public JsonStringLocalizerAsync(Task<IReadOnlyDictionary<string, string>?> loadingTask, CultureInfo cultureInfo)
        {
            this.loadingTask = loadingTask;
            this.cultureInfo = cultureInfo;
            this.loaded = false;

            if (this.loadingTask.Status == TaskStatus.RanToCompletion)
            {
                var map = this.loadingTask.Result;

                if (map != null)
                {
                    this.stringLocalizer = new JsonStringLocalizer(map, this.cultureInfo);
                }
                else
                {
                    this.stringLocalizer = new NullStringLocalizer(cultureInfo);
                }

                this.loaded = true;
            }
            else
            {
                this.stringLocalizer = new ConstStringLocalizer("...");

                lock (LoadingTasks)
                {
                    LoadingTasks.Add(this.stringLocalizerGuid, this);
                }
            }
        }

        ///<inheritdoc/>
        public LocalizedString this[string name] =>
            name == nameof(this.stringLocalizerGuid)
            ? new(nameof(this.stringLocalizerGuid), this.stringLocalizerGuid, false)
            : LoadingForwarder((l) => l[name]);

        ///<inheritdoc/>
        public LocalizedString this[string name, params object[] arguments] =>
            name == nameof(this.stringLocalizerGuid)
            ? new(nameof(this.stringLocalizerGuid), this.stringLocalizerGuid, false)
            : LoadingForwarder((l) => l[name, arguments]);

        ///<inheritdoc/>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
            LoadingForwarder((l) => l.GetAllStrings(includeParentCultures));

        private TData LoadingForwarder<TData>(Func<IStringLocalizer, TData> handler)
        {
            lock (this.syncObject)
            {
                if (!this.loaded)
                {
                    var completed = false;
                    lock (LoadingTasks)
                    {
                        if (this.loadingTask.Status == TaskStatus.RanToCompletion && LoadingTasks.ContainsKey(this.stringLocalizerGuid))
                        {
                            LoadingTasks.Remove(this.stringLocalizerGuid);
                            completed = true;
                        }
                    }

                    if (completed)
                    {
                        var map = this.loadingTask.Result;

                        if (map != null)
                        {
                            this.stringLocalizer = new JsonStringLocalizer(map, this.cultureInfo);
                        }

                        this.loaded = true;
                    }
                }

                return handler(this.stringLocalizer);
            }
        }

        private static readonly IDictionary<string, JsonStringLocalizerAsync> LoadingTasks =
            new Dictionary<string, JsonStringLocalizerAsync>();

        internal static ValueTask LoadAsync(IStringLocalizer localizer)
        {
            JsonStringLocalizerAsync? stringLocalizerAsync;
            lock (LoadingTasks)
            {
                var guid = localizer[nameof(stringLocalizerGuid)].Value;
                if (!LoadingTasks.TryGetValue(guid, out stringLocalizerAsync))
                {
                    return ValueTask.CompletedTask;
                }
                LoadingTasks.Remove(guid);
            }
            return stringLocalizerAsync.LoadDataAsync();
        }

        private async ValueTask LoadDataAsync()
        {
            var map = await this.loadingTask.ConfigureAwait(false);

            lock (this.syncObject)
            {
                if (map != null)
                {
                    this.stringLocalizer = new JsonStringLocalizer(map, this.cultureInfo);
                }

                this.loaded = true;
            }
        }
    }
}
