// ----------------------------------------------------------------------
// <copyright file="MyObjectStringLocalizerPattern.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

#pragma warning disable IDE0005 // Using directive is unnecessary.
using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization;
using SoloX.BlazorJsonLocalization.Attributes;
using SoloX.BlazorJsonLocalization.Tools.Core.Handlers;
using SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Itf;
using SoloX.GeneratorTools.Core.CSharp.Generator;
using SoloX.GeneratorTools.Core.CSharp.Generator.Attributes;
using SoloX.GeneratorTools.Core.CSharp.Generator.Selectors;
using System.Collections.Generic;
#pragma warning restore IDE0005 // Using directive is unnecessary.

namespace SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Impl
{
    /// <summary>
    /// IMyObjectStringLocalizerPattern generated implementation.
    /// </summary>
    [Pattern<AttributeSelector<LocalizerAttribute>>]
    [Repeat(Pattern = nameof(IMyObjectStringLocalizerPattern), Prefix = "I")]
    [ReplacePattern(typeof(TypeReplaceHandler))]
    public partial class MyObjectStringLocalizerPattern : IMyObjectStringLocalizerPattern
    {
        private readonly IStringLocalizer<MyObject> stringLocalizer;

        /// <summary>
        /// Setup instance.
        /// </summary>
        /// <param name="stringLocalizer">Base string localizer.</param>
        public MyObjectStringLocalizerPattern(IStringLocalizer<MyObject> stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
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
        /// Get MyObjectSubStringLocalizerProperty Sub string localizer.
        /// </summary>
        [Repeat(Pattern = nameof(IMyObjectStringLocalizerPattern.MyObjectSubStringLocalizerProperty), Prefix = "I")]
        public IMyObjectSubStringLocalizerPattern MyObjectSubStringLocalizerProperty
        {
            get => new MyObjectSubStringLocalizerPattern(this.stringLocalizer, this.stringLocalizer.GetSubLocalizer(nameof(MyObjectSubStringLocalizerProperty)));
        }

        /// <summary>
        /// Get SomeProperty localized string.
        /// </summary>
        [Repeat(Pattern = nameof(IMyObjectStringLocalizerPattern.SomeProperty))]
        public string SomeProperty
        {
            get => this.stringLocalizer[nameof(SomeProperty)];
        }

        /// <summary>
        /// Get SomeStringArgs localized string.
        /// </summary>
        [Repeat(Pattern = nameof(IMyObjectStringLocalizerPattern.SomeStringArgs))]
        public string SomeStringArgs([Repeat(Pattern = nameof(someParameter))] object someParameter)
            => this.stringLocalizer[nameof(SomeStringArgs), Repeat.Argument(nameof(someParameter), someParameter)];
    }


}
