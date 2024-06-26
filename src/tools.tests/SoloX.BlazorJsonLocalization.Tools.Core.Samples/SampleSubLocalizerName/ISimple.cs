﻿// ----------------------------------------------------------------------
// <copyright file="ISimpleLocalizer.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Attributes;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Samples.SampleSubLocalizerName
{
    [Localizer("Resources", new[] { "fr", "en" })]
    public interface ISimple : IStringLocalizer<Component>
    {
        public ISimpleSub SubLocalizer1 { get; }

        public ISimpleSub SubLocalizer2 { get; }

        string BasicProperty { get; }
    }
}
