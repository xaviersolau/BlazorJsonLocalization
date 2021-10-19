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
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using SoloX.BlazorJsonLocalization.Helpers;

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
        /// <param name="webHostEnvironment">The IWebHostEnvironment.</param>
        /// <param name="logger">Logger where to log processing messages.</param>
        public HttpHostedJsonLocalizationExtensionService(IWebHostEnvironment webHostEnvironment, ILogger<HttpHostedJsonLocalizationExtensionService> logger)
            : base(logger)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.logger = logger;
        }

        ///<inheritdoc/>
        protected override async ValueTask<IReadOnlyDictionary<string, string>?> TryLoadFromUriAsync(Uri uri, JsonSerializerOptions? jsonSerializerOptions)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            this.logger.LogDebug($"Loading localization data from {uri} using Web Host WebRootFileProvider");

            var fileInfo = this.webHostEnvironment.WebRootFileProvider.GetFileInfo(uri.OriginalString);

            if (!fileInfo.Exists)
            {
                this.logger.LogWarning($"Web Host File {uri} does not exist");
                return null;
            }

            this.logger.LogDebug($"Loading file {uri} does not exist");

            using var stream = fileInfo.CreateReadStream();

            var map = await JsonHelper
                .DeserializeAsync<Dictionary<string, string>>(stream, jsonSerializerOptions)
                .ConfigureAwait(false);

            return map ?? throw new FileLoadException("Null resources");
        }
    }
}
