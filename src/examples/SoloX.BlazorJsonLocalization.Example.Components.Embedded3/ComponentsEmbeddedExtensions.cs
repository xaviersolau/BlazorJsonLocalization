using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloX.BlazorJsonLocalization.Example.Components.Embedded3
{
    public static class ComponentsEmbeddedExtensions
    {
        public static JsonLocalizationOptionsBuilder UseComponentsEmbedded3(this JsonLocalizationOptionsBuilder builder)
        {
            return builder.UseEmbeddedJson(options =>
            {
                options.Assemblies = new[] { typeof(ComponentsEmbeddedExtensions).Assembly };
                options.NamingPolicy = (basePath, cultureName) => $"{basePath}.razor{(string.IsNullOrEmpty(cultureName) ? string.Empty : $"-{cultureName}")}.json";
            });
        }
    }
}
