// ----------------------------------------------------------------------
// <copyright file="JsonStringLocalizerFactory.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SoloX.BlazorJsonLocalization.Helpers;
using SoloX.BlazorJsonLocalization.Helpers.Impl;
using SoloX.BlazorJsonLocalization.Services;

#if !NET6_0_OR_GREATER
using ArgumentNullException = SoloX.BlazorJsonLocalization.Helpers.ArgumentNullExceptionLegacy;
#endif

namespace SoloX.BlazorJsonLocalization.Core.Impl
{
    /// <summary>
    /// IStringLocalizerFactory implementation that will create the IStringLocalizer instance
    /// from the extension configured in the JsonLocalization options.
    /// </summary>
    public sealed class JsonStringLocalizerFactory : IStringLocalizerFactory, IJsonStringLocalizerFactoryInternal
    {
        private readonly JsonLocalizationOptions options;
        private readonly IExtensionResolverService extensionResolverService;
        private readonly ICultureInfoService cultureInfoService;
        private readonly ICacheService cacheService;
        private readonly ILogger<JsonStringLocalizerFactory> logger;
        private readonly ILogger<StringLocalizerProxy> loggerForStringLocalizerProxy;

        /// <summary>
        /// Setup the factory.
        /// </summary>
        /// <param name="options">The JsonLocalization options.</param>
        /// <param name="cultureInfoService">Service providing the current culture.</param>
        /// <param name="extensionResolverService">The service resolver to get the JsonLocalization
        /// extension service.</param>
        /// <param name="cacheService">The service to cache the loaded data.</param>
        /// <param name="logger">Logger where to log processing messages.</param>
        /// <param name="loggerForStringLocalizerProxy">Logger to pass when creating a <see cref="StringLocalizerProxy"/>.</param>
        public JsonStringLocalizerFactory(
            IOptions<JsonLocalizationOptions> options,
            ICultureInfoService cultureInfoService,
            IExtensionResolverService extensionResolverService,
            ICacheService cacheService,
            ILogger<JsonStringLocalizerFactory> logger,
            ILogger<StringLocalizerProxy> loggerForStringLocalizerProxy)
        {
            ArgumentNullException.ThrowIfNull(options, nameof(options));

            this.options = options.Value;
            this.logger = this.options.GetLogger(logger);
            this.loggerForStringLocalizerProxy = this.options.GetLogger(loggerForStringLocalizerProxy);
            this.extensionResolverService = extensionResolverService;
            this.cultureInfoService = cultureInfoService;
            this.cacheService = cacheService;
        }

        ///<inheritdoc/>
        public IStringLocalizer Create(Type resourceSource)
        {
            ArgumentNullException.ThrowIfNull(resourceSource, nameof(resourceSource));

            var assembly = resourceSource.Assembly;
            var baseName = resourceSource.GetBaseName();

            this.logger.CreateStringLocalizer(baseName);

            return CreateStringLocalizerProxy(new StringLocalizerResourceSource(baseName, assembly, resourceSource));
        }

        ///<inheritdoc/>
        public IStringLocalizer Create(string baseName, string location)
        {
            this.logger.CreateStringLocalizerInLocation(baseName, location);

            var assembly = Assembly.Load(location);

            return CreateStringLocalizerProxy(new StringLocalizerResourceSource(baseName, assembly, null));
        }

        private IStringLocalizer CreateStringLocalizerProxy(StringLocalizerResourceSource resourceSource)
        {
            // First use the cache.
            var localizer = this.cacheService.Match(resourceSource.Assembly, resourceSource.BaseName, null);
            if (localizer != null)
            {
                this.logger.GotStringLocalizerProxyFromCache(resourceSource.BaseName, resourceSource.Assembly);

                return localizer.AsStringLocalizer;
            }

            this.logger.CreateStringLocalizerProxy(resourceSource.BaseName, resourceSource.Assembly);

            localizer = new StringLocalizerProxy(
                resourceSource,
                this.loggerForStringLocalizerProxy,
                this.cultureInfoService,
                this);

            this.cacheService.Cache(resourceSource.Assembly, resourceSource.BaseName, null, localizer);

            return localizer.AsStringLocalizer;
        }

