﻿using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Attributes;
using SoloX.BlazorJsonLocalization.Tools.Core.Handlers;
using SoloX.BlazorJsonLocalization.Tools.Core.Selectors;
using SoloX.GeneratorTools.Core.CSharp.Generator.Attributes;
using SoloX.GeneratorTools.Core.CSharp.Generator.Selectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Itf
{
    public interface IMyObjectStringLocalizerPattern : IStringLocalizer<MyObject>
    {
        [Pattern(typeof(SubLocalizerPropertySelector))]
        IMyObjectSubStringLocalizerPattern MyObjectSubStringLocalizerProperty { get; }

        [Pattern(typeof(StringPropertySelector))]
        string SomeProperty { get; }

        string SomeStringArgs(object someParameter);
    }
}
