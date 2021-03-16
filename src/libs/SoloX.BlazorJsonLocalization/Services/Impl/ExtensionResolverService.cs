// ----------------------------------------------------------------------
// <copyright file="ExtensionResolverService.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using SoloX.BlazorJsonLocalization.Core;
using System;

namespace SoloX.BlazorJsonLocalization.Services.Impl
{
    /// <summary>
    /// ExtensionResolverService implementation.
    /// </summary>
    public class ExtensionResolverService : IExtensionResolverService
    {
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Setup the ExtensionResolverService with a service provider.
        /// </summary>
        /// <param name="serviceProvider">The service provider to get the extension services from.</param>
        public ExtensionResolverService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        ///<inheritdoc/>
        public IJsonLocalizationExtensionService GetExtensionService(IExtensionOptionsContainer optionsContainer)
        {
            if (optionsContainer == null)
            {
                throw new ArgumentNullException(nameof(optionsContainer));
            }

            var extensionService = this.serviceProvider
                .GetRequiredService(optionsContainer.ExtensionServiceType);

            return (IJsonLocalizationExtensionService)extensionService;
        }
    }
}
