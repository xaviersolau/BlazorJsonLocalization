using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Attributes;
using SoloX.BlazorJsonLocalization.Tools.Core.Handlers;
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
    [Repeat(RepeatPattern = nameof(IMyObjectStringLocalizerPattern), RepeatPatternPrefix = "I")]
    [ReplacePattern(typeof(TypeReplaceHandler))]
    public static class MyObjectStringLocalizerPatternExtensions
    {
        public static IMyObjectStringLocalizerPattern ToMyObjectStringLocalizer(this IStringLocalizer<MyObject> stringLocalizer)
            => new MyObjectStringLocalizerPattern(stringLocalizer);

        [Repeat(RepeatPattern = nameof(IMyObjectStringLocalizerPattern.SomeProperty))]
        public static string SomeProperty(this IStringLocalizer<MyObject> stringLocalizer)
        {
            return stringLocalizer[nameof(SomeProperty)];
        }

        [Repeat(RepeatPattern = nameof(IMyObjectStringLocalizerPattern.SomeStringArgs))]
        public static string SomeStringArgs(this IStringLocalizer<MyObject> stringLocalizer, [Repeat(RepeatPattern = "someParameter")] object someParameter)
        {
            return stringLocalizer[nameof(SomeStringArgs), someParameter];
        }
    }
}
