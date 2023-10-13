// ----------------------------------------------------------------------
// <copyright file="LocalizationGeneratorResults.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Collections.Generic;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Impl
{
    internal class LocalizationGeneratorResults : ILocalizationGeneratorResults
    {
        /// <summary>
        /// Setup instance.
        /// </summary>
        public LocalizationGeneratorResults(
            IEnumerable<string> inputFiles,
            IEnumerable<string> generatedCodeFiles,
            IEnumerable<string> generatedResourceFiles)
        {
            InputFiles = inputFiles;
            GeneratedCodeFiles = generatedCodeFiles;
            GeneratedResourceFiles = generatedResourceFiles;
        }

        /// <inheritdoc/>
        public IEnumerable<string> InputFiles { get; private set; }

        /// <inheritdoc/>
        public IEnumerable<string> GeneratedCodeFiles { get; private set; }

        /// <inheritdoc/>
        public IEnumerable<string> GeneratedResourceFiles { get; private set; }
    }
}
