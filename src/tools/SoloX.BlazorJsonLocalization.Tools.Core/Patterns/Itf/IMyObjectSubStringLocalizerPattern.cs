// ----------------------------------------------------------------------
// <copyright file="IMyObjectSubStringLocalizerPattern.cs" company="Xavier Solau">
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
    /// Code first sub string localizer definition pattern.
    /// </summary>
    public interface IMyObjectSubStringLocalizerPattern : IStringLocalizer
    {
        /// <summary>
        /// Get Sub localized argument.
        /// </summary>
        [Pattern<LocalizerArgumentSelector>]
        string SomeArgument { get; }

        /// <summary>
        /// Get MyObjectSub2StringLocalizerProperty Sub string localizer.
        /// </summary>
        [Pattern<SubLocalizerPropertySelector>]
        IMyObjectSub2StringLocalizerPattern MyObjectSub2StringLocalizerProperty { get; }

        /// <summary>
        /// Get SomeSubProperty localized string.
        /// </summary>
        [Pattern<LocalizerPropertySelector>]
        string SomeSubProperty { get; }

        /// <summary>
        /// Get SomeSubStringArgs localized string.
        /// </summary>
        string SomeSubStringArgs(object someParameter);
    }
}
