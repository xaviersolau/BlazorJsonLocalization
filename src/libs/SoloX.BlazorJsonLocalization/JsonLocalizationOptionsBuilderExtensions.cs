using System;
using System.Collections.Generic;
using System.Text;

namespace SoloX.BlazorJsonLocalization
{
    public static class JsonLocalizationOptionsBuilderExtensions
    {
        public static void UseEmbeddedJson(this JsonLocalizationOptionsBuilder builder)
            => builder.UseEmbeddedJson(null);

        public static void UseEmbeddedJson(this JsonLocalizationOptionsBuilder builder, Action<EmbeddedJsonLocalizationOptions> setup)
        {
            var optExt = new EmbeddedJsonLocalizationOptions();

            setup?.Invoke(optExt);

            builder.AddOptionsExtension(optExt);
        }
    }
}
