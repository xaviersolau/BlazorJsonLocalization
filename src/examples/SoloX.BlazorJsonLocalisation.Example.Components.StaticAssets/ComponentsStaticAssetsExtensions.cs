using SoloX.BlazorJsonLocalization;
using SoloX.BlazorJsonLocalization.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SoloX.BlazorJsonLocalisation.Example.Components.StaticAssets
{
    public static class ComponentsStaticAssetsExtensions
    {
        public static JsonLocalizationOptionsBuilder UseComponentsStaticAssets(this JsonLocalizationOptionsBuilder builder)
        {
            return builder.UseHttpHostedJson(options =>
            {
                options.AssemblyNames = new[] { typeof(ComponentsStaticAssetsExtensions).Assembly.GetName().Name };

                // we don't need to setup the applicationAssembly because this options focuses only on this component that will be a component of the application.
                //options.ApplicationAssembly = null;

                options.NamingPolicy = (basePath, cultureName) => new Uri($"{basePath}{(string.IsNullOrEmpty(cultureName) ? string.Empty : $"-{cultureName}")}.json", UriKind.Relative);
                options.ResourcesPath = "Resources";
            });
        }
    }
}
