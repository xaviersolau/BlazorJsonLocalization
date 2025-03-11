// ----------------------------------------------------------------------
// <copyright file="HttpHostedJsonLocalizationExtensionService.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using SoloX.BlazorJsonLocalization.Helpers;
using SoloX.BlazorJsonLocalization.Helpers.Impl;
using SoloX.BlazorJsonLocalization.Services;
using SoloX.BlazorJsonLocalization.Services.Impl;
using Microsoft.Extensions.Options;

#if !NET6_0_OR_GREATER
using ArgumentNullException = SoloX.BlazorJsonLocalization.Helpers.ArgumentNullExceptionLegacy;
#endif

namespace SoloX.BlazorJsonLocalization.ServerSide.Services.Impl
{
    /// <summary>
    /// Http hosted Json Localization extension service.
    /// </summary>
    public class HttpHostedJsonLocalizationExtensionService : AHttpHostedJsonLocalizationExtensionService<HttpHostedJsonLocalizationOptions>
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ILogger<HttpHostedJsonLocalizationExtensionService> logger;

        /// <summary>
        /// Setup with the IWebHostEnvironment.
        /// </summary>
        /// <param name="options">Localizer options.</param>
        /// <param name="webHostEnvironment">The IWebHostEnvironment.</param>
        /// <param name="logger">Logger where to log processing messages.</param>
        /// <param name="httpCacheService">Http loading task cache service.</param>
        public HttpHostedJsonLocalizationExtensionService(
            IOptions<JsonLocalizationOptions> options,
            IWebHostEnvironment webHostEnvironment,
            ILogger<HttpHostedJsonLocalizationExtensionService> logger,
            IHttpCacheService httpCacheService)
            : base(options, logger, httpCacheService)
        {
            ArgumentNullException.ThrowIfNull(options, nameof(options));

#pragma warning disable CA1062 // Validate arguments of public methods
            this.logger = options.Value.GetLogger(logger);
#pragma warning restore CA1062 // Validate arguments of public methods

            this.webHostEnvironment = webHostEnvironment;
        }

        ///<inheritdoc/>
        protected override async Task<IReadOnlyDictionary<string, string>?> TryLoadFromUriAsync(Uri uri, HttpHostedJsonLocalizationOptions options)
        {
            ArgumentNullException.ThrowIfNull(uri, nameof(uri));
            ArgumentNullException.ThrowIfNull(options, nameof(options));

            this.logger.LoadingLocalizationDataFromHost(uri);

#pragma warning disable CA1062 // Validate arguments of public methods
            var jsonSerializerOptions = options.JsonSerializerOptions;

            var filePath = uri.OriginalString;
#pragma warning restore CA1062 // Validate arguments of public methods

            // Since we actually load the resource files from the WebRootFileProvider we can remove the query part of the URI.
            var queryIndex = filePath.IndexOf('?', StringComparison.InvariantCulture);

            if (queryIndex >= 0)
            {
                filePath = filePath.Remove(queryIndex);
            }

            var fileInfo = this.webHostEnvironment.WebRootFileProvider.GetFileInfo(filePath);

            if (!fileInfo.Exists)
            {
                this.logger.FileDoesNotExist(uri);
                return null;
            }

            this.logger.LoadingFile(uri);

            using var stream = fileInfo.CreateReadStream();

            var map = await JsonHelper
                .DeserializeAsync(stream, jsonSerializerOptions)
                .ConfigureAwait(false);

            return map ?? throw new FileLoadException("Null resources");
        }
    }
}
