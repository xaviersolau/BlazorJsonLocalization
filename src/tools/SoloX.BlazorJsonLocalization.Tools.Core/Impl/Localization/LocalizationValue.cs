// ----------------------------------------------------------------------
// <copyright file="LocalizationValue.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Impl.Localization
{
    /// <summary>
    /// Localization data value (a leaf of the Json tree).
    /// </summary>
    public class LocalizationValue : ALocalizationData
    {
        /// <summary>
        /// Enable Multi-Line.
        /// </summary>
        public bool MultiLine { get; private set; }

        /// <summary>
        /// New line separator to use when MultiLine is disabled.
        /// </summary>
        public string? NewLineSeparator { get; private set; }

        /// <summary>
        /// The localization data leaf value.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Setup instance.
        /// </summary>
        /// <param name="value">Leaf value.</param>
        public LocalizationValue(string value)
        {
            Value = value;
        }

        /// <inheritdoc/>
        public override void SetMultiLine(bool multiLine)
        {
            MultiLine = multiLine;
        }

        /// <inheritdoc/>
        public override void SetNewLineSeparator(string? newLineSeparator = null)
        {
            NewLineSeparator = newLineSeparator;
        }

        /// <inheritdoc/>
        public override ALocalizationData Merge(ALocalizationData source, string path, out bool dirty)
        {
            dirty = false;
            switch (source)
            {
                case LocalizationMap:
                    {
                        // Warning overwriting a complex structure by a simple value on path
                        dirty = true;
                        return this;
                    }
                case LocalizationValue:
                    {
                        return source;
                    }
                default:
                    throw new NotSupportedException();
            }
        }

        /// <inheritdoc/>
        public override bool Trim()
        {
            return false;
        }
    }
}
