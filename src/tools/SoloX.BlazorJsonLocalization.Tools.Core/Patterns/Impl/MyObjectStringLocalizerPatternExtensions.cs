using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Attributes;
using SoloX.BlazorJsonLocalization.Tools.Core.Handlers;
using SoloX.BlazorJsonLocalization.Tools.Core.Selectors;
using SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Itf;
using SoloX.GeneratorTools.Core.CSharp.Generator.Attributes;
using SoloX.GeneratorTools.Core.CSharp.Generator.Selectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Impl
{
    [Pattern(typeof(AttributeSelector<LocalizerAttribute>))]
    [Repeat(Pattern = nameof(IMyObjectStringLocalizerPattern), Prefix = "I")]
    [ReplacePattern(typeof(TypeReplaceHandler))]
    public static class MyObjectStringLocalizerPatternExtensions
    {
        public static IMyObjectStringLocalizerPattern ToMyObjectStringLocalizerPattern(this IStringLocalizer<MyObject> stringLocalizer)
            => new MyObjectStringLocalizerPattern(stringLocalizer);

        [Repeat(Pattern = nameof(IMyObjectStringLocalizerPattern.SomeProperty))]
        public static string SomeProperty(this IStringLocalizer<MyObject> stringLocalizer)
        {
            return stringLocalizer[nameof(SomeProperty)];
        }

        [Repeat(Pattern = nameof(IMyObjectStringLocalizerPattern.SomeStringArgs))]
        public static string SomeStringArgs(this IStringLocalizer<MyObject> stringLocalizer, [Repeat(Pattern = "someParameter")] object someParameter)
        {
            return stringLocalizer[nameof(SomeStringArgs), someParameter];
        }
    }
}
