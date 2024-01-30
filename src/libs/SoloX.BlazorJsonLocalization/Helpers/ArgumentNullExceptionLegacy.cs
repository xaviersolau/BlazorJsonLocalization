// ----------------------------------------------------------------------
// <copyright file="ArgumentNullExceptionLegacy.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

#if !NET6_0_OR_GREATER

namespace SoloX.BlazorJsonLocalization.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public static class ArgumentNullExceptionLegacy
    {
        /// <summary>
        /// Throw if value is null.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueName"></param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void ThrowIfNull(object value, string valueName)
        {
            if (value == null)
            {
                throw new System.ArgumentNullException(valueName);
            }
        }
    }
}

#endif
