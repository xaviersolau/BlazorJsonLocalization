using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SoloX.GeneratorTools.Core.CSharp.Workspace;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloX.BlazorJsonLocalization.Tools.Core
{
    public interface IToolsGenerator
    {
        /// <summary>
        /// Apply the generator tools on the given project.
        /// </summary>
        /// <param name="projectFile">Project file.</param>
        void Generate(string projectFile);

        /// <summary>
        /// Apply the generator tools on the given compilation instance.
        /// </summary>
        /// <param name="compilation">Compilation instance.</param>
        void Generate(Compilation compilation, ImmutableArray<InterfaceDeclarationSyntax> classes, SourceProductionContext context);
    }
}
