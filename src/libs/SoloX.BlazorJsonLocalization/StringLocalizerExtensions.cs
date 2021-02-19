using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace SoloX.BlazorJsonLocalization
{
    public static class StringLocalizerExtensions
    {
        public static MarkupString Html<T>(this IStringLocalizer<T> localizer, string key, params object[] arguments)
        {
            return new MarkupString(localizer[key, arguments]);
        }
    }
}
