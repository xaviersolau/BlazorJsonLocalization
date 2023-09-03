// ----------------------------------------------------------------------
// <copyright file="TranslateArgAttribute.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;

namespace SoloX.BlazorJsonLocalization.Attributes
{
    /// <summary>
    /// Translate attribute to define translation from code.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class TranslateArgAttribute : Attribute
    {
        /// <summary>
        /// Setup translation attribute.
        /// </summary>
        public TranslateArgAttribute()
        {
            Translation = string.Empty;
        }

        /// <summary>
        /// Setup translation attribute.
        /// </summary>
        /// <param name="translation">Translation to use by default.</param>
        public TranslateArgAttribute(string translation)
        {
            Translation = translation;
        }

        /// <summary>
        /// Translation to use.
        /// </summary>
        public string Translation { get; }
    }
}
