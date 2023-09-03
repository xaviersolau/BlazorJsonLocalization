// ----------------------------------------------------------------------
// <copyright file="TranslateSubAttribute.cs" company="Xavier Solau">
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
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class TranslateSubAttribute : Attribute
    {
        /// <summary>
        /// Setup translation attribute.
        /// </summary>
        /// <param name="argumentName">Translate argument name.</param>
        /// <param name="translation">Translation to use.</param>
        public TranslateSubAttribute(string argumentName, string translation)
        {
            Translation = translation;
            ArgumentName = argumentName;
        }

        /// <summary>
        /// Sub-property path.
        /// </summary>
        public string ArgumentName { get; }

        /// <summary>
        /// Translation to use.
        /// </summary>
        public string Translation { get; }
    }
}
