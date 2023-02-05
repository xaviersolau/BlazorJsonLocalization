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
    public class MyObjectStringLocalizerPattern : IMyObjectStringLocalizerPattern
    {
        private IStringLocalizer<MyObject> stringLocalizer;

        public MyObjectStringLocalizerPattern(IStringLocalizer<MyObject> stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
        }

        public LocalizedString this[string name]
            => this.stringLocalizer[name];

        public LocalizedString this[string name, params object[] arguments]
            => this.stringLocalizer[name, arguments];

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
            => this.stringLocalizer.GetAllStrings(includeParentCultures);

        [Repeat(Pattern = nameof(IMyObjectStringLocalizerPattern.MyObjectSubStringLocalizerProperty), Prefix = "I")]
        public IMyObjectSubStringLocalizerPattern MyObjectSubStringLocalizerProperty
        {
            get => new MyObjectSubStringLocalizerPattern(this.stringLocalizer.GetSubLocalizer(nameof(MyObjectSubStringLocalizerProperty)));
        }

        [Repeat(Pattern = nameof(IMyObjectStringLocalizerPattern.SomeProperty))]
        public string SomeProperty
        {
            get => this.stringLocalizer[nameof(SomeProperty)];
        }

        [Repeat(Pattern = nameof(IMyObjectStringLocalizerPattern.SomeStringArgs))]
        public string SomeStringArgs([Repeat(Pattern = "someParameter")] object someParameter)
            => this.stringLocalizer[nameof(SomeStringArgs), someParameter];
    }


}
