// ----------------------------------------------------------------------
// <copyright file="SelectorResolver.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.BlazorJsonLocalization.Attributes;
using SoloX.GeneratorTools.Core.CSharp.Generator.Impl;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Selectors
{
    internal class SelectorResolver : DefaultSelectorResolver
    {
        public SelectorResolver()
            : base(typeof(SubLocalizerPropertySelector), typeof(LocalizerPropertySelector), typeof(LocalizerArgumentSelector), typeof(LocalizerAttribute), typeof(SubLocalizerAttribute))
        { }
    }
}
