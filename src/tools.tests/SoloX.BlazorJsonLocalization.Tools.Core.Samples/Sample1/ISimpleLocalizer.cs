// ----------------------------------------------------------------------
// <copyright file="ISimpleLocalizer.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Attributes;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Samples.Sample1
{
    [Localizer("Resources", new[] { "fr", "en" })]
    public interface ISimpleLocalizer : IStringLocalizer<Component>
    {
        string BasicProperty { get; }
        string BasicMethod();
        string BasicMethodArg(string arg);
        string BasicMethodArg1Arg2(string arg1, string arg2);
    }
}
