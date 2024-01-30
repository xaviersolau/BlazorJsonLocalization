﻿// ----------------------------------------------------------------------
// <copyright file="HttpCacheService.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#if !NET6_0_OR_GREATER
using ArgumentNullException = SoloX.BlazorJsonLocalization.Helpers.ArgumentNullExceptionLegacy;
#endif

namespace SoloX.BlazorJsonLocalization.Services.Impl
{
    /// <summary>
    /// IHttpCacheService implementation to prevent loading several times the same resources
    /// </summary>
    public class HttpCacheService : IHttpCacheService
    {
        /// <summary>
        /// Cache to avoid Json file reload.
        /// </summary>
        private readonly Dictionary<Uri, Task<IReadOnlyDictionary<string, string>?>> cache = new Dictionary<Uri, Task<IReadOnlyDictionary<string, string>?>>();

        /// <inheritdoc/>
        public Task<IReadOnlyDictionary<string, string>?> ProcessLoadingTask(Uri uri, Func<Task<IReadOnlyDictionary<string, string>?>> loadingHandler)
        {
            ArgumentNullException.ThrowIfNull(loadingHandler, nameof(loadingHandler));

            lock (this.cache)
            {
                if (!this.cache.TryGetValue(uri, out var loadingTask))
                {
                    loadingTask = loadingHandler();
                    this.cache.Add(uri, loadingTask);
                }
                else if (loadingTask.IsFaulted)
                {
                    loadingTask = loadingHandler();
                    this.cache[uri] = loadingTask;
                }
                return loadingTask;
            }
        }
    }
}
