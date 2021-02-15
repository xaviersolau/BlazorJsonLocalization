using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace SoloX.BlazorJsonLocalization.Core
{
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private JsonLocalizationOptions _options;

        public JsonStringLocalizerFactory(IOptions<JsonLocalizationOptions> options)
        {
            _options = options.Value;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            var resources = new EmbeddedFileProvider(resourceSource.Assembly);
            return new JsonStringLocalizer(resources, _options.Resources, resourceSource.Name);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            throw new NotSupportedException();
        }
    }
}
