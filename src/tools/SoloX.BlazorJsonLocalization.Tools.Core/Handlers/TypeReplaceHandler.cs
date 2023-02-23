// ----------------------------------------------------------------------
// <copyright file="TypeReplaceHandler.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using SoloX.GeneratorTools.Core.CSharp.Generator.ReplacePattern;
using SoloX.GeneratorTools.Core.CSharp.Model;
using SoloX.GeneratorTools.Core.CSharp.Model.Use;
using System;
using System.Linq;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Handlers
{
    /// <summary>
    /// Custom replace pattern handler to replace the declaration interface first generic parameter name.
    /// like "MyObject" in "interface IMyObjectStringLocalizerPattern : IStringLocalizer MyObject"
    /// </summary>
    public class TypeReplaceHandler : IReplacePatternHandlerFactory, IReplacePatternHandler
    {
        private string source = default!;
        private string target = default!;

        /// <inheritdoc/>
        public string ApplyOn(string patternText)
        {
            if (patternText == null)
            {
                throw new ArgumentNullException(nameof(patternText));
            }

            return patternText.Replace(this.source, this.target);
        }

        /// <inheritdoc/>
        public IReplacePatternHandler Setup(IGenericDeclaration<SyntaxNode> pattern, IGenericDeclaration<SyntaxNode> declaration)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            if (declaration == null)
            {
                throw new ArgumentNullException(nameof(declaration));
            }

            this.source = ((IGenericDeclarationUse)pattern.Extends.First()).GenericParameters.First().Declaration.Name;
            this.target = ((IGenericDeclarationUse)declaration.Extends.First()).GenericParameters.First().Declaration.Name;
            return this;
        }
    }
}
