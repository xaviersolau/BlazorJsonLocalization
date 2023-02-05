using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Itf;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Impl
{
    internal class MyObjectSub2StringLocalizerPattern : IMyObjectSub2StringLocalizerPattern
    {
        private IStringLocalizer stringLocalizer;

        public MyObjectSub2StringLocalizerPattern(IStringLocalizer stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
        }
    }
}
