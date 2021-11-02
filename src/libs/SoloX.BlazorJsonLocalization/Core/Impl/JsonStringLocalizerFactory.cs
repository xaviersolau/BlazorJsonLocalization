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
using SoloX.BlazorJsonLocalization.Services;

namespace SoloX.BlazorJsonLocalization.Core.Impl
{
    /// <summary>
    /// IStringLocalizerFactory implementation that will create the IStringLocalizer instance
    /// from the extension configured in the JsonLocalization options.
    /// </summary>
    public sealed class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private class FactoryInternal : IJsonStringLocalizerFactoryInternal
        {
            private readonly Func<CultureInfo, IStringLocalizer> createHandler;

            public FactoryInternal(Func<CultureInfo, IStringLocalizer> create)
            {
                this.createHandler = create;
            }

            public IStringLocalizer CreateStringLocalizer(CultureInfo cultureInfo)
            {
                return this.createHandler(cultureInfo);
            }
        }

        private readonly JsonLocalizationOptions options;
        private readonly IExtensionResolverService extensionResolverService;
        private readonly ICultureInfoService cultureInfoService;
        private readonly ICacheService cacheService;
        private readonly ILogger<JsonStringLocalizerFactory> logger;

        /// <summary>
        /// Setup the factory.
        /// </summary>
        /// <param name="options">The JsonLocalization options.</param>
        /// <param name="cultureInfoService">Service providing the current culture.</param>
        /// <param name="extensionResolverService">The service resolver to get the JsonLocalization
        /// extension service.</param>
        /// <param name="cacheService">The service to cache the loaded data.</param>
        /// <param name="logger">Logger where to log processing messages.</param>
        public JsonStringLocalizerFactory(
            IOptions<JsonLocalizationOptions> options,
            ICultureInfoService cultureInfoService,
            IExtensionResolverService extensionResolverService,
            ICacheService cacheService,
            ILogger<JsonStringLocalizerFactory> logger)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.logger = logger;
            this.options = options.Value;
            this.extensionResolverService = extensionResolverService;
            this.cultureInfoService = cultureInfoService;
            this.cacheService = cacheService;
        }

        ///<inheritdoc/>
        public IStringLocalizer Create(Type resourceSource)
        {
            this.logger.LogDebug($"Create String localizer for {resourceSource?.FullName}");

            if (resourceSource == null)
            {
                throw new ArgumentNullException(nameof(resourceSource));
            }

            var assembly = resourceSource.Assembly;
            var baseName = resourceSource.FullName ?? resourceSource.Name;

            return CreateStringLocalizerProxy(baseName, assembly);
        }

        ///<inheritdoc/>
        public IStringLocalizer Create(string baseName, string location)
        {
            this.logger.LogDebug($"Create String localizer for {baseName} in {location}");

            var assembly = Assembly.Load(location);

            return CreateStringLocalizerProxy(baseName, assembly);
        }

        private IStringLocalizer CreateStringLocalizerProxy(string baseName, Assembly assembly)
        {
            // First use the cache.
            var localizer = this.cacheService.Match(assembly, baseName, null);
            if (localizer != null)
            {
                this.logger.LogDebug($"Got String localizer proxy for {baseName} in {assembly} from cache");

                return localizer;
            }

            this.logger.LogDebug($"Create String localizer proxy for {baseName} in {assembly} and register in cache");

            localizer = new StringLocalizerProxy(this.cultureInfoService, new FactoryInternal(ci => this.CreateStringLocalizer(baseName, assembly, ci)));
            this.cacheService.Cache(assembly, baseName, null, localizer);

            return localizer;
        }

        private IStringLocalizer CreateStringLocalizer(string baseName, Assembly assembly, CultureInfo cultureInfo)
        {
            // First use the cache.
            var localizer = this.cacheService.Match(assembly, baseName, cultureInfo);
            if (localizer != null)
            {
                this.logger.LogDebug($"Got String localizer for {baseName} in {assembly} with culture {cultureInfo} from cache");

                return localizer;
            }

            var task = LoadStringLocalizerAsync(assembly, baseName, cultureInfo);

            if (task.Status == TaskStatus.RanToCompletion)
            {
                this.logger.LogInformation($"Loading task completed synchronously for {baseName} in {assembly} with culture {cultureInfo}");

                var map = task.Result;
                if (map != null)
                {
                    localizer = new JsonStringLocalizer(map, cultureInfo, new FactoryInternal(ci => this.CreateStringLocalizer(baseName, assembly, ci)));
                    this.cacheService.Cache(assembly, baseName, cultureInfo, localizer);
                }
                else
                {
                    localizer = new NullStringLocalizer(cultureInfo, new FactoryInternal(ci => this.CreateStringLocalizer(baseName, assembly, ci)), true);
                }
            }
            else
            {
                this.logger.LogInformation($"Loading data asynchronously for {baseName} in {assembly} with culture {cultureInfo}");

                var factory = new FactoryInternal(ci => this.CreateStringLocalizer(baseName, assembly, ci));

                IStringLocalizer loadingLocalizer = this.options.IsDisplayKeysWhileLoadingAsynchronouslyEnabled
                    ? new NullStringLocalizer(cultureInfo, factory, false)
                    : new ConstStringLocalizer("...", factory);

                localizer = new JsonStringLocalizerAsync(task, cultureInfo, factory, loadingLocalizer);
                this.cacheService.Cache(assembly, baseName, cultureInfo, localizer);
            }

            return localizer;
        }

        private async Task<IReadOnlyDictionary<string, string>?> LoadStringLocalizerAsync(Assembly assembly, string baseName, CultureInfo cultureInfo)
        {
            foreach (var extensionOptionsContainer in this.options.ExtensionOptions)
            {
                try
                {
                    var options = extensionOptionsContainer.Options;

                    var noAssambliesToMatch = !options.AssemblyNames.Any();

                    if (noAssambliesToMatch || options.AssemblyNames.Contains(assembly.GetName().Name))
                    {
                        var extensionService = this.extensionResolverService.GetExtensionService(extensionOptionsContainer);

                        var map = await extensionService.TryLoadAsync(options, assembly, baseName, cultureInfo).ConfigureAwait(false);
                        if (map != null)
                        {
                            this.logger.LogInformation($"Loaded localization data for {baseName} in assembly {assembly.GetName().Name} with culture {cultureInfo}");
                            return map;
                        }
                    }
                }
                catch (FileLoadException e)
                {
                    this.logger.LogError(e, $"Error while loading localization data from extension {extensionOptionsContainer.ExtensionOptionsType.Name}");
                }
#pragma warning disable CA1031 // Ne pas intercepter les types d'exception générale
                catch (Exception e)
#pragma warning restore CA1031 // Ne pas intercepter les types d'exception générale
                {
                    this.logger.LogError(e, $"Error while loading localization data from extension {extensionOptionsContainer.ExtensionOptionsType.Name}");
                }
            }

            this.logger.LogError($"Unable to load localization data for {baseName} in assembly {assembly.GetName().Name} with culture {cultureInfo}");
            return null;
        }
    }
}
