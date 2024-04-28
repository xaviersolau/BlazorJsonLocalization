// ----------------------------------------------------------------------
// <copyright file="ISimpleLocalizer.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Attributes;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Samples.SampleWithTranslate
{
    [Localizer("Resources", new[] { "fr", "en" })]
    public interface ISimpleLocalizer : IStringLocalizer<Component>
    {
        [Translate(@"Basic property translation with special <chars>.")]
        string BasicProperty { get; }
    }
}
