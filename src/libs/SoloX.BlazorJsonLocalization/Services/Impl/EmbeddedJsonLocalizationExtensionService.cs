// ----------------------------------------------------------------------
// <copyright file="EmbeddedJsonLocalizationExtensionService.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoloX.BlazorJsonLocalization.Services.Impl
{
    /// <summary>
    /// Embedded Json Localization extension service.
    /// </summary>
    public class EmbeddedJsonLocalizationExtensionService : IJsonLocalizationExtensionService<EmbeddedJsonLocalizationOptions>
    {
        ///<inheritdoc/>
        public async ValueTask<IReadOnlyDictionary<string, string>?> TryLoadAsync(
            EmbeddedJsonLocalizationOptions options,
            Assembly assembly,
            string baseName,
            CultureInfo cultureInfo)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (cultureInfo == null)
            {
                throw new ArgumentNullException(nameof(cultureInfo));
            }

            var embeddedFileProvider = GetFileProvider(assembly);
            return await LoadStringMapAsync(embeddedFileProvider, options.ResourcesPath, baseName, cultureInfo)
                .ConfigureAwait(false);
        }

        private static async ValueTask<Dictionary<string, string>?> LoadStringMapAsync(
            IFileProvider fileProvider,
            string resourcesPath,
            string baseName,
            CultureInfo cultureInfo)
        {
            var cultureName = cultureInfo.TwoLetterISOLanguageName;

            Func<string, string> pathBuilder = string.IsNullOrEmpty(resourcesPath)
                ? s => s
                : s => Path.Combine(resourcesPath, s);

            var fileInfo =
                fileProvider.GetFileInfo(
                    pathBuilder($"{baseName}-{cultureName}.json"));

            if (!fileInfo.Exists)
            {
                fileInfo =
                    fileProvider.GetFileInfo(
                        pathBuilder($"{baseName}.json"));
            }

            if (!fileInfo.Exists)
            {
                return null;
            }

            using var stream = fileInfo.CreateReadStream();

            var map = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(stream).ConfigureAwait(false);

            if (map == null)
            {
                throw new FileLoadException("Null resources");
            }

            return map;
        }

        private static IFileProvider GetFileProvider(Assembly assembly)
        {
            return new EmbeddedFileProvider(assembly);
        }
    }
}
