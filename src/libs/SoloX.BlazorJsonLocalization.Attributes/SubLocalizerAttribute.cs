// ----------------------------------------------------------------------
// <copyright file="SubLocalizerAttribute.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;

namespace SoloX.BlazorJsonLocalization.Attributes
{
    /// <summary>
    /// Sub-localizer attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public sealed class SubLocalizerAttribute : Attribute
    {
        /// <summary>
        /// Setup attribute. (no parameters)
        /// </summary>
        public SubLocalizerAttribute()
        {
        }
    }
}
