// ----------------------------------------------------------------------
// <copyright file="HttpHostedJsonLocalizationExtensionService.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using SoloX.BlazorJsonLocalization.Services.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Encodings.Web;
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
        /// <param name="httpClient">The Host HttpClient.</param>
        /// <param name="logger">Logger where to log processing messages.</param>
        public HttpHostedJsonLocalizationExtensionService(HttpClient httpClient, ILogger<HttpHostedJsonLocalizationExtensionService> logger)
            : base(logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        ///<inheritdoc/>
        protected override async ValueTask<IReadOnlyDictionary<string, string>?> TryLoadFromUriAsync(Uri uri, JsonSerializerOptions? jsonSerializerOptions)
        {
            try
            {
                this.logger.LogDebug($"Loading localization data from {uri} using HTTP client");

                using var stream = await this.httpClient.GetStreamAsync(uri).ConfigureAwait(false);

                var options = jsonSerializerOptions ?? new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Default,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    WriteIndented = true
                };

                var map = await JsonSerializer.DeserializeAsync<IReadOnlyDictionary<string, string>>(stream, options).ConfigureAwait(false);

                return map ?? throw new FileLoadException("Null resources");
            }
            catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                this.logger.LogWarning(e, $"Http not found data from {uri}");

                return null;
            }
        }
    }
}
