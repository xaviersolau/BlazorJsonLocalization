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
using System.Text.Json;
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
    public class HttpHostedJsonLocalizationExtensionService : AHttpHostedJsonLocalizationExtensionService
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
        protected override async Task<IReadOnlyDictionary<string, string>?> TryLoadFromUriAsync(Uri uri, JsonSerializerOptions? jsonSerializerOptions)
        {
            ArgumentNullException.ThrowIfNull(uri, nameof(uri));

            this.logger.LoadingLocalizationDataFromHost(uri);

#pragma warning disable CA1062 // Validate arguments of public methods
            var fileInfo = this.webHostEnvironment.WebRootFileProvider.GetFileInfo(uri.OriginalString);
#pragma warning restore CA1062 // Validate arguments of public methods

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
