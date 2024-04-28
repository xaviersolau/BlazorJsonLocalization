// ----------------------------------------------------------------------
// <copyright file="LocalizationMap.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Impl.Localization
{
    /// <summary>
    /// Localization data map (a node of the Json tree).
    /// </summary>
    public class LocalizationMap : ALocalizationData
    {
        /// <summary>
        /// The Mapped values.
        /// </summary>
        public IReadOnlyDictionary<string, ALocalizationData> ValueMap { get; private set; }

        /// <summary>
        /// Setup LocalizationMap instance.
        /// </summary>
        /// <param name="valueMap">value to map.</param>
        public LocalizationMap(IReadOnlyDictionary<string, ALocalizationData> valueMap)
        {
            ValueMap = valueMap;
        }

        /// <inheritdoc/>
        public override void SetMultiLine(bool multiLine)
        {
            foreach (var item in ValueMap)
            {
                item.Value.SetMultiLine(multiLine);
            }
        }

        /// <inheritdoc/>
        public override void SetNewLineSeparator(string? newLineSeparator = null)
        {
            if (newLineSeparator == null)
            {
                return;
            }

            foreach (var item in ValueMap)
            {
                item.Value.SetNewLineSeparator(newLineSeparator);
            }
        }

        /// <inheritdoc/>
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

                        return new LocalizationMap(mergedMap);
                    }
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

        /// <inheritdoc/>
        public override bool Trim()
        {
            var isEmpty = true;

            var trimedMap = new Dictionary<string, ALocalizationData>();

            foreach (var item in ValueMap)
            {
                if (!item.Value.Trim())
                {
                    isEmpty = false;
                    trimedMap.Add(item.Key, item.Value);
                }
            }

            ValueMap = trimedMap;

            return isEmpty;
        }
    }
}
