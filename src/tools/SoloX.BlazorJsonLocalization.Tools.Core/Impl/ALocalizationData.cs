using System.Runtime.InteropServices.ComTypes;
using System.Text.Json.Serialization;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Impl
{
    [JsonConverter(typeof(LocalizationMapConverter))]
    public abstract class ALocalizationData
    {
        public abstract ALocalizationData Merge(ALocalizationData source, string path, out bool dirty);
    }
}
