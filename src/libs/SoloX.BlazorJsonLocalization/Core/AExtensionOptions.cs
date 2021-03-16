// ----------------------------------------------------------------------
// <copyright file="AExtensionOptions.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace SoloX.BlazorJsonLocalization.Core
{
    /// <summary>
    /// Base class for extension options.
    /// </summary>
    public abstract class AExtensionOptions
    {
        /// <summary>
        /// Assembly names to match when the extension is loading resources. The extension won't apply
        /// on other assemblies except if the list is empty.
        /// </summary>
        public IEnumerable<string> AssemblyNames { get; set; } = Array.Empty<string>();
    }
}
