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
        [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = "Loading localization data from {uri} using Web Host WebRootFileProvider")]
        internal static partial void LoadingLocalizationDataFromHost(
            this ILogger logger,
            Uri uri);

        [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "Loading file {uri}")]
        internal static partial void LoadingFile(
            this ILogger logger,
            Uri uri);

        [LoggerMessage(EventId = 2, Level = LogLevel.Warning, Message = "Web Host File {uri} does not exist")]
        internal static partial void FileDoesNotExist(
            this ILogger logger,
            Uri uri);
    }

#if !NET6_0_OR_GREATER
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class LoggerMessageAttribute : Attribute
    {
        public string Message { get; set; }
        public LogLevel Level { get; set; }
        public int EventId { get; set; }
    }

    partial class LoggingExtensions 
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.7.2304")]
        private static readonly global::System.Action<global::Microsoft.Extensions.Logging.ILogger, global::System.Uri, global::System.Exception?> __LoadingLocalizationDataFromHostCallback =
            global::Microsoft.Extensions.Logging.LoggerMessage.Define<global::System.Uri>(global::Microsoft.Extensions.Logging.LogLevel.Debug, new global::Microsoft.Extensions.Logging.EventId(0, nameof(LoadingLocalizationDataFromHost)), "Loading localization data from {uri} using Web Host WebRootFileProvider"); 

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.7.2304")]
        internal static partial void LoadingLocalizationDataFromHost(this global::Microsoft.Extensions.Logging.ILogger logger, global::System.Uri uri)
        {
            if (logger.IsEnabled(global::Microsoft.Extensions.Logging.LogLevel.Debug))
            {
                __LoadingLocalizationDataFromHostCallback(logger, uri, null);
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.7.2304")]
        private static readonly global::System.Action<global::Microsoft.Extensions.Logging.ILogger, global::System.Uri, global::System.Exception?> __LoadingFileCallback =
            global::Microsoft.Extensions.Logging.LoggerMessage.Define<global::System.Uri>(global::Microsoft.Extensions.Logging.LogLevel.Debug, new global::Microsoft.Extensions.Logging.EventId(1, nameof(LoadingFile)), "Loading file {uri}"); 

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.7.2304")]
        internal static partial void LoadingFile(this global::Microsoft.Extensions.Logging.ILogger logger, global::System.Uri uri)
        {
            if (logger.IsEnabled(global::Microsoft.Extensions.Logging.LogLevel.Debug))
            {
                __LoadingFileCallback(logger, uri, null);
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.7.2304")]
        private static readonly global::System.Action<global::Microsoft.Extensions.Logging.ILogger, global::System.Uri, global::System.Exception?> __FileDoesNotExistCallback =
            global::Microsoft.Extensions.Logging.LoggerMessage.Define<global::System.Uri>(global::Microsoft.Extensions.Logging.LogLevel.Warning, new global::Microsoft.Extensions.Logging.EventId(2, nameof(FileDoesNotExist)), "Web Host File {uri} does not exist"); 

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.7.2304")]
        internal static partial void FileDoesNotExist(this global::Microsoft.Extensions.Logging.ILogger logger, global::System.Uri uri)
        {
            if (logger.IsEnabled(global::Microsoft.Extensions.Logging.LogLevel.Warning))
            {
                __FileDoesNotExistCallback(logger, uri, null);
            }
        }
    }
#endif
}
