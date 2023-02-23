// ----------------------------------------------------------------------
// <copyright file="SelectorResolver.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.GeneratorTools.Core.CSharp.Generator.Selectors;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Selectors
{
    internal class SelectorResolver : ISelectorResolver
    {
        public ISelector? GetSelector(string selectorName)
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
