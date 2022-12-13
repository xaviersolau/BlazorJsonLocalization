using Microsoft.Extensions.DependencyInjection;
using SoloX.BlazorJsonLocalization.Tools.Core;
using SoloX.BlazorJsonLocalization.Tools.Core.Impl;
using SoloX.GeneratorTools.Core.CSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloX.BlazorJsonLocalization.Tools.Extensions
{
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
