using Microsoft.CodeAnalysis.CSharp.Syntax;
using SoloX.GeneratorTools.Core.CSharp.Generator.Selectors;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Selectors
{
    internal class SelectorResolver : ISelectorResolver
    {
        public ISelector GetSelector(string selectorName)
        {
            if (selectorName == typeof(SubLocalizerPropertySelector).FullName)
            {
                return new SubLocalizerPropertySelector();
            }
            else if (selectorName == typeof(StringPropertySelector).FullName)
            {
                return new StringPropertySelector();
            }
            return null;
        }
    }
}
