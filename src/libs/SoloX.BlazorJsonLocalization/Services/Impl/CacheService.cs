// ----------------------------------------------------------------------
// <copyright file="CacheService.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace SoloX.BlazorJsonLocalization.Services.Impl
{
    /// <summary>
    /// Cache service implementation.
    /// </summary>
    public class CacheService : ICacheService
    {
        private readonly Dictionary<string, IStringLocalizer> cacheMap = new Dictionary<string, IStringLocalizer>();

        ///<inheritdoc/>
        public void Cache(Assembly assembly, string baseName, CultureInfo cultureInfo, IStringLocalizer localizer)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }
            if (cultureInfo == null)
            {
                throw new ArgumentNullException(nameof(cultureInfo));
            }
            if (baseName == null)
            {
                throw new ArgumentNullException(nameof(baseName));
            }
            if (localizer == null)
            {
                throw new ArgumentNullException(nameof(localizer));
            }

            lock (this.cacheMap)
            {
                this.cacheMap.Add(ComputeKey(assembly, baseName, cultureInfo), localizer);
            }
        }

        ///<inheritdoc/>
        public IStringLocalizer? Match(Assembly assembly, string baseName, CultureInfo cultureInfo)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }
            if (cultureInfo == null)
            {
                throw new ArgumentNullException(nameof(cultureInfo));
            }
            if (baseName == null)
            {
                throw new ArgumentNullException(nameof(baseName));
            }

            lock (this.cacheMap)
            {
                return this.cacheMap.TryGetValue(ComputeKey(assembly, baseName, cultureInfo), out var entry) ? entry : null;
            }
        }

        private static string ComputeKey(Assembly assembly, string baseName, CultureInfo cultureInfo)
        {
            return $"{assembly.GetName().Name}-{baseName}-{cultureInfo.Name}";
        }
    }
}
