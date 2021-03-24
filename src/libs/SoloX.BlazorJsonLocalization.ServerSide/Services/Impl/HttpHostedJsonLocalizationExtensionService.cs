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

namespace SoloX.BlazorJsonLocalization.ServerSide.Services.Impl
{
    /// <summary>
    /// Http hosted Json Localization extension service.
    /// </summary>
    public class HttpHostedJsonLocalizationExtensionService : AHttpHostedJsonLocalizationExtensionService
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        /// <summary>
        /// Setup with the IWebHostEnvironment.
        /// </summary>
        /// <param name="webHostEnvironment">The IWebHostEnvironment.</param>
        public HttpHostedJsonLocalizationExtensionService(IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;
        }

        ///<inheritdoc/>
        protected override async ValueTask<IReadOnlyDictionary<string, string>?> TryLoadFromUriAsync(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            var fileInfo = this.webHostEnvironment.WebRootFileProvider.GetFileInfo(uri.OriginalString);

            if (!fileInfo.Exists)
            {
                return null;
            }

            using var stream = fileInfo.CreateReadStream();

            var map = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(stream).ConfigureAwait(false);

            return map ?? throw new FileLoadException("Null resources");
        }
    }
}
