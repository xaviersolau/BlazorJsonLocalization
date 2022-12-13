using Microsoft.CodeAnalysis;
using SoloX.GeneratorTools.Core.CSharp.Generator;
using SoloX.GeneratorTools.Core.CSharp.Generator.ReplacePattern;
using SoloX.GeneratorTools.Core.CSharp.Model;
using SoloX.GeneratorTools.Core.CSharp.Model.Use;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Handlers
{
    public class TypeReplaceHandler : IReplacePatternHandlerFactory, IReplacePatternHandler
    {
        private string source;
        private string target;

        public string ApplyOn(string patternText)
        {
            return patternText.Replace(this.source, this.target);
        }

        public IReplacePatternHandler Setup(IGenericDeclaration<SyntaxNode> pattern, IGenericDeclaration<SyntaxNode> declaration)
        {
            source = ((IGenericDeclarationUse)pattern.Extends.First()).GenericParameters.First().Declaration.Name;
            target = ((IGenericDeclarationUse)declaration.Extends.First()).GenericParameters.First().Declaration.Name;
            return this;
        }
    }
}
