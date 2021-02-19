using System;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace SoloX.BlazorJsonLocalization.Core.Impl
{
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private JsonLocalizationOptions _options;
        private ILocalizerFileProviderFactory _fileProviderFactory;

        public JsonStringLocalizerFactory(IOptions<JsonLocalizationOptions> options, ILocalizerFileProviderFactory fileProviderFactory)
        {
            _options = options.Value;
            _fileProviderFactory = fileProviderFactory;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            var assembly = resourceSource.Assembly;
            var baseName = resourceSource.Name;

            return CreateStringLocalizer(baseName, assembly);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            var assembly = Assembly.Load(location);

            return CreateStringLocalizer(baseName, assembly);
        }

        private IStringLocalizer CreateStringLocalizer(string baseName, Assembly assembly)
        {
            var resources = _fileProviderFactory.GetFileProvider(assembly);

            return new JsonStringLocalizer(resources, _options.ResourcesPath, baseName);
        }
    }
}
