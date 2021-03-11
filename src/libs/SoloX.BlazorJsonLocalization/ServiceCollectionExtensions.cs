using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Core;
using SoloX.BlazorJsonLocalization.Core.Impl;
using SoloX.BlazorJsonLocalization.Services;
using SoloX.BlazorJsonLocalization.Services.Impl;

namespace SoloX.BlazorJsonLocalization
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonLocalization(this IServiceCollection services)
            => services.AddJsonLocalization(builder => builder.UseEmbeddedJson());

        public static IServiceCollection AddJsonLocalization(this IServiceCollection services, Action<JsonLocalizationOptionsBuilder> setupAction)
        {
            services
                .AddLocalization()
                .AddSingleton<ICultureInfoService, CultureInfoService>()
                .AddScoped<IStringLocalizerFactory, JsonStringLocalizerFactory>();

            services.AddScoped<
                IJsonLocalizationService<EmbeddedJsonLocalizationOptions>,
                EmbeddedJsonLocalizationService>();

            var builder = new JsonLocalizationOptionsBuilder();
            setupAction.Invoke(builder);

            services.Configure<JsonLocalizationOptions>(builder.Build);

            return services;
        }
    }
}
