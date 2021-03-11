using SoloX.BlazorJsonLocalization.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoloX.BlazorJsonLocalization.Core
{
    public interface IJsonLocalizationOptionsExtension
    {
        public IJsonLocalizationService GetJsonLocalizationService(IServiceProvider serviceProvider);
    }
}
