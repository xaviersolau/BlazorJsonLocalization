using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Attributes;
using SoloX.BlazorJsonLocalization.Tools.Core.Handlers;
using SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Itf;
using SoloX.GeneratorTools.Core.CSharp.Generator.Attributes;
using SoloX.GeneratorTools.Core.CSharp.Generator.Selectors;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Impl
{
    [Pattern(typeof(AttributeSelector<SubLocalizerAttribute>))]
    [Repeat(Pattern = nameof(IMyObjectSubStringLocalizerPattern), Prefix = "I")]
    public class MyObjectSubStringLocalizerPattern : IMyObjectSubStringLocalizerPattern
    {
        private IStringLocalizer stringLocalizer;

        public MyObjectSubStringLocalizerPattern(IStringLocalizer stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
        }

        public LocalizedString this[string name]
            => this.stringLocalizer[name];

        public LocalizedString this[string name, params object[] arguments]
            => this.stringLocalizer[name, arguments];

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
            => this.stringLocalizer.GetAllStrings(includeParentCultures);

        [Repeat(Pattern = nameof(IMyObjectSubStringLocalizerPattern.MyObjectSub2StringLocalizerProperty), Prefix = "I")]
        public IMyObjectSub2StringLocalizerPattern MyObjectSub2StringLocalizerProperty
        {
            get => new MyObjectSub2StringLocalizerPattern(this.stringLocalizer.GetSubLocalizer(nameof(MyObjectSub2StringLocalizerProperty)));
        }

        [Repeat(Pattern = nameof(IMyObjectSubStringLocalizerPattern.SomeSubProperty))]
        public string SomeSubProperty
        {
            get => this.stringLocalizer[nameof(SomeSubProperty)];
        }

        [Repeat(Pattern = nameof(IMyObjectSubStringLocalizerPattern.SomeSubStringArgs))]
        public string SomeSubStringArgs([Repeat(Pattern = "someParameter")] object someParameter)
        {
            return this.stringLocalizer[nameof(SomeSubStringArgs), someParameter];
        }
    }
}
