// ----------------------------------------------------------------------
// <copyright file="JsonStringLocalizerFactory.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Reflection;
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
        private readonly IServiceProvider serviceProvider;
        private readonly ICultureInfoService cultureInfoService;

        /// <summary>
        /// Setup the factory.
        /// </summary>
        /// <param name="options">The JsonLocalization options.</param>
        /// <param name="cultureInfoService">Service providing the current culture.</param>
        /// <param name="serviceProvider">The service provider used to get the JsonLocalization
        /// extension service.</param>
        public JsonStringLocalizerFactory(
            IOptions<JsonLocalizationOptions> options,
            ICultureInfoService cultureInfoService,
            IServiceProvider serviceProvider)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.options = options.Value;
            this.serviceProvider = serviceProvider;
            this.cultureInfoService = cultureInfoService;
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

            foreach (var optionsExtension in this.options.ExtensionOptions)
            {
                var optionsService = optionsExtension.GetJsonLocalizationExtensionService(this.serviceProvider);

                if (optionsService.TryLoad(optionsExtension, assembly, baseName, cultureInfo, out var map))
                {
                    return new JsonStringLocalizer(map, cultureInfo);
                }
            }

            return new NullStringLocalizer(cultureInfo);
        }
    }
}
