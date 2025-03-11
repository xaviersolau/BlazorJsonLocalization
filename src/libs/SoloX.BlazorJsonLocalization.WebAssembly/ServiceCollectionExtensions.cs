// ----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
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
        [Obsolete("AddWebAssemblyJsonLocalization is now obsolete. You can use directly AddJsonLocalization (using SoloX.BlazorJsonLocalization).")]
        public static IServiceCollection AddWebAssemblyJsonLocalization(this IServiceCollection services, Action<JsonLocalizationOptionsBuilder> setupAction)
        {
            services
                .AddJsonLocalization(setupAction, ServiceLifetime.Scoped);

            return services;
        }
    }
}
