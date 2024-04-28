// ----------------------------------------------------------------------
// <copyright file="ISimpleSubLocalizer.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Attributes;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Samples.SampleWithSubLocalizerTranslate
{
    [SubLocalizer]
    public interface ISimpleSubLocalizer : IStringLocalizer
    {
        [TranslateArg("default arg1")]
        string Arg1 { get; }

        [TranslateArg("default arg2")]
        string Arg2 { get; }

        [Translate($"Basic sub property 1 translation. {nameof(Arg1)}")]
        string BasicSubProperty1 { get; }

        [Translate($"Basic sub property 2 translation. {nameof(Arg2)}")]
        string BasicSubProperty2 { get; }
    }
}
