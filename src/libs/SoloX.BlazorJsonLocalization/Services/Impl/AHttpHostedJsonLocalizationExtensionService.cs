// ----------------------------------------------------------------------
// <copyright file="AHttpHostedJsonLocalizationExtensionService.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;

namespace SoloX.BlazorJsonLocalization.Services.Impl
{
    /// <summary>
    /// Abstract HttpHostedJsonLocalizationExtensionService implementation.
    /// </summary>
    public abstract class AHttpHostedJsonLocalizationExtensionService
        : IJsonLocalizationExtensionService<HttpHostedJsonLocalizationOptions>
    {
        ///<inheritdoc/>
        public async ValueTask<IReadOnlyDictionary<string, string>?> TryLoadAsync(
            HttpHostedJsonLocalizationOptions options,
            Assembly assembly,
            string baseName,
            CultureInfo cultureInfo)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (cultureInfo == null)
            {
                throw new ArgumentNullException(nameof(cultureInfo));
            }

            var cultureName = cultureInfo.TwoLetterISOLanguageName;

            var path = (options.ApplicationAssembly == assembly)
                ? string.Empty
                : $"_content/{assembly.GetName().Name}/";

            if (!string.IsNullOrEmpty(options.ResourcesPath))
            {
                path = $"{path}{options.ResourcesPath}/";
            }

            var uri = new Uri($"{path}{baseName}-{cultureName}.json", UriKind.Relative);

            var map = await TryLoadFromUriAsync(uri).ConfigureAwait(false);

            if (map == null)
            {
                uri = new Uri($"{path}{baseName}.json", UriKind.Relative);

                map = await TryLoadFromUriAsync(uri).ConfigureAwait(false);
            }

            return map;
        }

        /// <summary>
        /// Load Http resources.
        /// </summary>
        /// <param name="uri">Resources Uri location.</param>
        /// <returns>The loaded Json map.</returns>
        protected abstract ValueTask<IReadOnlyDictionary<string, string>?> TryLoadFromUriAsync(Uri uri);
    }
}
