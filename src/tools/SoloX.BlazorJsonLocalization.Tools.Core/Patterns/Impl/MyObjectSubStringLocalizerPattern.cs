// ----------------------------------------------------------------------
// <copyright file="MyObjectSubStringLocalizerPattern.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

#pragma warning disable IDE0005 // Using directive is unnecessary.
using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization;
using SoloX.BlazorJsonLocalization.Attributes;
using SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Itf;
using SoloX.GeneratorTools.Core.CSharp.Generator;
using SoloX.GeneratorTools.Core.CSharp.Generator.Attributes;
using SoloX.GeneratorTools.Core.CSharp.Generator.Selectors;
using System.Collections.Generic;
#pragma warning restore IDE0005 // Using directive is unnecessary.

namespace SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Impl
{
    /// <summary>
    /// IMyObjectSubStringLocalizerPattern generated implementation.
    /// </summary>
    [Pattern<AttributeSelector<SubLocalizerAttribute>>]
    [Repeat(Pattern = nameof(IMyObjectSubStringLocalizerPattern), Prefix = "I")]
    public partial class MyObjectSubStringLocalizerPattern : IMyObjectSubStringLocalizerPattern
    {
        private readonly IStringLocalizer rootLocalizer;
        private readonly IStringLocalizer elementLocalizer;
        private readonly IStringLocalizer argumentStringLocalizer;

        /// <summary>
        /// Setup instance.
        /// </summary>
        /// <param name="rootLocalizer">Base string localizer.</param>
        /// <param name="argumentStringLocalizer">argument String Localizer</param>
        public MyObjectSubStringLocalizerPattern(IStringLocalizer rootLocalizer, IStringLocalizer argumentStringLocalizer)
        {
            this.rootLocalizer = rootLocalizer;
            this.elementLocalizer = rootLocalizer.GetSubLocalizer(typeof(IMyObjectSubStringLocalizerPattern).FullName);
            this.argumentStringLocalizer = argumentStringLocalizer;
        }

        /// <inheritdoc/>
        public LocalizedString this[string name]
            => this.elementLocalizer[name];

        /// <inheritdoc/>
        public LocalizedString this[string name, params object[] arguments]
            => this.elementLocalizer[name, arguments];

        /// <inheritdoc/>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
            => this.elementLocalizer.GetAllStrings(includeParentCultures);

        /// <summary>
        /// Get MyObjectSub2StringLocalizerProperty Sub string localizer.
        /// </summary>
        [Repeat(Pattern = nameof(IMyObjectSubStringLocalizerPattern.MyObjectSub2StringLocalizerProperty), Prefix = "I")]
        public IMyObjectSub2StringLocalizerPattern MyObjectSub2StringLocalizerProperty
        {
            get => new MyObjectSub2StringLocalizerPattern(this.rootLocalizer, this.argumentStringLocalizer.GetSubLocalizer(nameof(MyObjectSub2StringLocalizerProperty)));
        }

        /// <summary>
        /// Get SomeArgument localized string.
        /// </summary>
        [Repeat(Pattern = nameof(IMyObjectSubStringLocalizerPattern.SomeArgument))]
        public string SomeArgument
        {
            get => this.argumentStringLocalizer[nameof(SomeArgument)] ?? this.elementLocalizer[nameof(SomeArgument)];
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
        public string SomeSubStringArgs([Repeat(Pattern = nameof(someParameter))] object someParameter)
            => GetLocalizerValue(nameof(SomeSubStringArgs), Repeat.Argument(nameof(someParameter), someParameter));

        private string GetLocalizerValue(string name, params object[] arguments)
        {
            var txt = this.elementLocalizer[name, arguments];

            return ProcessLocalizerArgument(txt);
        }

        private string ProcessLocalizerArgument(string txt)
        {
            if (string.IsNullOrEmpty(txt))
            {
                return txt;
            }

            Repeat.Statements(nameof(IMyObjectSubStringLocalizerPattern.SomeArgument), () =>
            {
                txt = txt.Replace(nameof(SomeArgument), SomeArgument ?? string.Empty);
            });

            return txt;
        }
    }
}
