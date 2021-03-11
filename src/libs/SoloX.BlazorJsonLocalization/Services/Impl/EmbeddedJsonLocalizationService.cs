using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace SoloX.BlazorJsonLocalization.Services.Impl
{
    public class EmbeddedJsonLocalizationService : IJsonLocalizationService<EmbeddedJsonLocalizationOptions>
    {
        public bool TryLoad(
            EmbeddedJsonLocalizationOptions options,
            Assembly assembly,
            string baseName,
            CultureInfo cultureInfo,
            out IReadOnlyDictionary<string, string> map)
        {
            var embeddedFileProvider = GetFileProvider(assembly);
            map = LoadStringMap(embeddedFileProvider, options.ResourcesPath, baseName, cultureInfo);
            return map != null;
        }

        private Dictionary<string, string> LoadStringMap(
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
                return null;
            }

            using var stream = fileInfo.CreateReadStream();
            using var reader = new StreamReader(stream);

            return JsonSerializer.Deserialize<Dictionary<string, string>>(reader.ReadToEnd());
        }

        public IFileProvider GetFileProvider(Assembly assembly)
        {
            return new EmbeddedFileProvider(assembly);
        }

    }
}
