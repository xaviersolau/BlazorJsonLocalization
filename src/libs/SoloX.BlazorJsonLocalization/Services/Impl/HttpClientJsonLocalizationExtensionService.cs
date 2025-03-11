// ----------------------------------------------------------------------
// <copyright file="HttpClientJsonLocalizationExtensionService.cs" company="Xavier Solau">
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
using System.IO;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;


#if !NET6_0_OR_GREATER
using ArgumentNullException = SoloX.BlazorJsonLocalization.Helpers.ArgumentNullExceptionLegacy;
#endif

namespace SoloX.BlazorJsonLocalization.Services.Impl
{
    /// <summary>
    /// Http client Json Localization extension service.
    /// </summary>
    public class HttpClientJsonLocalizationExtensionService : AHttpHostedJsonLocalizationExtensionService<HttpClientJsonLocalizationOptions>
    {
        private readonly ILogger<HttpClientJsonLocalizationExtensionService> logger;
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Setup with the HttpClient.
        /// </summary>
        /// <param name="options">Localizer options.</param>
        /// <param name="serviceProvider">Service provider to get the HttpClient.</param>
        /// <param name="logger">Logger where to log processing messages.</param>
        /// <param name="httpCacheService">Http loading task cache service.</param>
        public HttpClientJsonLocalizationExtensionService(
            IOptions<JsonLocalizationOptions> options,
            IServiceProvider serviceProvider,
            ILogger<HttpClientJsonLocalizationExtensionService> logger,
            IHttpCacheService httpCacheService)
            : base(options, logger, httpCacheService)
        {
            ArgumentNullException.ThrowIfNull(options, nameof(options));

#pragma warning disable CA1062 // Validate arguments of public methods
            this.logger = options.Value.GetLogger(logger);
            this.serviceProvider = serviceProvider;
#pragma warning restore CA1062 // Validate arguments of public methods
        }

        ///<inheritdoc/>
        protected override async Task<IReadOnlyDictionary<string, string>?> TryLoadFromUriAsync(Uri uri, HttpClientJsonLocalizationOptions options)
        {
            ArgumentNullException.ThrowIfNull(options, nameof(options));

            var jsonSerializerOptions = options.JsonSerializerOptions;

            this.logger.LoadingLocalizationDataFromHttpClient(uri);

            using var response = await GetHttpResponseAsync(uri, options).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                this.logger.HttpTargetNotFound(uri);

                return null;
            }

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            var map = await JsonHelper
                .DeserializeAsync(stream, jsonSerializerOptions)
                .ConfigureAwait(false);

            return map ?? throw new FileLoadException("Null resources");
        }

        private async Task<HttpResponseMessage> GetHttpResponseAsync(Uri uri, HttpClientJsonLocalizationOptions options)
        {
            if (options.HttpClientBuilder == null)
            {
                var httpClient = this.serviceProvider.GetRequiredService<HttpClient>();

                return await httpClient.GetAsync(uri).ConfigureAwait(false);
            }
            else if (options.IsDisposeHttpClientFromBuilderEnabled)
            {
                using var httpClient = options.HttpClientBuilder(this.serviceProvider);

                return await httpClient.GetAsync(uri).ConfigureAwait(false);
            }
            else
            {
                var httpClient = options.HttpClientBuilder(this.serviceProvider);

                return await httpClient.GetAsync(uri).ConfigureAwait(false);
            }
        }
    }
}
