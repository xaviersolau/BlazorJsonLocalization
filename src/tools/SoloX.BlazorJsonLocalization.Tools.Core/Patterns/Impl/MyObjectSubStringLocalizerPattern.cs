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
    [Pattern(typeof(AttributeSelector<SubLocalizerAttribute>))]
    [Repeat(Pattern = nameof(IMyObjectSubStringLocalizerPattern), Prefix = "I")]
    public class MyObjectSubStringLocalizerPattern : IMyObjectSubStringLocalizerPattern
    {
        private readonly IStringLocalizer stringLocalizer;

        /// <summary>
        /// Setup instance.
        /// </summary>
        /// <param name="stringLocalizer">Base string localizer.</param>
        public MyObjectSubStringLocalizerPattern(IStringLocalizer stringLocalizer)
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
        /// Get MyObjectSub2StringLocalizerProperty Sub string localizer.
        /// </summary>
        [Repeat(Pattern = nameof(IMyObjectSubStringLocalizerPattern.MyObjectSub2StringLocalizerProperty), Prefix = "I")]
        public IMyObjectSub2StringLocalizerPattern MyObjectSub2StringLocalizerProperty
        {
            get => new MyObjectSub2StringLocalizerPattern(this.stringLocalizer.GetSubLocalizer(nameof(MyObjectSub2StringLocalizerProperty)));
        }

        /// <summary>
        /// Get SomeSubProperty localized string.
        /// </summary>
        [Repeat(Pattern = nameof(IMyObjectSubStringLocalizerPattern.SomeSubProperty))]
        public string SomeSubProperty
        {
            get => this.stringLocalizer[nameof(SomeSubProperty)];
        }

        /// <summary>
        /// Get SomeSubStringArgs localized string.
        /// </summary>
        [Repeat(Pattern = nameof(IMyObjectSubStringLocalizerPattern.SomeSubStringArgs))]
        public string SomeSubStringArgs([Repeat(Pattern = "someParameter")] object someParameter)
        {
            return this.stringLocalizer[nameof(SomeSubStringArgs), someParameter];
        }
    }
}
