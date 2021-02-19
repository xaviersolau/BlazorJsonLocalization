using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Core;
using SoloX.BlazorJsonLocalization.Core.Impl;

namespace SoloX.BlazorJsonLocalization
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonLocalization(this IServiceCollection services)
        {
            services.AddLocalization();

            services.TryAddScoped<IStringLocalizerFactory, JsonStringLocalizerFactory>();
            services.TryAddScoped<ILocalizerFileProviderFactory, LocalizerEmbeddedFileProviderFactory>();

            return services;
        }

        public static IServiceCollection AddJsonLocalization(this IServiceCollection services, Action<JsonLocalizationOptions> setupAction)
        {
            services
                .AddJsonLocalization()
                .Configure(setupAction);

            return services;
        }
    }
}
