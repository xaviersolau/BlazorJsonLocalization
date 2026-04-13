// ----------------------------------------------------------------------
// <copyright file="ISimpleLocalizer.cs" company="Xavier Solau">
// Copyright © 2021-2026 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Attributes;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Samples.SampleMethodArg
{
    [Localizer("Resources", new[] { "fr", "en" })]
    public interface ISimpleLocalizer : IStringLocalizer<Component>
    {
        string MethodWithArgString(string arg);
        string MethodWithArgInt(int arg);
        string MethodWithArgDouble(double arg);
        string MethodWithArgDateTime(DateTime arg);
        string MethodWithArgDateOnly(DateOnly arg);
    }
}
