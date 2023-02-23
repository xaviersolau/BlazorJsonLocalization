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
    }
}
