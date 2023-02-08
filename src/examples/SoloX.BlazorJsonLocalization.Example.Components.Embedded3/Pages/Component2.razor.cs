using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloX.BlazorJsonLocalization.Example.Components.Embedded3.Pages
{
    public partial class Component2
    {
        [Inject]
        private IStringLocalizer<Component2> L { get; set; }
    }
}
