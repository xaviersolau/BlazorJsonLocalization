using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoloX.BlazorJsonLocalization.Core.Impl
{
    public class NullStringLocalizer : IStringLocalizer
    {
        public LocalizedString this[string name]
            => new (name, name, true);

        public LocalizedString this[string name, params object[] arguments]
            => new (name, string.Format(name, arguments), true);

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return Array.Empty<LocalizedString>();
        }
    }
}
