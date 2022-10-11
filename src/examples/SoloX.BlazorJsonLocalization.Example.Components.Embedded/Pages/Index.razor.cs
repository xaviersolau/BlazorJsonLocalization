using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Example.Components.SharedLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloX.BlazorJsonLocalization.Example.Components.Embedded.Pages
{
    public partial class Index : IGlobal
    {
        [Inject]
        private IStringLocalizer<Index> L { get; set; }
    }
}
