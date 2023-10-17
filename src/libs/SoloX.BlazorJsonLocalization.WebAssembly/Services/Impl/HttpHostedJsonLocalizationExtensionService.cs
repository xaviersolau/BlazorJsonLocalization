// ----------------------------------------------------------------------
// <copyright file="HttpHostedJsonLocalizationExtensionService.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SoloX.BlazorJsonLocalization.Helpers;
using SoloX.BlazorJsonLocalization.Helpers.Impl;
using SoloX.BlazorJsonLocalization.ServerSide;
using SoloX.BlazorJsonLocalization.Services;
using SoloX.BlazorJsonLocalization.Services.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoloX.BlazorJsonLocalization.WebAssembly.Services.Impl
{
    /// <summary>
    /// Http hosted Json Localization extension service.
    /// </summary>
    public class HttpHostedJsonLocalizationExtensionService : AHttpHostedJsonLocalizationExtensionService
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<HttpHostedJsonLocalizationExtensionService> logger;

        /// <summary>
        /// Setup with the HttpClient.
        /// </summary>
        /// <param name="options">Localizer options.</param>
        /// <param name="httpClient">The Host HttpClient.</param>
        /// <param name="logger">Logger where to log processing messages.</param>
        /// <param name="httpCacheService">Http loading task cache service.</param>
        public HttpHostedJsonLocalizationExtensionService(
            IOptions<JsonLocalizationOptions> options,
            HttpClient httpClient,
            ILogger<HttpHostedJsonLocalizationExtensionService> logger,
            IHttpCacheService httpCacheService)
            : base(options, logger, httpCacheService)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.logger = options.Value.GetLogger(logger);
            this.httpClient = httpClient;
        }

        ///<inheritdoc/>
        protected override async Task<IReadOnlyDictionary<string, string>?> TryLoadFromUriAsync(Uri uri, JsonSerializerOptions? jsonSerializerOptions)
        {
            try
            {
                this.logger.LoadingLocalizationDataFromHttpClient(uri);

                using var stream = await this.httpClient.GetStreamAsync(uri).ConfigureAwait(false);

                var map = await JsonHelper
                    .DeserializeAsync(stream, jsonSerializerOptions)
                    .ConfigureAwait(false);

                return map ?? throw new FileLoadException("Null resources");
            }
            catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                this.logger.HttpFileNotFound(uri, e);

                return null;
            }
        }
    }
}
