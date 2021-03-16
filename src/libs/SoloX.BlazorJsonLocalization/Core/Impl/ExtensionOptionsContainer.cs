// ----------------------------------------------------------------------
// <copyright file="ExtensionOptionsContainer.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.BlazorJsonLocalization.Services;
using System;

namespace SoloX.BlazorJsonLocalization.Core.Impl
{
    /// <summary>
    /// Typed JsonLocalisation extension options.
    /// </summary>
    /// <typeparam name="TOptions">The extension options type.</typeparam>
    public sealed class ExtensionOptionsContainer<TOptions> : IExtensionOptionsContainer
    {
        /// <summary>
        /// Setup the ExtensionOptionsContainer with the given options.
        /// </summary>
        /// <param name="options">The extension options</param>
        public ExtensionOptionsContainer(TOptions options)
        {
            Options = options;
        }

        ///<inheritdoc/>
        public Type ExtensionOptionsType => typeof(TOptions);

        ///<inheritdoc/>
        public Type ExtensionServiceType => typeof(IJsonLocalizationExtensionService<TOptions>);

        /// <summary>
        /// Get the typed extension options.
        /// </summary>
        public TOptions Options { get; }
    }
}
