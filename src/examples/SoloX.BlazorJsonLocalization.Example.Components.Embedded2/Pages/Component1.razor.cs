using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace An.Other.Name.Embedded.Pages
{
    public partial class Component1
    {
        [Inject]
        private IStringLocalizer<Component1> L { get; set; }
    }
}
