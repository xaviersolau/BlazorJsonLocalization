// ----------------------------------------------------------------------
// <copyright file="EmbeddedJsonLocalizationOptions.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.BlazorJsonLocalization.Core;
using System;
using System.Reflection;

namespace SoloX.BlazorJsonLocalization
{
    /// <summary>
    /// JsonLocalizer Embedded extension options
    /// </summary>
    public class EmbeddedJsonLocalizationOptions : AJsonExtensionOptions
    {
        /// <summary>
        /// Gets/Sets Path where to get the resources.
        /// </summary>
        public string ResourcesPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets/Sets Assembly Root NameSpace resolver.
        /// </summary>
        /// <remarks>
        /// By default the root name space will be resolved as the assembly name.
        /// </remarks>
        public Func<Assembly, string?>? RootNameSpaceResolver { get; set; }
    }
}
