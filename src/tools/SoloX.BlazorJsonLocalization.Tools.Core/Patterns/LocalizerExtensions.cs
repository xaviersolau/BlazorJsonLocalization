using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Localization;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Impl
{
    internal static class LocalizerExtensions
    {
        public static IStringLocalizer GetSubLocalizer(this IStringLocalizer localizer, params string[] structuredKey)
        {
            return new SubStringLocalizer(localizer, string.Join(":", structuredKey) + ":");
        }
    }

    internal class SubStringLocalizer : IStringLocalizer
    {
        private readonly IStringLocalizer localizer;
        private readonly string structuredKeyPrefix;

        /// <summary>
        /// Setup instance.
        /// </summary>
        /// <param name="localizer">Base localizer reference.</param>
        /// <param name="structuredKeyPrefix">Structured key to use as prefix.</param>
        public SubStringLocalizer(IStringLocalizer localizer, string structuredKeyPrefix)
        {
            this.localizer = localizer;
            this.structuredKeyPrefix = structuredKeyPrefix;
        }

        ///<inheritdoc/>
        public LocalizedString this[string name] => this.localizer[this.structuredKeyPrefix + name];

        ///<inheritdoc/>
        public LocalizedString this[string name, params object[] arguments] => this.localizer[this.structuredKeyPrefix + name, arguments];

        ///<inheritdoc/>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return this.localizer.GetAllStrings(includeParentCultures).Where(s => s.Name.StartsWith(this.structuredKeyPrefix, StringComparison.InvariantCulture));
        }
    }
}
