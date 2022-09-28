using SoloX.BlazorJsonLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloX.BlazorJsonLocalisation.Example.Components.Embedded
{
    public static class ComponentsEmbeddedExtensions
    {
        public static JsonLocalizationOptionsBuilder UseComponentsEmbedded(this JsonLocalizationOptionsBuilder builder)
        {
            return builder.UseEmbeddedJson(options =>
            {
                options.AssemblyNames = new[] { typeof(ComponentsEmbeddedExtensions).Assembly.GetName().Name };
                options.ResourcesPath = "Resources";
            });
        }
    }
}
