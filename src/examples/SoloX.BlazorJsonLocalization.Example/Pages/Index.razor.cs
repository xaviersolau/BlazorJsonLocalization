using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoloX.BlazorJsonLocalization.Example.Pages
{
    public partial class Index
    {
        [Inject]
        private IStringLocalizer<Index> L { get; set; }
    }
}
