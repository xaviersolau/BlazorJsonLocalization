// ----------------------------------------------------------------------
// <copyright file="LocalizerArgumentSelector.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using SoloX.GeneratorTools.Core.CSharp.Model;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Selectors
{
    /// <summary>
    /// Custom PropertySelector to get string only properties.
    /// </summary>
    public class LocalizerArgumentSelector : ALocalizerSelector
    {
        /// <inheritdoc/>
        public override IEnumerable<IPropertyDeclaration> GetProperties(IGenericDeclaration<SyntaxNode> declaration)
        {
            if (declaration == null)
            {
                throw new ArgumentNullException(nameof(declaration));
            }

            return declaration.Properties.Where(p => IsStringProperty(p) && IsLocalizerArgument(p));
        }
    }
}
