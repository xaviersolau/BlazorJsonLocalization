using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SoloX.BlazorJsonLocalization.Services
{
    public interface ICultureInfoService
    {
        CultureInfo CurrentUICulture { get; }
    }
}
