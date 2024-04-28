// ----------------------------------------------------------------------
// <copyright file="SubLocalizerPropertySelector.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using SoloX.GeneratorTools.Core.CSharp.Generator.Selectors;
using SoloX.GeneratorTools.Core.CSharp.Model;
using SoloX.GeneratorTools.Core.CSharp.Workspace;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Attributes;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Selectors
{
    /// <summary>
    /// Custom PropertySelector to get SubLocalizer attributed classes.
    /// </summary>
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
            if (declaration == null)
            {
                throw new ArgumentNullException(nameof(declaration));
            }

            return declaration.Properties.Where(p => p.HasGetter && !p.HasSetter && ExtendsStringLocalizer(p.PropertyType.Declaration));
        }

        private static bool ExtendsStringLocalizer(IDeclaration<SyntaxNode> declaration)
        {
            if (declaration is IGenericDeclaration<SyntaxNode> genericDeclaration)
            {
                if (genericDeclaration.Attributes.FirstOrDefault(a => a.Name == nameof(SubLocalizerAttribute)) != null
                    && genericDeclaration.Extends?.FirstOrDefault(e => e.Declaration.Name == nameof(IStringLocalizer)) != null)
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public IEnumerable<IMethodDeclaration> GetMethods(IGenericDeclaration<SyntaxNode> declaration)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IEnumerable<IConstantDeclaration> GetConstants(IGenericDeclaration<SyntaxNode> declaration)
        {
            throw new NotImplementedException();
        }
    }
}
