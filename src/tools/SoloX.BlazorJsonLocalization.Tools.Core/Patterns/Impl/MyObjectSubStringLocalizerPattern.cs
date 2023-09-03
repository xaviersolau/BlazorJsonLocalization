// ----------------------------------------------------------------------
// <copyright file="MyObjectSubStringLocalizerPattern.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Attributes;
using SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Itf;
using SoloX.GeneratorTools.Core.CSharp.Generator.Attributes;
using SoloX.GeneratorTools.Core.CSharp.Generator.Selectors;
using System.Collections.Generic;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Impl
{
    /// <summary>
    /// IMyObjectSubStringLocalizerPattern generated implementation.
    /// </summary>
    [Pattern<AttributeSelector<SubLocalizerAttribute>>]
    [Repeat(Pattern = nameof(IMyObjectSubStringLocalizerPattern), Prefix = "I")]
    public class MyObjectSubStringLocalizerPattern : IMyObjectSubStringLocalizerPattern
    {
        private readonly IStringLocalizer stringLocalizer;
        private readonly IStringLocalizer argumentStringLocalizer;

        /// <summary>
        /// Setup instance.
        /// </summary>
        /// <param name="stringLocalizer">Base string localizer.</param>
        /// <param name="argumentStringLocalizer">argument String Localizer</param>
        public MyObjectSubStringLocalizerPattern(IStringLocalizer stringLocalizer, IStringLocalizer argumentStringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            this.argumentStringLocalizer = argumentStringLocalizer;
        }

        /// <inheritdoc/>
        public LocalizedString this[string name]
            => this.stringLocalizer[name];

        /// <inheritdoc/>
        public LocalizedString this[string name, params object[] arguments]
            => this.stringLocalizer[name, arguments];

        /// <inheritdoc/>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
            => this.stringLocalizer.GetAllStrings(includeParentCultures);

        /// <summary>
        /// Get MyObjectSub2StringLocalizerProperty Sub string localizer.
        /// </summary>
        [Repeat(Pattern = nameof(IMyObjectSubStringLocalizerPattern.MyObjectSub2StringLocalizerProperty), Prefix = "I")]
        public IMyObjectSub2StringLocalizerPattern MyObjectSub2StringLocalizerProperty
        {
            get => new MyObjectSub2StringLocalizerPattern(this.stringLocalizer, this.argumentStringLocalizer.GetSubLocalizer(nameof(MyObjectSub2StringLocalizerProperty)));
        }

        /// <summary>
        /// Get SomeArgument localized string.
        /// </summary>
        [Repeat(Pattern = nameof(IMyObjectSubStringLocalizerPattern.SomeArgument))]
        public string SomeArgument
        {
            get => this.argumentStringLocalizer[nameof(SomeArgument)] ?? this.stringLocalizer[nameof(SomeArgument)];
        }

        /// <summary>
        /// Get SomeSubProperty localized string.
        /// </summary>
        [Repeat(Pattern = nameof(IMyObjectSubStringLocalizerPattern.SomeSubProperty))]
        public string SomeSubProperty => GetLocalizerValue(nameof(SomeSubProperty));

        /// <summary>
        /// Get SomeSubStringArgs localized string.
        /// </summary>
        [Repeat(Pattern = nameof(IMyObjectSubStringLocalizerPattern.SomeSubStringArgs))]
        public string SomeSubStringArgs([Repeat(Pattern = "someParameter")] object someParameter)
            => GetLocalizerValue(nameof(SomeSubStringArgs), someParameter);

        private string GetLocalizerValue(string name, params object[] arguments)
        {
            var txt = this.stringLocalizer[name, arguments];

            return ProcessLocalizerArgument(txt);
        }

        [RepeatStatements(Pattern = nameof(IMyObjectSubStringLocalizerPattern.SomeArgument))]
        private string ProcessLocalizerArgument(string txt)
        {
            if (string.IsNullOrEmpty(txt))
            {
                return txt;
            }

            txt = txt.Replace(nameof(SomeArgument), SomeArgument ?? string.Empty);

            return txt;
        }
    }
}
