// ----------------------------------------------------------------------
// <copyright file="CacheService.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.BlazorJsonLocalization.Core;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

#if NET6_0_OR_GREATER
using System;
#else
using ArgumentNullException = SoloX.BlazorJsonLocalization.Helpers.ArgumentNullExceptionLegacy;
#endif

namespace SoloX.BlazorJsonLocalization.Services.Impl
{
    /// <summary>
    /// Cache service implementation.
    /// </summary>
    public class CacheService : ICacheService
    {
        private readonly Dictionary<string, IStringLocalizerInternal> cacheMap = new Dictionary<string, IStringLocalizerInternal>();

        ///<inheritdoc/>
        public void Cache(Assembly assembly, string baseName, CultureInfo? cultureInfo, IStringLocalizerInternal localizer)
        {
            ArgumentNullException.ThrowIfNull(assembly, nameof(assembly));
            ArgumentNullException.ThrowIfNull(baseName, nameof(baseName));
            ArgumentNullException.ThrowIfNull(localizer, nameof(localizer));

            lock (this.cacheMap)
            {
                this.cacheMap.Add(ComputeKey(assembly, baseName, cultureInfo), localizer);
            }
        }

        ///<inheritdoc/>
        public IStringLocalizerInternal? Match(Assembly assembly, string baseName, CultureInfo? cultureInfo)
        {
            ArgumentNullException.ThrowIfNull(assembly, nameof(assembly));
            ArgumentNullException.ThrowIfNull(baseName, nameof(baseName));

            lock (this.cacheMap)
            {
                return this.cacheMap.TryGetValue(ComputeKey(assembly, baseName, cultureInfo), out var entry) ? entry : null;
            }
        }

        ///<inheritdoc/>
        public bool Reset(Assembly assembly, string baseName, CultureInfo? cultureInfo)
        {
            ArgumentNullException.ThrowIfNull(assembly, nameof(assembly));
            ArgumentNullException.ThrowIfNull(baseName, nameof(baseName));

            lock (this.cacheMap)
            {
                return this.cacheMap.Remove(ComputeKey(assembly, baseName, cultureInfo));
            }
        }

        private static string ComputeKey(Assembly assembly, string baseName, CultureInfo? cultureInfo)
        {
            return cultureInfo == null ? $"{assembly.GetName().Name}-{baseName}" : $"{assembly.GetName().Name}-{baseName}-{cultureInfo.Name}";
        }
    }
}
