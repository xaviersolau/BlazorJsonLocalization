// ----------------------------------------------------------------------
// <copyright file="Key.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;

namespace SoloX.BlazorJsonLocalization
{
    /// <summary>
    /// Localization Key handling class.
    /// </summary>
    public static class Key
    {
        /// <summary>
        /// Key separator use with structured Json object.
        /// </summary>
        public const char Separator = ':';

        /// <summary>
        /// Build a key from the given structure Json path.
        /// </summary>
        /// <param name="structuredKey"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static string Path(params string[] structuredKey)
        {
            if (structuredKey == null)
            {
                throw new ArgumentNullException(nameof(structuredKey));
            }

            if (structuredKey.Length == 0)
            {
                throw new ArgumentException("At least one key must be provided", nameof(structuredKey));
            }

            return string.Join(Separator, structuredKey);
        }
    }
}
