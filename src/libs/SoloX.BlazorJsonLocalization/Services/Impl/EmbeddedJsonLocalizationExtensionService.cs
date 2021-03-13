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

namespace SoloX.BlazorJsonLocalization.Services.Impl
{
    /// <summary>
    /// Embedded Json Localization extension service.
    /// </summary>
    public class EmbeddedJsonLocalizationExtensionService : IJsonLocalizationExtensionService<EmbeddedJsonLocalizationOptions>
    {
        private static readonly Dictionary<string, string> EmptyMap = new Dictionary<string, string>();

        ///<inheritdoc/>
        public bool TryLoad(
            EmbeddedJsonLocalizationOptions options,
            Assembly assembly,
            string baseName,
            CultureInfo cultureInfo,
            out IReadOnlyDictionary<string, string> map)
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
            map = LoadStringMap(embeddedFileProvider, options.ResourcesPath, baseName, cultureInfo);
            return map != null;
        }

        private static Dictionary<string, string> LoadStringMap(
            IFileProvider fileProvider,
            string resourcesPath,
            string baseName,
            CultureInfo cultureInfo)
        {
            var cultureName = cultureInfo.TwoLetterISOLanguageName;

            var fileInfo =
                fileProvider.GetFileInfo(
                    Path.Combine(resourcesPath, $"{baseName}-{cultureName}.json"));

            if (!fileInfo.Exists)
            {
                fileInfo =
                    fileProvider.GetFileInfo(
                        Path.Combine(resourcesPath, $"{baseName}.json"));
            }

            if (!fileInfo.Exists)
            {
                return EmptyMap;
            }

            using var stream = fileInfo.CreateReadStream();
            using var reader = new StreamReader(stream);

            var map = JsonSerializer.Deserialize<Dictionary<string, string>>(reader.ReadToEnd());

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
