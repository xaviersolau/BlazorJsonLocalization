using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Impl
{
    public class LocalizationMap : ALocalizationData
    {
        public IDictionary<string, ALocalizationData> ValueMap { get; set; }

        public override ALocalizationData Merge(ALocalizationData source, string path, out bool dirty)
        {
            dirty = false;

            switch (source)
            {
                case LocalizationMap sourceMap:
                    {
                        var mergedMap = new Dictionary<string, ALocalizationData>();

                        foreach (var item in ValueMap)
                        {
                            if (sourceMap.ValueMap.TryGetValue(item.Key, out var sourceMapValue))
                            {
                                // Existing key.
                                mergedMap.Add(item.Key, item.Value.Merge(sourceMapValue, path + ":" + item.Key, out var dirtyItem));

                                dirty |= dirtyItem;
                            }
                            else
                            {
                                // Adding new key.
                                mergedMap.Add(item.Key, item.Value);

                                dirty = true;
                            }
                        }

                        foreach (var item in sourceMap.ValueMap.Where(i => !ValueMap.ContainsKey(i.Key)))
                        {
                            // Warning unused localization key detected.
                            mergedMap.Add(item.Key, item.Value);
                        }

                        return new LocalizationMap { ValueMap = mergedMap };
                    }
                    break;
                case LocalizationValue mapValue:
                    {
                        // Warning overwriting a simple by a complex structure on path
                        dirty = true;
                        return this;
                    }
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
