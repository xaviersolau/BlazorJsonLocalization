// ----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Core.Impl;
using SoloX.BlazorJsonLocalization.Services;
using SoloX.BlazorJsonLocalization.Services.Impl;

namespace SoloX.BlazorJsonLocalization
{
    /// <summary>
    /// Extension methods to setup the Json localization.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add Json localization services with the embedded Json resource files support.
        /// </summary>
        /// <param name="services">The service collection to setup.</param>
        /// <param name="serviceLifetime">Service Lifetime to use to register the IStringLocalizerFactory. (Default is Scoped)</param>
        /// <returns>The given service collection updated with the Json localization services.</returns>
        public static IServiceCollection AddJsonLocalization(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            => services.AddJsonLocalization(builder => builder.UseEmbeddedJson(), serviceLifetime);

        /// <summary>
        /// Add Json localization services.
        /// </summary>
        /// <param name="services">The service collection to setup.</param>
        /// <param name="setupAction">The action delegate to fine tune the Json localizer behavior
        /// (Use embedded Json resource files if null).</param>
        /// <param name="serviceLifetime">Service Lifetime to use to register the IStringLocalizerFactory. (Default is Scoped)</param>
        /// <returns>The given service collection updated with the Json localization services.</returns>
        public static IServiceCollection AddJsonLocalization(this IServiceCollection services, Action<JsonLocalizationOptionsBuilder> setupAction, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            services
                .AddLocalization()
                .AddSingleton<ICultureInfoService, CultureInfoService>()
                .AddSingleton<ICacheService, CacheService>()
                .AddSingleton<
                    IJsonLocalizationExtensionService<EmbeddedJsonLocalizationOptions>,
                    EmbeddedJsonLocalizationExtensionService>();

            switch (serviceLifetime)
            {
                case ServiceLifetime.Singleton:
                    services
                        .AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>()
                        .AddSingleton<IExtensionResolverService, ExtensionResolverService>();
                    break;
                case ServiceLifetime.Scoped:
                    services
                        .AddScoped<IStringLocalizerFactory, JsonStringLocalizerFactory>()
                        .AddScoped<IExtensionResolverService, ExtensionResolverService>();
                    break;
                case ServiceLifetime.Transient:
                default:
                    services
                        .AddTransient<IStringLocalizerFactory, JsonStringLocalizerFactory>()
                        .AddTransient<IExtensionResolverService, ExtensionResolverService>();
                    break;
            }

            var builder = new JsonLocalizationOptionsBuilder();

            if (setupAction != null)
            {
                setupAction.Invoke(builder);
            }
            else
            {
                builder.UseEmbeddedJson();
            }

            services.Configure<JsonLocalizationOptions>(builder.Build);

            return services;
        }
    }
}
