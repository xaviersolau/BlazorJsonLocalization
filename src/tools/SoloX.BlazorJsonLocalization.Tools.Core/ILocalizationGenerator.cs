// ----------------------------------------------------------------------
// <copyright file="ILocalizationGenerator.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace SoloX.BlazorJsonLocalization.Tools.Core
{
    /// <summary>
    /// Localization tools generator interface.
    /// </summary>
    public interface ILocalizationGenerator
    {
        /// <summary>
        /// Apply the generator tools on the given project.
        /// </summary>
        /// <param name="projectFile">Project file.</param>
        /// <param name="generatorOptions">Generator options.</param>
        ILocalizationGeneratorResults Generate(
            string projectFile,
            GeneratorOptions? generatorOptions = null);

        /// <summary>
        /// Apply the generator tools on the given compilation instance.
        /// </summary>
        /// <param name="compilation">Compilation instance.</param>
        /// <param name="classes">Input classes.</param>
        /// <param name="context">Source production context.</param>
        /// <param name="generatorOptions">Generator options.</param>
        ILocalizationGeneratorResults Generate(
            Compilation compilation,
            ImmutableArray<InterfaceDeclarationSyntax> classes,
            SourceProductionContext context,
            GeneratorOptions? generatorOptions = null);
    }

    /// <summary>
    /// Generator options.
    /// </summary>
    public class GeneratorOptions
    {
        /// <summary>
        /// Tells to use Relaxed Json Escaping.
        /// </summary>
        public bool UseRelaxedJsonEscaping { get; }

        /// <summary>
        /// Tells to use Multi-Line Json values.
        /// </summary>
        public bool UseMultiLine { get; }

        /// <summary>
        /// New line separator. Use default if null
        /// </summary>
        public string? NewLineSeparator { get; set; }

        /// <summary>
        /// Setup options.
        /// </summary>
        /// <param name="useRelaxedJsonEscaping">Use special chars in the Json string values.</param>
        /// <param name="useMultiLine">Use Multi-Line in the Json string values.</param>
        public GeneratorOptions(bool useRelaxedJsonEscaping, bool useMultiLine)
        {
            UseRelaxedJsonEscaping = useRelaxedJsonEscaping;
            UseMultiLine = useMultiLine;
        }
    }
}
