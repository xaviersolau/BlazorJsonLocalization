using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoloX.BlazorJsonLocalization;

namespace SoloX.BlazorJsonLocalization.Example.Components.StaticAssets.Pages
{
    public partial class Counter
    {
        [Inject]
        private IStringLocalizer<Counter> L { get; set; }

        private int currentCount = 0;

        private void IncrementCount()
        {
            currentCount++;
        }

        protected override async Task OnInitializedAsync()
        {
            await L.LoadAsync();
            await base.OnInitializedAsync();
        }
    }
}
