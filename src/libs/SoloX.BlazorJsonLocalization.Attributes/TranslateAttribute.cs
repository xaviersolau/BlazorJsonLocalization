// ----------------------------------------------------------------------
// <copyright file="TranslateAttribute.cs" company="Xavier Solau">
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
    public sealed class TranslateAttribute : Attribute
    {
        /// <summary>
        /// Setup translation attribute.
        /// </summary>
        /// <param name="translation">Translation to use.</param>
        public TranslateAttribute(string translation)
        {
            Translation = translation;
        }

        /// <summary>
        /// Translation to use.
        /// </summary>
        public string Translation { get; }
    }
}
