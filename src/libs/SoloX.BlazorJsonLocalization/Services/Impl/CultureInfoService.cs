using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SoloX.BlazorJsonLocalization.Services.Impl
{
    public class CultureInfoService : ICultureInfoService
    {
        public CultureInfo CurrentUICulture => CultureInfo.CurrentUICulture;
    }
}
