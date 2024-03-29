﻿using SoloX.BlazorJsonLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloX.BlazorJsonLocalization.Example.Components.Embedded
{
    public static class ComponentsEmbeddedExtensions
    {
        public static JsonLocalizationOptionsBuilder UseComponentsEmbedded(this JsonLocalizationOptionsBuilder builder)
        {
            return builder.UseEmbeddedJson(options =>
            {
                options.Assemblies = new[] { typeof(ComponentsEmbeddedExtensions).Assembly };
                options.ResourcesPath = "Resources";
            });
        }
    }
}
