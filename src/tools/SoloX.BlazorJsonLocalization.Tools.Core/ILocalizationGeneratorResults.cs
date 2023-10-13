// ----------------------------------------------------------------------
// <copyright file="ILocalizationGeneratorResults.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Collections.Generic;

namespace SoloX.BlazorJsonLocalization.Tools.Core
{
    /// <summary>
    /// Localization generator results.
    /// </summary>
    public interface ILocalizationGeneratorResults
    {
        /// <summary>
        /// Files listed as input.
        /// </summary>
        IEnumerable<string> InputFiles { get; }

        /// <summary>
        /// Generated code files.
        /// </summary>
        IEnumerable<string> GeneratedCodeFiles { get; }

        /// <summary>
        /// Generated Json resource files.
        /// </summary>
        IEnumerable<string> GeneratedResourceFiles { get; }
    }
}
