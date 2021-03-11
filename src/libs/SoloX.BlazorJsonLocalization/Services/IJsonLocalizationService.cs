using SoloX.BlazorJsonLocalization.Core;
using SoloX.BlazorJsonLocalization.Core.Impl;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace SoloX.BlazorJsonLocalization.Services
{
    public interface IJsonLocalizationService
    {
        bool TryLoad(
            IJsonLocalizationOptionsExtension optionsExtension,
            Assembly assembly,
            string baseName,
            CultureInfo cultureInfo,
            out IReadOnlyDictionary<string, string> map);
    }

    public interface IJsonLocalizationService<TOptions> : IJsonLocalizationService
    {
        bool IJsonLocalizationService.TryLoad(
            IJsonLocalizationOptionsExtension optionsExtension,
            Assembly assembly,
            string baseName,
            CultureInfo cultureInfo,
            out IReadOnlyDictionary<string, string> map)
        {
            var typedOptionsExtension = (JsonLocalizationOptionsExtension<TOptions>) optionsExtension;

            return TryLoad(typedOptionsExtension.Options, assembly, baseName, cultureInfo, out map);
        }

        bool TryLoad(
            TOptions options,
            Assembly assembly,
            string baseName,
            CultureInfo cultureInfo,
            out IReadOnlyDictionary<string, string> map);
    }
}
