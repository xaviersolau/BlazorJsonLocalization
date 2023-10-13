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
        ILocalizationGeneratorResults Generate(string projectFile);

        /// <summary>
        /// Apply the generator tools on the given compilation instance.
        /// </summary>
        /// <param name="compilation">Compilation instance.</param>
        ILocalizationGeneratorResults Generate(Compilation compilation, ImmutableArray<InterfaceDeclarationSyntax> classes, SourceProductionContext context);
    }
}
