// ----------------------------------------------------------------------
// <copyright file="LoggingExtensions.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Reflection;

namespace SoloX.BlazorJsonLocalization
{
    internal static partial class LoggingExtensions
    {
        [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = "Current UI culture changed: from {oldCulture} to {newCulture}.")]
        internal static partial void SwitchCurrentCulture(
            this ILogger logger,
            CultureInfo oldCulture,
            CultureInfo newCulture);

        [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "Loading embedded data : {path}")]
        internal static partial void LoadingEmbeddedData(
            this ILogger logger,
            string path);

        [LoggerMessage(EventId = 2, Level = LogLevel.Debug, Message = "Create String localizer for {name}")]
        internal static partial void CreateStringLocalizer(
            this ILogger logger,
            string name);

        [LoggerMessage(EventId = 3, Level = LogLevel.Debug, Message = "Create String localizer for {name} in {location}")]
        internal static partial void CreateStringLocalizerInLocation(
            this ILogger logger,
            string name,
            string location);

        [LoggerMessage(EventId = 4, Level = LogLevel.Debug, Message = "Loading static assets data from {uri}")]
        internal static partial void LoadingStaticAssets(
            this ILogger logger,
            Uri? uri);

        [LoggerMessage(EventId = 5, Level = LogLevel.Debug, Message = "Got String localizer proxy for {name} in {assembly} from cache")]
        internal static partial void GotStringLocalizerProxyFromCache(
            this ILogger logger,
            string name,
            Assembly assembly);

        [LoggerMessage(EventId = 6, Level = LogLevel.Debug, Message = "Create String localizer proxy for {name} in {assembly} and register in cache")]
        internal static partial void CreateStringLocalizerProxy(
            this ILogger logger,
            string name,
            Assembly assembly);

        [LoggerMessage(EventId = 7, Level = LogLevel.Warning, Message = "Embedded File {path} does not exist")]
        internal static partial void EmbeddedFileNotFound(
            this ILogger logger,
            string path);

        [LoggerMessage(EventId = 8, Level = LogLevel.Debug, Message = "Loading file {path}")]
        internal static partial void LoadingFile(
            this ILogger logger,
            string path);

        [LoggerMessage(EventId = 9, Level = LogLevel.Debug, Message = "Got String localizer for {name} in {assembly} with culture {cultureInfo} from cache")]
        internal static partial void GotStringLocalizerFromCache(
            this ILogger logger,
            string name,
            Assembly assembly,
            CultureInfo cultureInfo);

        [LoggerMessage(EventId = 10, Level = LogLevel.Information, Message = "Loading task completed synchronously for {name} in {assembly} with culture {cultureInfo}")]
        internal static partial void LoadingTaskCompletedSynchronously(
            this ILogger logger,
            string name,
            Assembly assembly,
            CultureInfo cultureInfo);

        [LoggerMessage(EventId = 11, Level = LogLevel.Information, Message = "Loading data asynchronously for {name} in {assembly} with culture {cultureInfo}")]
        internal static partial void LoadingDataAsynchronously(
            this ILogger logger,
            string name,
            Assembly assembly,
            CultureInfo cultureInfo);

        [LoggerMessage(EventId = 12, Level = LogLevel.Information, Message = "Loaded localization data for {name} in assembly {assembly} with culture {cultureInfo}")]
        internal static partial void LoadedLocalizationData(
            this ILogger logger,
            string name,
            Assembly assembly,
            CultureInfo cultureInfo);

        [LoggerMessage(EventId = 13, Level = LogLevel.Error, Message = "Error while loading localization data from extension {extensionName}")]
        internal static partial void ErrorWhileLoadingLocalizationData(
            this ILogger logger,
            string extensionName,
            Exception exception);

        [LoggerMessage(EventId = 14, Level = LogLevel.Error, Message = "Unable to load localization data for {name} in assembly {assembly} with culture {cultureInfo}")]
        internal static partial void UnableToLoadLocalizationData(
            this ILogger logger,
            string name,
            Assembly assembly,
            CultureInfo cultureInfo);
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
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "7.0.7.1805")]
        private static readonly global::System.Action<global::Microsoft.Extensions.Logging.ILogger, global::System.Globalization.CultureInfo, global::System.Globalization.CultureInfo, global::System.Exception?> __SwitchCurrentCultureCallback =
            global::Microsoft.Extensions.Logging.LoggerMessage.Define<global::System.Globalization.CultureInfo, global::System.Globalization.CultureInfo>(global::Microsoft.Extensions.Logging.LogLevel.Debug, new global::Microsoft.Extensions.Logging.EventId(0, nameof(SwitchCurrentCulture)), "Current UI culture changed: from {oldCulture} to {newCulture}.");

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "7.0.7.1805")]
        internal static partial void SwitchCurrentCulture(this global::Microsoft.Extensions.Logging.ILogger logger, global::System.Globalization.CultureInfo oldCulture, global::System.Globalization.CultureInfo newCulture)
        {
            if (logger.IsEnabled(global::Microsoft.Extensions.Logging.LogLevel.Debug))
            {
                __SwitchCurrentCultureCallback(logger, oldCulture, newCulture, null);
            }
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        private static readonly global::System.Action<global::Microsoft.Extensions.Logging.ILogger, global::System.String, global::System.Exception?> __LoadingEmbeddedDataCallback =
            global::Microsoft.Extensions.Logging.LoggerMessage.Define<global::System.String>(global::Microsoft.Extensions.Logging.LogLevel.Debug, new global::Microsoft.Extensions.Logging.EventId(1, nameof(LoadingEmbeddedData)), "Loading embedded data : {path}");

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        internal static partial void LoadingEmbeddedData(this global::Microsoft.Extensions.Logging.ILogger logger, global::System.String path)
        {
            if (logger.IsEnabled(global::Microsoft.Extensions.Logging.LogLevel.Debug))
            {
                __LoadingEmbeddedDataCallback(logger, path, null);
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        private static readonly global::System.Action<global::Microsoft.Extensions.Logging.ILogger, global::System.String, global::System.Exception?> __CreateStringLocalizerCallback =
            global::Microsoft.Extensions.Logging.LoggerMessage.Define<global::System.String>(global::Microsoft.Extensions.Logging.LogLevel.Debug, new global::Microsoft.Extensions.Logging.EventId(2, nameof(CreateStringLocalizer)), "Create String localizer for {name}");

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        internal static partial void CreateStringLocalizer(this global::Microsoft.Extensions.Logging.ILogger logger, global::System.String name)
        {
            if (logger.IsEnabled(global::Microsoft.Extensions.Logging.LogLevel.Debug))
            {
                __CreateStringLocalizerCallback(logger, name, null);
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        private static readonly global::System.Action<global::Microsoft.Extensions.Logging.ILogger, global::System.String, global::System.String, global::System.Exception?> __CreateStringLocalizerInLocationCallback =
            global::Microsoft.Extensions.Logging.LoggerMessage.Define<global::System.String, global::System.String>(global::Microsoft.Extensions.Logging.LogLevel.Debug, new global::Microsoft.Extensions.Logging.EventId(3, nameof(CreateStringLocalizerInLocation)), "Create String localizer for {name} in {location}");

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        internal static partial void CreateStringLocalizerInLocation(this global::Microsoft.Extensions.Logging.ILogger logger, global::System.String name, global::System.String location)
        {
            if (logger.IsEnabled(global::Microsoft.Extensions.Logging.LogLevel.Debug))
            {
                __CreateStringLocalizerInLocationCallback(logger, name, location, null);
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        private static readonly global::System.Action<global::Microsoft.Extensions.Logging.ILogger, global::System.Uri?, global::System.Exception?> __LoadingStaticAssetsCallback =
            global::Microsoft.Extensions.Logging.LoggerMessage.Define<global::System.Uri?>(global::Microsoft.Extensions.Logging.LogLevel.Debug, new global::Microsoft.Extensions.Logging.EventId(4, nameof(LoadingStaticAssets)), "Loading static assets data from {uri}");

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        internal static partial void LoadingStaticAssets(this global::Microsoft.Extensions.Logging.ILogger logger, global::System.Uri? uri)
        {
            if (logger.IsEnabled(global::Microsoft.Extensions.Logging.LogLevel.Debug))
            {
                __LoadingStaticAssetsCallback(logger, uri, null);
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        private static readonly global::System.Action<global::Microsoft.Extensions.Logging.ILogger, global::System.String, global::System.Reflection.Assembly, global::System.Exception?> __GotStringLocalizerProxyFromCacheCallback =
            global::Microsoft.Extensions.Logging.LoggerMessage.Define<global::System.String, global::System.Reflection.Assembly>(global::Microsoft.Extensions.Logging.LogLevel.Debug, new global::Microsoft.Extensions.Logging.EventId(5, nameof(GotStringLocalizerProxyFromCache)), "Got String localizer proxy for {name} in {assembly} from cache");

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        internal static partial void GotStringLocalizerProxyFromCache(this global::Microsoft.Extensions.Logging.ILogger logger, global::System.String name, global::System.Reflection.Assembly assembly)
        {
            if (logger.IsEnabled(global::Microsoft.Extensions.Logging.LogLevel.Debug))
            {
                __GotStringLocalizerProxyFromCacheCallback(logger, name, assembly, null);
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        private static readonly global::System.Action<global::Microsoft.Extensions.Logging.ILogger, global::System.String, global::System.Reflection.Assembly, global::System.Exception?> __CreateStringLocalizerProxyCallback =
            global::Microsoft.Extensions.Logging.LoggerMessage.Define<global::System.String, global::System.Reflection.Assembly>(global::Microsoft.Extensions.Logging.LogLevel.Debug, new global::Microsoft.Extensions.Logging.EventId(6, nameof(CreateStringLocalizerProxy)), "Create String localizer proxy for {name} in {assembly} and register in cache");

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        internal static partial void CreateStringLocalizerProxy(this global::Microsoft.Extensions.Logging.ILogger logger, global::System.String name, global::System.Reflection.Assembly assembly)
        {
            if (logger.IsEnabled(global::Microsoft.Extensions.Logging.LogLevel.Debug))
            {
                __CreateStringLocalizerProxyCallback(logger, name, assembly, null);
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        private static readonly global::System.Action<global::Microsoft.Extensions.Logging.ILogger, global::System.String, global::System.Exception?> __EmbeddedFileNotFoundCallback =
            global::Microsoft.Extensions.Logging.LoggerMessage.Define<global::System.String>(global::Microsoft.Extensions.Logging.LogLevel.Warning, new global::Microsoft.Extensions.Logging.EventId(7, nameof(EmbeddedFileNotFound)), "Embedded File {path} does not exist");

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        internal static partial void EmbeddedFileNotFound(this global::Microsoft.Extensions.Logging.ILogger logger, global::System.String path)
        {
            if (logger.IsEnabled(global::Microsoft.Extensions.Logging.LogLevel.Warning))
            {
                __EmbeddedFileNotFoundCallback(logger, path, null);
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        private static readonly global::System.Action<global::Microsoft.Extensions.Logging.ILogger, global::System.String, global::System.Exception?> __LoadingFileCallback =
            global::Microsoft.Extensions.Logging.LoggerMessage.Define<global::System.String>(global::Microsoft.Extensions.Logging.LogLevel.Debug, new global::Microsoft.Extensions.Logging.EventId(8, nameof(LoadingFile)), "Loading file {path}");

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        internal static partial void LoadingFile(this global::Microsoft.Extensions.Logging.ILogger logger, global::System.String path)
        {
            if (logger.IsEnabled(global::Microsoft.Extensions.Logging.LogLevel.Debug))
            {
                __LoadingFileCallback(logger, path, null);
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        private static readonly global::System.Action<global::Microsoft.Extensions.Logging.ILogger, global::System.String, global::System.Reflection.Assembly, global::System.Globalization.CultureInfo, global::System.Exception?> __GotStringLocalizerFromCacheCallback =
            global::Microsoft.Extensions.Logging.LoggerMessage.Define<global::System.String, global::System.Reflection.Assembly, global::System.Globalization.CultureInfo>(global::Microsoft.Extensions.Logging.LogLevel.Debug, new global::Microsoft.Extensions.Logging.EventId(9, nameof(GotStringLocalizerFromCache)), "Got String localizer for {name} in {assembly} with culture {cultureInfo} from cache");

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        internal static partial void GotStringLocalizerFromCache(this global::Microsoft.Extensions.Logging.ILogger logger, global::System.String name, global::System.Reflection.Assembly assembly, global::System.Globalization.CultureInfo cultureInfo)
        {
            if (logger.IsEnabled(global::Microsoft.Extensions.Logging.LogLevel.Debug))
            {
                __GotStringLocalizerFromCacheCallback(logger, name, assembly, cultureInfo, null);
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        private static readonly global::System.Action<global::Microsoft.Extensions.Logging.ILogger, global::System.String, global::System.Reflection.Assembly, global::System.Globalization.CultureInfo, global::System.Exception?> __LoadingTaskCompletedSynchronouslyCallback =
            global::Microsoft.Extensions.Logging.LoggerMessage.Define<global::System.String, global::System.Reflection.Assembly, global::System.Globalization.CultureInfo>(global::Microsoft.Extensions.Logging.LogLevel.Information, new global::Microsoft.Extensions.Logging.EventId(10, nameof(LoadingTaskCompletedSynchronously)), "Loading task completed synchronously for {name} in {assembly} with culture {cultureInfo}");

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        internal static partial void LoadingTaskCompletedSynchronously(this global::Microsoft.Extensions.Logging.ILogger logger, global::System.String name, global::System.Reflection.Assembly assembly, global::System.Globalization.CultureInfo cultureInfo)
        {
            if (logger.IsEnabled(global::Microsoft.Extensions.Logging.LogLevel.Information))
            {
                __LoadingTaskCompletedSynchronouslyCallback(logger, name, assembly, cultureInfo, null);
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        private static readonly global::System.Action<global::Microsoft.Extensions.Logging.ILogger, global::System.String, global::System.Reflection.Assembly, global::System.Globalization.CultureInfo, global::System.Exception?> __LoadingDataAsynchronouslyCallback =
            global::Microsoft.Extensions.Logging.LoggerMessage.Define<global::System.String, global::System.Reflection.Assembly, global::System.Globalization.CultureInfo>(global::Microsoft.Extensions.Logging.LogLevel.Information, new global::Microsoft.Extensions.Logging.EventId(11, nameof(LoadingDataAsynchronously)), "Loading data asynchronously for {name} in {assembly} with culture {cultureInfo}");

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        internal static partial void LoadingDataAsynchronously(this global::Microsoft.Extensions.Logging.ILogger logger, global::System.String name, global::System.Reflection.Assembly assembly, global::System.Globalization.CultureInfo cultureInfo)
        {
            if (logger.IsEnabled(global::Microsoft.Extensions.Logging.LogLevel.Information))
            {
                __LoadingDataAsynchronouslyCallback(logger, name, assembly, cultureInfo, null);
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        private static readonly global::System.Action<global::Microsoft.Extensions.Logging.ILogger, global::System.String, global::System.Reflection.Assembly, global::System.Globalization.CultureInfo, global::System.Exception?> __LoadedLocalizationDataCallback =
            global::Microsoft.Extensions.Logging.LoggerMessage.Define<global::System.String, global::System.Reflection.Assembly, global::System.Globalization.CultureInfo>(global::Microsoft.Extensions.Logging.LogLevel.Information, new global::Microsoft.Extensions.Logging.EventId(12, nameof(LoadedLocalizationData)), "Loaded localization data for {name} in assembly {assembly} with culture {cultureInfo}");

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        internal static partial void LoadedLocalizationData(this global::Microsoft.Extensions.Logging.ILogger logger, global::System.String name, global::System.Reflection.Assembly assembly, global::System.Globalization.CultureInfo cultureInfo)
        {
            if (logger.IsEnabled(global::Microsoft.Extensions.Logging.LogLevel.Information))
            {
                __LoadedLocalizationDataCallback(logger, name, assembly, cultureInfo, null);
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        private static readonly global::System.Action<global::Microsoft.Extensions.Logging.ILogger, global::System.String, global::System.Exception?> __ErrorWhileLoadingLocalizationDataCallback =
            global::Microsoft.Extensions.Logging.LoggerMessage.Define<global::System.String>(global::Microsoft.Extensions.Logging.LogLevel.Error, new global::Microsoft.Extensions.Logging.EventId(13, nameof(ErrorWhileLoadingLocalizationData)), "Error while loading localization data from extension {extensionName}");

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        internal static partial void ErrorWhileLoadingLocalizationData(this global::Microsoft.Extensions.Logging.ILogger logger, global::System.String extensionName, global::System.Exception exception)
        {
            if (logger.IsEnabled(global::Microsoft.Extensions.Logging.LogLevel.Error))
            {
                __ErrorWhileLoadingLocalizationDataCallback(logger, extensionName, exception);
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        private static readonly global::System.Action<global::Microsoft.Extensions.Logging.ILogger, global::System.String, global::System.Reflection.Assembly, global::System.Globalization.CultureInfo, global::System.Exception?> __UnableToLoadLocalizationDataCallback =
            global::Microsoft.Extensions.Logging.LoggerMessage.Define<global::System.String, global::System.Reflection.Assembly, global::System.Globalization.CultureInfo>(global::Microsoft.Extensions.Logging.LogLevel.Error, new global::Microsoft.Extensions.Logging.EventId(14, nameof(UnableToLoadLocalizationData)), "Unable to load localization data for {name} in assembly {assembly} with culture {cultureInfo}");

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
        internal static partial void UnableToLoadLocalizationData(this global::Microsoft.Extensions.Logging.ILogger logger, global::System.String name, global::System.Reflection.Assembly assembly, global::System.Globalization.CultureInfo cultureInfo)
        {
            if (logger.IsEnabled(global::Microsoft.Extensions.Logging.LogLevel.Error))
            {
                __UnableToLoadLocalizationDataCallback(logger, name, assembly, cultureInfo, null);
            }
        }
    }
#endif
}