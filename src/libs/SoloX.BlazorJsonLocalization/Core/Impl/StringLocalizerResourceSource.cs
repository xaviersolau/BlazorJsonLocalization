// ----------------------------------------------------------------------
// <copyright file="StringLocalizerResourceSource.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;

namespace SoloX.BlazorJsonLocalization.Core.Impl
{
    /// <summary>
    /// StringLocalizerResourceSource record to tell where the localizer is loading its resources from.
    /// </summary>
    public class StringLocalizerResourceSource
    {
        private readonly Lazy<IEnumerable<StringLocalizerResourceSource>> lazyParent;

        /// <summary>
        /// Setup instance.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="assembly"></param>
        /// <param name="resourceSourceType"></param>
        /// <param name="localizerHierarchyLoader">Func to load ResourceSource localizer hierarchy.</param>
        public StringLocalizerResourceSource(string baseName, Assembly assembly, Type? resourceSourceType, Func<StringLocalizerResourceSource, IEnumerable<StringLocalizerResourceSource>>? localizerHierarchyLoader = null)
        {
            BaseName = baseName;
            Assembly = assembly;
            ResourceSourceType = resourceSourceType;

            this.lazyParent = new Lazy<IEnumerable<StringLocalizerResourceSource>>(() => localizerHierarchyLoader != null ? localizerHierarchyLoader(this) : []);
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

        /// <summary>
        /// Get ResourceSource localizer hierarchy parent.
        /// </summary>
        public IEnumerable<StringLocalizerResourceSource> Parent
        {
            get
            {
                return this.lazyParent.Value;
            }
        }
    }
}
