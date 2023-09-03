// ----------------------------------------------------------------------
// <copyright file="ALocalizerSelector.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using SoloX.BlazorJsonLocalization.Attributes;
using SoloX.GeneratorTools.Core.CSharp.Generator.Selectors;
using SoloX.GeneratorTools.Core.CSharp.Model;
using SoloX.GeneratorTools.Core.CSharp.Workspace;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Selectors
{
    /// <summary>
    /// Custom PropertySelector to get Localizer properties or arguments.
    /// </summary>
    public abstract class ALocalizerSelector : ISelector
    {
        /// <inheritdoc/>
        public IEnumerable<IDeclaration<SyntaxNode>> GetDeclarations(IEnumerable<ICSharpFile> files)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public abstract IEnumerable<IPropertyDeclaration> GetProperties(IGenericDeclaration<SyntaxNode> declaration);

        /// <summary>
        /// Tells if a PropertyDeclaration is a readonly string property.
        /// </summary>
        /// <param name="propertyDeclaration">The propertyDeclaration to test.</param>
        /// <returns></returns>
        protected static bool IsStringProperty(IPropertyDeclaration propertyDeclaration)
        {
            if (propertyDeclaration == null)
            {
                throw new ArgumentNullException(nameof(propertyDeclaration));
            }

            return propertyDeclaration.HasGetter && !propertyDeclaration.HasSetter && propertyDeclaration.PropertyType.Declaration.Name.EndsWith("string", StringComparison.Ordinal);
        }

        /// <summary>
        /// Tells if a PropertyDeclaration is a localizer argument.
        /// </summary>
        /// <param name="propertyDeclaration">The propertyDeclaration to test.</param>
        /// <returns></returns>
        protected static bool IsLocalizerArgument(IPropertyDeclaration propertyDeclaration)
        {
            if (propertyDeclaration == null)
            {
                throw new ArgumentNullException(nameof(propertyDeclaration));
            }

            return propertyDeclaration.Attributes.Any(a => a.Name == nameof(TranslateArgAttribute));
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
