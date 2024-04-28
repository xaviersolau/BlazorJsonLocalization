// ----------------------------------------------------------------------
// <copyright file="ISimpleLocalizer.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Attributes;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Samples.SampleWithSubLocalizerTranslate
{
    [Localizer("Resources", new[] { "fr", "en" })]
    public interface ISimpleLocalizer : IStringLocalizer<Component>
    {
        [TranslateSub(nameof(ISimpleSubLocalizer.Arg1), "Argument 1 translation from SubLocalizer1")]
        [TranslateSub(nameof(ISimpleSubLocalizer.Arg2), "Argument 2 translation from SubLocalizer1")]
        public ISimpleSubLocalizer SubLocalizer1 { get; }

        [TranslateSub(nameof(ISimpleSubLocalizer.Arg1), "Argument 1 translation from SubLocalizer2")]
        [TranslateSub(nameof(ISimpleSubLocalizer.Arg2), "Argument 2 translation from SubLocalizer2")]
        public ISimpleSubLocalizer SubLocalizer2 { get; }

        [Translate("Basic property translation.")]
        string BasicProperty { get; }
    }
}
