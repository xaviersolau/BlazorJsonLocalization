// ----------------------------------------------------------------------
// <copyright file="HttpHostedJsonLocalizationExtensionService.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

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

        /// <summary>
        /// Setup with the HttpClient.
        /// </summary>
        /// <param name="httpClient">The Host HttpClient.</param>
        public HttpHostedJsonLocalizationExtensionService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        ///<inheritdoc/>
        protected override async ValueTask<IReadOnlyDictionary<string, string>?> TryLoadFromUriAsync(Uri uri)
        {
            try
            {
                using var stream = await this.httpClient.GetStreamAsync(uri).ConfigureAwait(false);

                var map = await JsonSerializer.DeserializeAsync<IReadOnlyDictionary<string, string>>(stream).ConfigureAwait(false);

                if (map == null)
                {
                    throw new FileLoadException("Null resources");
                }

                return map;
            }
            catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }
    }
}
