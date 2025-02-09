// ----------------------------------------------------------------------
// <copyright file="StringLocalizerResourceSource.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Reflection;

namespace SoloX.BlazorJsonLocalization.Core.Impl
{
    /// <summary>
    /// StringLocalizerResourceSource record to tell where the localizer is loading its resources from.
    /// </summary>
    public class StringLocalizerResourceSource
    {
        /// <summary>
        /// Setup instance.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="assembly"></param>
        /// <param name="resourceSourceType"></param>
        public StringLocalizerResourceSource(string baseName, Assembly assembly, Type? resourceSourceType)
        {
            BaseName = baseName;
            Assembly = assembly;
            ResourceSourceType = resourceSourceType;
        }

        /// <summary>
        /// Base name of the localizer.
        /// </summary>
        public string BaseName { get; }

        /// <summary>
        /// Assembly where to load data from.
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        /// Type associated to the localizer.
        /// </summary>
        public Type? ResourceSourceType { get; }
    }
}
