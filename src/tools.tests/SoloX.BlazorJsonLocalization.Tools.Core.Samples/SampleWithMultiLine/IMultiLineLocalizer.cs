// ----------------------------------------------------------------------
// <copyright file="ISimpleLocalizer.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Attributes;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Samples.SampleWithMultiLine
{
    [Localizer("Resources", new[] { "fr", "en" })]
    public interface IMultiLineLocalizer : IStringLocalizer<Component>
    {
        [Translate(@"
            Basic property
            translation with
            multi lines.")]
        string BasicProperty { get; }
    }
}
