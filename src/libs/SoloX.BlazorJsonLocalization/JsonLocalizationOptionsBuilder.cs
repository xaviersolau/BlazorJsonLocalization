// ----------------------------------------------------------------------
// <copyright file="JsonLocalizationOptionsBuilder.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.BlazorJsonLocalization.Core;
using SoloX.BlazorJsonLocalization.Core.Impl;
using System.Collections.Generic;

namespace SoloX.BlazorJsonLocalization
{
    /// <summary>
    /// Json localization options builder.
    /// </summary>
    public sealed class JsonLocalizationOptionsBuilder
    {
        private readonly IList<IJsonLocalizationExtensionOptions> extensionOptions
            = new List<IJsonLocalizationExtensionOptions>();

        /// <summary>
        /// Add extension options.
        /// </summary>
        /// <typeparam name="TOptions">Type of the extension options.</typeparam>
        /// <param name="options">The options to add.</param>
        public void AddExtensionOptions<TOptions>(TOptions options)
        {
            this.extensionOptions.Add(new JsonLocalizationExtensionOptions<TOptions>(options));
        }

        internal void Build(JsonLocalizationOptions opt)
        {
            if (opt != null)
            {
                opt.ExtensionOptions = this.extensionOptions;
            }
        }
    }
}
