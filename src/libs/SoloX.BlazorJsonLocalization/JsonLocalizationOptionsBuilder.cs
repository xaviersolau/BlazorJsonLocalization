using SoloX.BlazorJsonLocalization.Core;
using SoloX.BlazorJsonLocalization.Core.Impl;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoloX.BlazorJsonLocalization
{
    public sealed class JsonLocalizationOptionsBuilder
    {
        private IList<IJsonLocalizationOptionsExtension> _optionsExtensions = new List<IJsonLocalizationOptionsExtension>();

        public void AddOptionsExtension<TOptions>(TOptions options)
        {
            _optionsExtensions.Add(new JsonLocalizationOptionsExtension<TOptions>(options));
        }

        public void Build(JsonLocalizationOptions opt) => opt.OptionsExtensions = _optionsExtensions;
    }
}
