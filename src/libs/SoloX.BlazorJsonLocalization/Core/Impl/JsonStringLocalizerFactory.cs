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
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using SoloX.BlazorJsonLocalization.Services;

namespace SoloX.BlazorJsonLocalization.Core.Impl
{
    /// <summary>
    /// IStringLocalizerFactory implementation that will create the IStringLocalizer instance
    /// from the extension configured in the JsonLocalization options.
    /// </summary>
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly JsonLocalizationOptions options;
        private readonly IExtensionResolverService extensionResolverService;
        private readonly ICultureInfoService cultureInfoService;
        private readonly ICacheService cacheService;

        /// <summary>
        /// Setup the factory.
        /// </summary>
        /// <param name="options">The JsonLocalization options.</param>
        /// <param name="cultureInfoService">Service providing the current culture.</param>
        /// <param name="extensionResolverService">The service resolver to get the JsonLocalization
        /// extension service.</param>
        /// <param name="cacheService">The service to cache the loaded data.</param>
        public JsonStringLocalizerFactory(
            IOptions<JsonLocalizationOptions> options,
            ICultureInfoService cultureInfoService,
            IExtensionResolverService extensionResolverService,
            ICacheService cacheService)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.options = options.Value;
            this.extensionResolverService = extensionResolverService;
            this.cultureInfoService = cultureInfoService;
            this.cacheService = cacheService;
        }

        ///<inheritdoc/>
        public IStringLocalizer Create(Type resourceSource)
        {
            if (resourceSource == null)
            {
                throw new ArgumentNullException(nameof(resourceSource));
            }

            var assembly = resourceSource.Assembly;
            var baseName = resourceSource.Name;

            return CreateStringLocalizer(baseName, assembly);
        }

        ///<inheritdoc/>
        public IStringLocalizer Create(string baseName, string location)
        {
            var assembly = Assembly.Load(location);

            return CreateStringLocalizer(baseName, assembly);
        }

        private IStringLocalizer CreateStringLocalizer(string baseName, Assembly assembly)
        {
            var cultureInfo = this.cultureInfoService.CurrentUICulture;

            // First use the cache.
            var localizer = this.cacheService.Match(assembly, baseName, cultureInfo);
            if (localizer != null)
            {
                return localizer;
            }

            var task = LoadStringLocalizerAsync(assembly, baseName, cultureInfo);

            if (task.Status == TaskStatus.RanToCompletion)
            {
                var map = task.Result;
                if (map != null)
                {
                    localizer = new JsonStringLocalizer(map, cultureInfo);
                    this.cacheService.Cache(assembly, baseName, cultureInfo, localizer);
                }
                else
                {
                    localizer = new NullStringLocalizer(cultureInfo);
                }
            }
            else
            {
                localizer = new JsonStringLocalizerAsync(task, cultureInfo);
                this.cacheService.Cache(assembly, baseName, cultureInfo, localizer);
            }

            return localizer;
        }

        private async Task<IReadOnlyDictionary<string, string>?> LoadStringLocalizerAsync(Assembly assembly, string baseName, CultureInfo cultureInfo)
        {
            foreach (var extensionOptionsContainer in this.options.ExtensionOptions)
            {
                var options = extensionOptionsContainer.Options;

                var noAssambliesToMatch = !options.AssemblyNames.Any();

                if (noAssambliesToMatch || options.AssemblyNames.Contains(assembly.GetName().Name))
                {
                    var extensionService = this.extensionResolverService.GetExtensionService(extensionOptionsContainer);

                    var map = await extensionService.TryLoadAsync(options, assembly, baseName, cultureInfo).ConfigureAwait(false);
                    if (map != null)
                    {
                        return map;
                    }
                }
            }
            return null;
        }
    }
}
