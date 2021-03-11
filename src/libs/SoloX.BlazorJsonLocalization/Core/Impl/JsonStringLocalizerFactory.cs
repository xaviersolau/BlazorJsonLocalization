using System;
using System.Reflection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using SoloX.BlazorJsonLocalization.Services;

namespace SoloX.BlazorJsonLocalization.Core.Impl
{
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private JsonLocalizationOptions _options;
        private IServiceProvider _serviceProvider;
        private ICultureInfoService _cultureInfoService;

        public JsonStringLocalizerFactory(
            IOptions<JsonLocalizationOptions> options,
            ICultureInfoService cultureInfoService,
            IServiceProvider serviceProvider)
        {
            _options = options.Value;
            _serviceProvider = serviceProvider;
            _cultureInfoService = cultureInfoService;
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
            var cultureInfo = _cultureInfoService.CurrentUICulture;

            foreach (var optionsExtension in _options.OptionsExtensions)
            {
                var optionsService = optionsExtension.GetJsonLocalizationService(_serviceProvider);

                if (optionsService.TryLoad(optionsExtension, assembly, baseName, cultureInfo, out var map))
                {
                    return new JsonStringLocalizer(map);
                }
            }

            return new NullStringLocalizer();
        }
    }
}
