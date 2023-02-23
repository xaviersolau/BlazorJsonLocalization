// ----------------------------------------------------------------------
// <copyright file="MyObjectStringLocalizerPatternExtensions.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Attributes;
using SoloX.BlazorJsonLocalization.Tools.Core.Handlers;
using SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Itf;
using SoloX.GeneratorTools.Core.CSharp.Generator.Attributes;
using SoloX.GeneratorTools.Core.CSharp.Generator.Selectors;
using System;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Impl
{
    /// <summary>
    /// MyObjectStringLocalizerPatternExtensions generated extension methods.
    /// </summary>
    [Pattern(typeof(AttributeSelector<LocalizerAttribute>))]
    [Repeat(Pattern = nameof(IMyObjectStringLocalizerPattern), Prefix = "I")]
    [ReplacePattern(typeof(TypeReplaceHandler))]
    public static class MyObjectStringLocalizerPatternExtensions
    {
        /// <summary>
        /// Convert StringLocalizer to IMyObjectStringLocalizerPattern
        /// </summary>
        /// <param name="stringLocalizer">Base string localizer.</param>
        /// <returns>The localizer instance.</returns>
        public static IMyObjectStringLocalizerPattern ToMyObjectStringLocalizerPattern(this IStringLocalizer<MyObject> stringLocalizer)
            => new MyObjectStringLocalizerPattern(stringLocalizer);

        /// <summary>
        /// Get SomeProperty localized value from the String Localizer.
        /// </summary>
        /// <param name="stringLocalizer">String localizer to get the value from.</param>
        /// <returns>The localized value.</returns>
        [Repeat(Pattern = nameof(IMyObjectStringLocalizerPattern.SomeProperty))]
        public static string SomeProperty(this IStringLocalizer<MyObject> stringLocalizer)
        {
            if (stringLocalizer == null)
            {
                throw new ArgumentNullException(nameof(stringLocalizer));
            }

            return stringLocalizer[nameof(SomeProperty)];
        }

        /// <summary>
        /// Get SomeStringArgs localized value from the String Localizer.
        /// </summary>
        /// <param name="stringLocalizer">String localizer to get the value from.</param>
        /// <returns>The localized value.</returns>
        [Repeat(Pattern = nameof(IMyObjectStringLocalizerPattern.SomeStringArgs))]
        public static string SomeStringArgs(this IStringLocalizer<MyObject> stringLocalizer, [Repeat(Pattern = "someParameter")] object someParameter)
        {
            if (stringLocalizer == null)
            {
                throw new ArgumentNullException(nameof(stringLocalizer));
            }

            return stringLocalizer[nameof(SomeStringArgs), someParameter];
        }
    }
}
