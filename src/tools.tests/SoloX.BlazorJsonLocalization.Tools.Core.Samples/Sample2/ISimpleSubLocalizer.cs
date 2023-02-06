// ----------------------------------------------------------------------
// <copyright file="ISimpleSubLocalizer.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Attributes;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Samples.Sample2
{
    [SubLocalizer]
    public interface ISimpleSubLocalizer : IStringLocalizer
    {
        string BasicSubProperty1 { get; }
        string BasicSubProperty2 { get; }
    }
}
