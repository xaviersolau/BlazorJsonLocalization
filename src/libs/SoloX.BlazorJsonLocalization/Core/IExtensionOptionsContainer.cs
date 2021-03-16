// ----------------------------------------------------------------------
// <copyright file="IExtensionOptionsContainer.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;

namespace SoloX.BlazorJsonLocalization.Core
{
    /// <summary>
    /// Json localization extension options container.
    /// </summary>
    public interface IExtensionOptionsContainer
    {
        /// <summary>
        /// Get the extension options type.
        /// </summary>
        Type ExtensionOptionsType { get; }

        /// <summary>
        /// Get the extension service type.
        /// </summary>
        Type ExtensionServiceType { get; }
    }
}
