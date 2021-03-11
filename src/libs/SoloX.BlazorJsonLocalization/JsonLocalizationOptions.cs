using SoloX.BlazorJsonLocalization.Core;
using System.Collections.Generic;

namespace SoloX.BlazorJsonLocalization
{
    public sealed class JsonLocalizationOptions
    {
        public IEnumerable<IJsonLocalizationOptionsExtension> OptionsExtensions { get; internal set; }
    }
}
