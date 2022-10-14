// ----------------------------------------------------------------------
// <copyright file="NameHelper.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;

namespace SoloX.BlazorJsonLocalization.Helpers
{
    internal static class NameHelper
    {
        internal static string GetBaseName(this Type type)
        {
            return type.FullName ?? type.Name;
        }
    }
}
