// ----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using SoloX.BlazorJsonLocalization.Services;
using SoloX.BlazorJsonLocalization.WebAssembly.Services.Impl;
using System;

namespace SoloX.BlazorJsonLocalization.WebAssembly
{
    /// <summary>
    /// Extension methods to setup the WebAssembly Json localization.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add WebAssembly Json localization services.
        /// </summary>
        /// <param name="services">The service collection to setup.</param>
        /// <param name="setupAction">The action delegate to fine tune the Json localizer behavior
        /// (Use embedded Json resource files if null).</param>
        /// <returns>The given service collection updated with the Json localization services.</returns>
        public static IServiceCollection AddWebAssemblyJsonLocalization(this IServiceCollection services, Action<JsonLocalizationOptionsBuilder> setupAction)
        {
            services
                .AddJsonLocalization(setupAction, ServiceLifetime.Scoped);

            services.AddScoped<
                IJsonLocalizationExtensionService<HttpHostedJsonLocalizationOptions>,
                HttpHostedJsonLocalizationExtensionService>();

            return services;
        }
    }
}
