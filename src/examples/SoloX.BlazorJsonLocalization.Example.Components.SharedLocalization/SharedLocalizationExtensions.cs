﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloX.BlazorJsonLocalization.Example.Components.SharedLocalization
{
    public static class SharedLocalizationExtensions
    {
        public static JsonLocalizationOptionsBuilder UseSharedLocalization(this JsonLocalizationOptionsBuilder builder)
        {
            return builder.UseEmbeddedJson(options =>
            {
                options.Assemblies = new[] { typeof(SharedLocalizationExtensions).Assembly };
                options.ResourcesPath = "Resources";
            });
        }
    }
}
