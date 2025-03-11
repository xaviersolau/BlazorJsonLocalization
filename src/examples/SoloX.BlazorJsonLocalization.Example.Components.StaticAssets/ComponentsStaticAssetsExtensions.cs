using SoloX.BlazorJsonLocalization;
using SoloX.BlazorJsonLocalization.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SoloX.BlazorJsonLocalization.Example.Components.StaticAssets
{
    public static class ComponentsStaticAssetsExtensions
    {
        /// <summary>
        /// Setup for Http hosted static asserts.
        /// </summary>
        /// <typeparam name="TOptions">HttpClientJsonLocalizationOptions must be use for web assembly projects and
        /// HttpHostedJsonLocalizationOptions for server side (if loading directly resource files)
        /// </typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static JsonLocalizationOptionsBuilder UseComponentsStaticAssets<TOptions>(this JsonLocalizationOptionsBuilder builder)
            where TOptions : HttpHostedJsonLocalizationOptions, new()
        {
            return builder.UseHttpHostedJson<TOptions>(options =>
            {
                options.Assemblies = new[] { typeof(ComponentsStaticAssetsExtensions).Assembly };

                // we don't need to setup the applicationAssembly because this options focuses only on this component that will be a component of the application.
                //options.ApplicationAssembly = null;

                options.NamingPolicy = (basePath, cultureName) => new Uri($"{basePath}{(string.IsNullOrEmpty(cultureName) ? string.Empty : $"-{cultureName}")}.json", UriKind.Relative);
                options.ResourcesPath = "Resources";
            });
        }
    }
}
