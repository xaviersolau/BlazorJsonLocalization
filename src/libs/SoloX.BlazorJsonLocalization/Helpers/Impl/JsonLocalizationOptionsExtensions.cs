// ----------------------------------------------------------------------
// <copyright file="JsonLocalizationOptionsExtensions.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using System;

#if !NET6_0_OR_GREATER
using ArgumentNullException = SoloX.BlazorJsonLocalization.Helpers.ArgumentNullExceptionLegacy;
#endif

namespace SoloX.BlazorJsonLocalization.Helpers.Impl
{
    /// <summary>
    /// JsonLocalizationOptions Extension to setup logger.
    /// </summary>
    public static class JsonLocalizationOptionsExtensions
    {
        /// <summary>
        /// Setup logger depending of the options.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static ILogger<T> GetLogger<T>(this JsonLocalizationOptions options, ILogger<T> logger)
        {
            ArgumentNullException.ThrowIfNull(options, nameof(options));

            if (options.IsLoggerEnabled)
            {
                return logger;
            }
            else
            {
                return new NoLogger<T>();
            }
        }

        private class NoLogger<T> : ILogger<T>
        {
            private class NoLoggerScope : IDisposable
            {
                public void Dispose()
                {
                    // Nothing to do.
                }
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return new NoLoggerScope();
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return false;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                // Nothing to do.
            }
        }
    }
}
