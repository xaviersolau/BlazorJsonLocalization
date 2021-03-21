// ----------------------------------------------------------------------
// <copyright file="HttpHostedJsonLocalizationExtensionService.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoloX.BlazorJsonLocalization.Services.Impl
{
    /// <summary>
    /// Http hosted Json Localization extension service.
    /// </summary>
    public class HttpHostedJsonLocalizationExtensionService : IJsonLocalizationExtensionService<HttpHostedJsonLocalizationOptions>
    {
        private readonly HttpClient httpClient;

        /// <summary>
        /// Setup with the HttpClient.
        /// </summary>
        /// <param name="httpClient"></param>
        public HttpHostedJsonLocalizationExtensionService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
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
            if (cultureInfo == null)
            {
                throw new ArgumentNullException(nameof(cultureInfo));
            }

            var cultureName = cultureInfo.TwoLetterISOLanguageName;

            var path = (options.ApplicationAssembly == assembly)
                ? string.Empty
                : $"_content/{assembly.GetName().Name}/";

            if (!string.IsNullOrEmpty(options.ResourcesPath))
            {
                path = $"{path}{options.ResourcesPath}/";
            }

            var uri = new Uri($"{path}{baseName}-{cultureName}.json", UriKind.Relative);

            var map = await TryLoadFromUriAsync(uri).ConfigureAwait(false);

            if (map == null)
            {
                uri = new Uri($"{path}{baseName}.json", UriKind.Relative);

                map = await TryLoadFromUriAsync(uri).ConfigureAwait(false);
            }

            return map;
        }

        private async ValueTask<Dictionary<string, string>?> TryLoadFromUriAsync(Uri uri)
        {
            try
            {
                using var stream = await this.httpClient.GetStreamAsync(uri).ConfigureAwait(false);

                var map = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(stream).ConfigureAwait(false);

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
