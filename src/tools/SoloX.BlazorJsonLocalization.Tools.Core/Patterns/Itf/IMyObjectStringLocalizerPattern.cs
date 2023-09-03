// ----------------------------------------------------------------------
// <copyright file="IMyObjectStringLocalizerPattern.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Tools.Core.Selectors;
using SoloX.GeneratorTools.Core.CSharp.Generator.Attributes;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Itf
{
    /// <summary>
    /// Code first string localizer definition pattern.
    /// </summary>
    public interface IMyObjectStringLocalizerPattern : IStringLocalizer<MyObject>
    {
        /// <summary>
        /// Get MyObjectSubStringLocalizerProperty Sub string localizer.
        /// </summary>
        [Pattern<SubLocalizerPropertySelector>]
        IMyObjectSubStringLocalizerPattern MyObjectSubStringLocalizerProperty { get; }

        /// <summary>
        /// Get SomeProperty localized string.
        /// </summary>
        [Pattern<LocalizerPropertySelector>]
        string SomeProperty { get; }

        /// <summary>
        /// Get SomeStringArgs localized string.
        /// </summary>
        string SomeStringArgs(object someParameter);
    }
}