        /// <inheritdoc/>
        public IStringLocalizerInternal CreateStringLocalizer(StringLocalizerResourceSource resourceSource, CultureInfo cultureInfo)
        {
            ArgumentNullException.ThrowIfNull(resourceSource, nameof(resourceSource));

            // First use the cache.
            var localizer = this.cacheService.Match(resourceSource.Assembly, resourceSource.BaseName, cultureInfo);
            if (localizer != null)
            {
                this.logger.GotStringLocalizerFromCache(resourceSource.BaseName, resourceSource.Assembly, cultureInfo);

                return localizer;
            }

            var task = LoadStringLocalizerAsync(resourceSource.Assembly, resourceSource.BaseName, cultureInfo);

            if (task.Status == TaskStatus.RanToCompletion)
            {
                this.logger.LoadingTaskCompletedSynchronously(resourceSource.BaseName, resourceSource.Assembly, cultureInfo);

                var map = task.Result;
                if (map != null)
                {
                    localizer = new JsonStringLocalizer(resourceSource, map, cultureInfo, this);
                    localizer = this.cacheService.Cache(resourceSource.Assembly, resourceSource.BaseName, cultureInfo, localizer);
                }
                else
                {
                    localizer = this.CreateDefaultStringLocalizer(resourceSource, cultureInfo, null);
                }
            }
            else
            {
                this.logger.LoadingDataAsynchronously(resourceSource.BaseName, resourceSource.Assembly, cultureInfo);

                localizer = new JsonStringLocalizerAsync(resourceSource, task, cultureInfo, this);
                localizer = this.cacheService.Cache(resourceSource.Assembly, resourceSource.BaseName, cultureInfo, localizer);
            }

            return localizer;
        }

