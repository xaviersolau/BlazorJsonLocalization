// ----------------------------------------------------------------------
// <copyright file="LoggingExtensions.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using System;

namespace SoloX.BlazorJsonLocalization.ServerSide
{
    internal static partial class LoggingExtensions
    {
        [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = "Loading localization data from {uri} using HTTP client")]
        internal static partial void LoadingLocalizationDataFromHttpClient(
            this ILogger logger,
            Uri uri);

        [LoggerMessage(EventId = 0, Level = LogLevel.Warning, Message = "Http file not found from {uri}")]
        internal static partial void HttpFileNotFound(
            this ILogger logger,
            Uri uri,
            Exception exception);
    }

#if !NET6_0_OR_GREATER
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class LoggerMessageAttribute : Attribute
    {
        public string Message { get; set; }
        public LogLevel Level { get; set; }
        public int EventId { get; set; }
    }

    internal partial class LoggingExtensions
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        private static readonly global::System.Action<global::Microsoft.Extensions.Logging.ILogger, global::System.Uri, global::System.Exception?> __LoadingLocalizationDataFromHttpClientCallback =
            global::Microsoft.Extensions.Logging.LoggerMessage.Define<global::System.Uri>(global::Microsoft.Extensions.Logging.LogLevel.Debug, new global::Microsoft.Extensions.Logging.EventId(0, nameof(LoadingLocalizationDataFromHttpClient)), "Loading localization data from {uri} using HTTP client");

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        internal static partial void LoadingLocalizationDataFromHttpClient(this global::Microsoft.Extensions.Logging.ILogger logger, global::System.Uri uri)
        {
            if (logger.IsEnabled(global::Microsoft.Extensions.Logging.LogLevel.Debug))
            {
                __LoadingLocalizationDataFromHttpClientCallback(logger, uri, null);
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        private static readonly global::System.Action<global::Microsoft.Extensions.Logging.ILogger, global::System.Uri, global::System.Exception?> __HttpFileNotFoundCallback =
            global::Microsoft.Extensions.Logging.LoggerMessage.Define<global::System.Uri>(global::Microsoft.Extensions.Logging.LogLevel.Warning, new global::Microsoft.Extensions.Logging.EventId(0, nameof(HttpFileNotFound)), "Http file not found from {uri}");

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        internal static partial void HttpFileNotFound(this global::Microsoft.Extensions.Logging.ILogger logger, global::System.Uri uri, global::System.Exception exception)
        {
            if (logger.IsEnabled(global::Microsoft.Extensions.Logging.LogLevel.Warning))
            {
                __HttpFileNotFoundCallback(logger, uri, exception);
            }
        }
    }
#endif
}
