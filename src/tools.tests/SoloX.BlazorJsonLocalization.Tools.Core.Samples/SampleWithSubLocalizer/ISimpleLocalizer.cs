// ----------------------------------------------------------------------
// <copyright file="ISimpleLocalizer.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Attributes;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Samples.SampleWithSubLocalizer
{
    [Localizer("Resources", new[] { "fr", "en" })]
    public interface ISimpleLocalizer : IStringLocalizer<Component>
    {
        public ISimpleSubLocalizer SubLocalizer1 { get; }

        public ISimpleSubLocalizer SubLocalizer2 { get; }

        string BasicProperty { get; }
    }
}
