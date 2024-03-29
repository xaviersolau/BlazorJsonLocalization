﻿// ----------------------------------------------------------------------
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

            // TODO enhance sub localizer property detection.

            return declaration.Properties.Where(p => p.HasGetter && !p.HasSetter && p.PropertyType.Declaration.Name.EndsWith("Localizer", StringComparison.Ordinal));
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
