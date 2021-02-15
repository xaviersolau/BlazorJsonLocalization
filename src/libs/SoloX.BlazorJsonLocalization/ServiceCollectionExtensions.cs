using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Core;

namespace SoloX.BlazorJsonLocalization
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonLocalization(this IServiceCollection services)
        {
            services
                .AddLocalization()
                .TryAddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();

            return services;
        }

        public static IServiceCollection AddJsonLocalization(this IServiceCollection services, Action<JsonLocalizationOptions> setupAction)
        {
            services
                .AddLocalization()
                .TryAddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();

            services.Configure(setupAction);

            return services;
        }
    }
}