        private async Task<IReadOnlyDictionary<string, string>?> LoadStringLocalizerAsync(Assembly assembly, string baseName, CultureInfo cultureInfo)
        {
            if (this.options.SkipBaseNamePrefix.Any(p => baseName.StartsWith(p, StringComparison.Ordinal)))
            {
                return null;
            }

            var hadErrors = false;
            foreach (var extensionOptionsContainer in this.options.ExtensionOptions)
            {
#pragma warning disable CA1031 // Do not catch general exception types
                try
                {
                    var options = extensionOptionsContainer.Options;

                    var noAssambliesToMatch = !options.Assemblies.Any();

                    if (noAssambliesToMatch || options.Assemblies.Contains(assembly))
                    {
                        var extensionService = this.extensionResolverService.GetExtensionService(extensionOptionsContainer);

                        var map = await extensionService.TryLoadAsync(options, assembly, baseName, cultureInfo).ConfigureAwait(false);
                        if (map != null)
                        {
                            this.logger.LoadedLocalizationData(baseName, assembly, cultureInfo);
                            return map;
                        }
                    }
                }
                catch (FileLoadException e)
                {
                    this.logger.ErrorWhileLoadingLocalizationData(extensionOptionsContainer.ExtensionOptionsType.Name, e);
                }
                catch (Exception e)
                {
                    this.logger.ErrorWhileLoadingLocalizationData(extensionOptionsContainer.ExtensionOptionsType.Name, e);
                    hadErrors = true;
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }

            this.logger.UnableToLoadLocalizationData(baseName, assembly, cultureInfo);

            if (hadErrors)
            {
                this.cacheService.Reset(assembly, baseName, cultureInfo);
            }

            return null;
        }

        /// <inheritdoc/>
        public LocalizedString? FindThroughStringLocalizerHierarchy(IStringLocalizerInternal stringLocalizer, CultureInfo cultureInfo, Func<IStringLocalizerInternal, LocalizedString?> findHandler)
        {
            ArgumentNullException.ThrowIfNull(stringLocalizer, nameof(stringLocalizer));
            ArgumentNullException.ThrowIfNull(findHandler, nameof(findHandler));
            ArgumentNullException.ThrowIfNull(cultureInfo, nameof(cultureInfo));

            // 1. Find with the current localizer.
            var ls = findHandler(stringLocalizer);

            if (ls != null && !ls.ResourceNotFound)
            {
                return ls;
            }

            // 2. Find with the parent culture info.
            var ci = cultureInfo.Parent == cultureInfo ? null : cultureInfo.Parent;

            while (ci != null)
            {
                var parentLocalizer = CreateStringLocalizer(stringLocalizer.ResourceSource, ci);

                ls = findHandler(parentLocalizer);

                if (ls != null && !ls.ResourceNotFound)
                {
                    return ls;
                }

                ci = ci.Parent == ci ? null : ci.Parent;
            }

            // 3. Find on the class hierarchy.
            var parents = LoadLocalizerHierarchy(stringLocalizer.ResourceSource);

            foreach (var parent in parents)
            {
                ci = cultureInfo;

                do
                {
                    var parentLocalizer = CreateStringLocalizer(parent, ci);

                    ls = findHandler(parentLocalizer);

                    if (ls != null && !ls.ResourceNotFound)
                    {
                        return ls;
                    }

                    ci = ci.Parent == ci ? null : ci.Parent;
                }
                while (ci != null);
            }

            return null;
        }

        /// <inheritdoc/>
        public async ValueTask LoadDataThroughStringLocalizerHierarchyAsync(
            IStringLocalizerInternal stringLocalizer,
            CultureInfo cultureInfo,
            bool loadParentCulture)
        {
            ArgumentNullException.ThrowIfNull(stringLocalizer, nameof(stringLocalizer));
            ArgumentNullException.ThrowIfNull(cultureInfo, nameof(cultureInfo));

            // 1. Process with the current localizer.
            var loaded = await stringLocalizer.LoadDataAsync().ConfigureAwait(false);

            // 2. Process with the parent culture info.
            var ci = cultureInfo.Parent;

            while (ci != null && (loadParentCulture || !loaded))
            {
                var parentLocalizer = CreateStringLocalizer(stringLocalizer.ResourceSource, ci);

                loaded = await parentLocalizer.LoadDataAsync().ConfigureAwait(false);

                ci = ci.Parent == ci ? null : ci.Parent;
            }

            // 3. Process on the class hierarchy.
            var parents = LoadLocalizerHierarchy(stringLocalizer.ResourceSource);

            foreach (var parent in parents)
            {
                ci = cultureInfo;

                do
                {
                    var parentLocalizer = CreateStringLocalizer(parent, ci);

                    loaded = await parentLocalizer.LoadDataAsync().ConfigureAwait(false);

                    ci = ci.Parent == ci ? null : ci.Parent;
                }
                while (ci != null && (loadParentCulture || !loaded));
            }
        }

        private List<StringLocalizerResourceSource> LoadLocalizerHierarchy(StringLocalizerResourceSource resourceSource)
        {
            var parents = new List<StringLocalizerResourceSource>();

            if (resourceSource.ResourceSourceType != null)
            {
                var baseType = resourceSource.ResourceSourceType.BaseType;

                if (baseType != null && baseType != typeof(object))
                {
                    var baseName = baseType.GetBaseName();

                    if (!this.options.SkipBaseNamePrefix.Any(p => baseName.StartsWith(p, StringComparison.Ordinal)))
                    {
                        parents.Add(new StringLocalizerResourceSource(baseType.GetBaseName(), baseType.Assembly, baseType));
                    }
                }

                parents.AddRange(
                    resourceSource.ResourceSourceType.GetInterfaces()
                        .Where(x => !this.options.SkipBaseNamePrefix.Any(p => x.GetBaseName().StartsWith(p, StringComparison.Ordinal)))
                        .Select(x => new StringLocalizerResourceSource(x.GetBaseName(), x.Assembly, x)));
            }

            parents.AddRange(this.options.Fallbacks.Select(x => new StringLocalizerResourceSource(x.baseName, x.assembly, null)));

            return parents;
        }

        /// <inheritdoc/>
        public IStringLocalizerInternal CreateDefaultStringLocalizer(StringLocalizerResourceSource resourceSource, CultureInfo cultureInfo, string? stringLocalizerGuid)
        {
            return this.options.IsDisplayKeysWhenResourceNotFoundEnabled
                ? new NullStringLocalizer(resourceSource, cultureInfo, this, true, stringLocalizerGuid)
                : new ConstStringLocalizer(resourceSource, cultureInfo, this, "...", true, stringLocalizerGuid);
        }
    }
}
