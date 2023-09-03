// ----------------------------------------------------------------------
// <copyright file="LocalizerAttribute.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;

namespace SoloX.BlazorJsonLocalization.Attributes
{
    /// <summary>
    /// Localizer attribute used for localization helper and data files.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public sealed class LocalizerAttribute : Attribute
    {
        /// <summary>
        /// Setup attribute with target path for Json files and the language list of files to generate.
        /// </summary>
        /// <param name="path">Target path where to generate the Json files.</param>
        /// <param name="languages">The language list of files to generate</param>
        public LocalizerAttribute(string path, string[] languages)
        {
            Path = path;
            Languages = languages;
        }

        /// <summary>
        /// Languages to generate.
        /// </summary>
        public string[] Languages { get; }

        /// <summary>
        /// Path where to generate Json files.
        /// </summary>
        public string Path { get; }
    }
}
