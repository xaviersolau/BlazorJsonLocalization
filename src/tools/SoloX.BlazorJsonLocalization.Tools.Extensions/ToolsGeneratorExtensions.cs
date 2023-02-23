// ----------------------------------------------------------------------
// <copyright file="ToolsGeneratorExtensions.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using SoloX.BlazorJsonLocalization.Tools.Core;
using SoloX.BlazorJsonLocalization.Tools.Core.Impl;
using SoloX.GeneratorTools.Core.CSharp.Extensions;

namespace SoloX.BlazorJsonLocalization.Tools.Extensions
{
    /// <summary>
    /// Extension methods to register the Tools generator.
    /// </summary>
    public static class ToolsGeneratorExtensions
    {
        /// <summary>
        /// Add dependency injections for the state generator.
        /// </summary>
        /// <param name="services">The service collection where to setup dependencies.</param>
        /// <returns>The input services once setup is done.</returns>
        public static IServiceCollection AddToolsGenerator(this IServiceCollection services)
        {
            return services
                .AddCSharpToolsGenerator()
                .AddTransient<IToolsGenerator, ToolsGenerator>();
        }
    }
}
