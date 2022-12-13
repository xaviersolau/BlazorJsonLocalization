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
        private readonly IHttpCacheService httpCacheService;

        /// <summary>
        /// Setup EmbeddedJsonLocalizationExtensionService with the given logger.
        /// </summary>
        /// <param name="logger">Logger where to log processing messages.</param>
        /// <param name="httpCacheService">Http loading task cache service.</param>
        protected AHttpHostedJsonLocalizationExtensionService(ILogger<AHttpHostedJsonLocalizationExtensionService> logger, IHttpCacheService httpCacheService)
        {
            this.logger = logger;
            this.httpCacheService = httpCacheService;
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

                    this.logger.LoadingStaticAssets(uri);

                    return this.httpCacheService.ProcessLoadingTask(uri, () => TryLoadFromUriAsync(uri, options.JsonSerializerOptions));
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
