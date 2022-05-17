// ----------------------------------------------------------------------
// <copyright file="AHttpHostedJsonLocalizationExtensionService.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using SoloX.BlazorJsonLocalization.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoloX.BlazorJsonLocalization.Services.Impl
{
    /// <summary>
    /// Abstract HttpHostedJsonLocalizationExtensionService implementation.
    /// </summary>
    public abstract class AHttpHostedJsonLocalizationExtensionService
        : IJsonLocalizationExtensionService<HttpHostedJsonLocalizationOptions>
    {
        private readonly ILogger<AHttpHostedJsonLocalizationExtensionService> logger;

        /// <summary>
        /// Cache to avoid Json file reload.
        /// </summary>
        private static readonly IDictionary<Uri, Task<IReadOnlyDictionary<string, string>?>> Cache = new Dictionary<Uri, Task<IReadOnlyDictionary<string, string>?>>();

        /// <summary>
        /// Setup EmbeddedJsonLocalizationExtensionService with the given logger.
        /// </summary>
        /// <param name="logger">Logger where to log processing messages.</param>
        protected AHttpHostedJsonLocalizationExtensionService(ILogger<AHttpHostedJsonLocalizationExtensionService> logger)
        {
            this.logger = logger;
        }

        ///<inheritdoc/>
        public async ValueTask<IReadOnlyDictionary<string, string>?> TryLoadAsync(
            HttpHostedJsonLocalizationOptions options,
            Assembly assembly,
            string baseName,
            CultureInfo cultureInfo)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var assemblyName = assembly.GetName().Name;

            var rootPath = (options.ApplicationAssembly == assembly)
                ? string.Empty
                : $"_content/{assemblyName}/";

            if (!string.IsNullOrEmpty(options.ResourcesPath))
            {
                rootPath = $"{rootPath}{options.ResourcesPath}/";
            }

            var basePath = $"{rootPath}{ResourcePathHelper.ComputeBasePath(assembly, baseName)}";

            return await CultureInfoHelper.WalkThoughCultureInfoParentsAsync(cultureInfo,
                cultureName =>
                {
                    var handler = options.NamingPolicy ?? ResourcePathHelper.DefaultHttpHostedJsonNamingPolicy;
                    var uri = handler.Invoke(basePath, cultureName);

                    this.logger.LogDebug($"Loading static assets data from {uri}");

                    lock (Cache)
                    {
                        if (!Cache.TryGetValue(uri, out var loadingTask))
                        {
                            loadingTask = TryLoadFromUriAsync(uri, options.JsonSerializerOptions);
                            Cache.Add(uri, loadingTask);
                        }
                        else if (loadingTask.IsFaulted)
                        {
                            loadingTask = TryLoadFromUriAsync(uri, options.JsonSerializerOptions);
                            Cache[uri] = loadingTask;
                        }
                        return loadingTask;
                    }
                })
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Load Http resources.
        /// </summary>
        /// <param name="uri">Resources Uri location.</param>
        /// <param name="jsonSerializerOptions">Custom JSON serializer options.</param>
        /// <returns>The loaded Json map.</returns>
        protected abstract Task<IReadOnlyDictionary<string, string>?> TryLoadFromUriAsync(Uri uri, JsonSerializerOptions? jsonSerializerOptions);
    }
}
