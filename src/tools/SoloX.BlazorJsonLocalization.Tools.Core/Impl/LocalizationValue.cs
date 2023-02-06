using System;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Impl
{
    public class LocalizationValue : ALocalizationData
    {
        public string Value { get; set; }

        public override ALocalizationData Merge(ALocalizationData source, string path, out bool dirty)
        {
            dirty = false;
            switch (source)
            {
                case LocalizationMap map:
                    {
                        // Warning overwriting a complex structure by a simple value on path
                        dirty = true;
                        return this;
                    }
                    break;
                case LocalizationValue mapValue:
                    {
                        return source;
                    }
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
