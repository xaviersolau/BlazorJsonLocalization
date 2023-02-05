using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Localization;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Impl
{
    internal static class LocalizerExtensions
    {
        public static IStringLocalizer GetSubLocalizer(this IStringLocalizer localizer, params string[] structuredKey)
        {
            throw new NotImplementedException();
        }
    }
}
