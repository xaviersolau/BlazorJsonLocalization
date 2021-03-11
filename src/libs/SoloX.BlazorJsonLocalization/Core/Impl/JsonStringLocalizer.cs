using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;

namespace SoloX.BlazorJsonLocalization.Core.Impl
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private IReadOnlyDictionary<string, string> _stringMap;

        public JsonStringLocalizer(IReadOnlyDictionary<string, string> stringMap)
        {
            this._stringMap = stringMap;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return _stringMap.Select(s => new LocalizedString(s.Key, s.Value));
        }

        public LocalizedString this[string name]
            => BuildLocalizedString(name, s => s);

        public LocalizedString this[string name, params object[] arguments]
            => BuildLocalizedString(name, s => string.Format(s, arguments));

        private LocalizedString BuildLocalizedString(string name, Func<string, string> format)
        {
            if (_stringMap.TryGetValue(name, out var value))
            {
                return new (name, format(value));
            }
            else
            {
                return new (name, format(name), true);
            }
        }
    }
}
