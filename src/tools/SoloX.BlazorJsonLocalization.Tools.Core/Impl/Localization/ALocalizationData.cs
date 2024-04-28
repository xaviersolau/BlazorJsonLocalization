// ----------------------------------------------------------------------
// <copyright file="ALocalizationData.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Impl.Localization
{
    /// <summary>
    /// Abstract class to describe localization data.
    /// </summary>
    [JsonConverter(typeof(LocalizationMapConverter))]
    public abstract class ALocalizationData
    {
        /// <summary>
        /// Set multi-line parameter.
        /// </summary>
        /// <param name="multiLine"></param>
        public abstract void SetMultiLine(bool multiLine);

        /// <summary>
        /// Set new line separator parameter.
        /// </summary>
        /// <param name="newLineSeparator">New line separator when Multi line is disabled ("\\r\\n" or "\\n")</param>
        public abstract void SetNewLineSeparator(string? newLineSeparator = null);

#pragma warning disable CA1021 // Avoid out parameters
        /// <summary>
        /// Merge the current LocalizationData with the source.
        /// </summary>
        /// <param name="source">Source localization data that may be customized by hand.</param>
        /// <param name="path">Localization path key.</param>
        /// <param name="dirty">Tells if the result is modified after merge or if it is same as the source.</param>
        /// <returns>The merged localization data.</returns>
        public abstract ALocalizationData Merge(ALocalizationData source, string path, out bool dirty);
#pragma warning restore CA1021 // Avoid out parameters

        /// <summary>
        /// Remove empty entries from data tree.
        /// </summary>
        /// <returns>True if the resulting tree is empty.</returns>
        public abstract bool Trim();
    }
}
