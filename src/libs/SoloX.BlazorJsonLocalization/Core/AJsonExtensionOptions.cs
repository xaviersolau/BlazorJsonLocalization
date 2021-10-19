// ----------------------------------------------------------------------
// <copyright file="AJsonExtensionOptions.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Text.Json;

namespace SoloX.BlazorJsonLocalization.Core
{
    /// <summary>
    /// Base class for extension options using Json serialization.
    /// </summary>
    public abstract class AJsonExtensionOptions : AExtensionOptions
    {
        /// <summary>
        /// Gets/Sets JsonSerializer custom options.
        /// </summary>
        public JsonSerializerOptions? JsonSerializerOptions { get; set; }
    }
}
