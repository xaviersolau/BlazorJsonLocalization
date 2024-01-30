// ----------------------------------------------------------------------
// <copyright file="AHttpHostedJsonLocalizationExtensionService.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SoloX.BlazorJsonLocalization.Helpers;
using SoloX.BlazorJsonLocalization.Helpers.Impl;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

#if !NET6_0_OR_GREATER
using ArgumentNullException = SoloX.BlazorJsonLocalization.Helpers.ArgumentNullExceptionLegacy;
#endif

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
        /// <param name="options">Localizer options.</param>
        /// <param name="logger">Logger where to log processing messages.</param>
        /// <param name="httpCacheService">Http loading task cache service.</param>
        protected AHttpHostedJsonLocalizationExtensionService(IOptions<JsonLocalizationOptions> options, ILogger<AHttpHostedJsonLocalizationExtensionService> logger, IHttpCacheService httpCacheService)
        {
            ArgumentNullException.ThrowIfNull(options, nameof(options));

            this.logger = options.Value.GetLogger(logger);
            this.httpCacheService = httpCacheService;
        }

        ///<inheritdoc/>
        public async ValueTask<IReadOnlyDictionary<string, string>?> TryLoadAsync(
            HttpHostedJsonLocalizationOptions options,
            Assembly assembly,
            string baseName,
            CultureInfo cultureInfo)
        {
            ArgumentNullException.ThrowIfNull(assembly, nameof(assembly));
            ArgumentNullException.ThrowIfNull(options, nameof(options));

            var assemblyName = assembly.GetName().Name;

            var rootPath = (options.ApplicationAssembly == assembly)
                ? string.Empty
                : $"_content/{assemblyName}/";

            if (!string.IsNullOrEmpty(options.ResourcesPath))
            {
                rootPath = $"{rootPath}{options.ResourcesPath}/";
            }

            var basePath = $"{rootPath}{ResourcePathHelper.ComputeBasePath(assembly, baseName, assembly.GetName().Name)}";

            return await CultureInfoHelper.WalkThoughCultureInfoParentsAsync(cultureInfo,
                culture =>
                {
                    var handler = options.NamingPolicy ?? ResourcePathHelper.DefaultHttpHostedJsonNamingPolicy;
                    var uri = handler.Invoke(basePath, culture.Name);

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
