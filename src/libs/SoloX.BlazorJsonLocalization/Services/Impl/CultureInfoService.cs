// ----------------------------------------------------------------------
// <copyright file="CultureInfoService.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using System.Globalization;

namespace SoloX.BlazorJsonLocalization.Services.Impl
{
    /// <summary>
    /// CultureInfo service implementation.
    /// </summary>
    public class CultureInfoService : ICultureInfoService
    {
        private ILogger<CultureInfoService> Logger { get; }

        /// <summary>
        /// Setup with logger.
        /// </summary>
        /// <param name="logger">The logger to log messages.</param>
        public CultureInfoService(ILogger<CultureInfoService> logger)
        {
            Logger = logger;
        }

        ///<inheritdoc/>
        public CultureInfo CurrentUICulture
        {
            get
            {
                Logger.LogInformation($"Current UI culture detected: {CultureInfo.CurrentUICulture}");
                return CultureInfo.CurrentUICulture;
            }
        }
    }
}
