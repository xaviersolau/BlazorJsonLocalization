// ----------------------------------------------------------------------
// <copyright file="MyObjectSub2StringLocalizerPattern.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Itf;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Impl
{
    internal class MyObjectSub2StringLocalizerPattern : IMyObjectSub2StringLocalizerPattern
    {
        private readonly IStringLocalizer stringLocalizer;

        public MyObjectSub2StringLocalizerPattern(IStringLocalizer stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
        }
    }
}
