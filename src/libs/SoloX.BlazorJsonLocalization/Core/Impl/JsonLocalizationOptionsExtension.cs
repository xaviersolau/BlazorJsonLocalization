using Microsoft.Extensions.DependencyInjection;
using SoloX.BlazorJsonLocalization.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoloX.BlazorJsonLocalization.Core.Impl
{
    public sealed class JsonLocalizationOptionsExtension<TOptions> : IJsonLocalizationOptionsExtension
    {
        public JsonLocalizationOptionsExtension(TOptions options)
        {
            Options = options;
        }

        public TOptions Options { get; }

        public IJsonLocalizationService GetJsonLocalizationService(IServiceProvider serviceProvider)
        {
            var optionsService = serviceProvider
                .GetRequiredService<IJsonLocalizationService<TOptions>>();

            return optionsService;
        }
    }
}
