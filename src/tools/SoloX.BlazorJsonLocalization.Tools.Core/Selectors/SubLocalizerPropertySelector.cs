using Microsoft.CodeAnalysis;
using SoloX.GeneratorTools.Core.CSharp.Generator.Selectors;
using SoloX.GeneratorTools.Core.CSharp.Model;
using SoloX.GeneratorTools.Core.CSharp.Workspace;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Selectors
{
    public class SubLocalizerPropertySelector : ISelector
    {
        /// <inheritdoc/>
        public IEnumerable<IDeclaration<SyntaxNode>> GetDeclarations(IEnumerable<ICSharpFile> files)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IEnumerable<IPropertyDeclaration> GetProperties(IGenericDeclaration<SyntaxNode> declaration)
        {
            return declaration?.Properties.Where(p => p.HasGetter && !p.HasSetter && p.PropertyType.Declaration.Name.EndsWith("Localizer"));
        }

        /// <inheritdoc/>
        public IEnumerable<IMethodDeclaration> GetMethods(IGenericDeclaration<SyntaxNode> declaration)
        {
            throw new NotImplementedException();
        }
    }
}
