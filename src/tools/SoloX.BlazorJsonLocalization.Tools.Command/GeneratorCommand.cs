// ----------------------------------------------------------------------
// <copyright file="GeneratorCommand.cs" company="Xavier Solau">
// Copyright © 2021-2026 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.CommandLine;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Serilog;
using SoloX.BlazorJsonLocalization.Tools.Core;
using SoloX.BlazorJsonLocalization.Tools.Extensions;

namespace SoloX.BlazorJsonLocalization.Tools
{
    /// <summary>
    /// Generator command.
    /// </summary>
    internal sealed class GeneratorCommand
    {
        private readonly RootCommand rootCommand = new RootCommand("Localization generation command.");

        private readonly Argument<FileInfo> projectArgument = new Argument<FileInfo>("projectFile")
        {
            Description = "CS Project to run generation."
        };
        private readonly Option<FileInfo> outputCodeOption = new Option<FileInfo>("--outputCode")
        {
            Description = "File to store CS output file list."
        };
        private readonly Option<FileInfo> outputResourceOption = new Option<FileInfo>("--outputResource")
        {
            Description = "File to store Json output resource file list."
        };
        private readonly Option<FileInfo> logFileOption = new Option<FileInfo>("--logFile")
        {
            Description = "File to write logging."
        };
        private readonly Option<bool> logDebugOption = new Option<bool>("--logDebug")
        {
            Description = "Enable debug logging."
        };
        private readonly Option<bool> useRelaxedJsonEscaping = new Option<bool>("--useRelaxedJsonEscaping")
        {
            Description = "Use Relaxed Json Escaping."
        };
        private readonly Option<bool> useMultiLine = new Option<bool>("--useMultiLine")
        {
            Description = "Use Multi-Line Json resources."
        };
        private readonly Option<bool> registerEmbeddedResource = new Option<bool>("--registerEmbeddedResource")
        {
            Description = "Register generated Json files as EmbeddedResource in the project."
        };
        private readonly Option<bool> disableCompile = new Option<bool>("--disableCompile")
        {
            Description = "Disable Compile integration of the generated C# files."
        };

        /// <summary>
        /// Setup GeneratorCommand instance.
        /// </summary>
        public GeneratorCommand()
        {
            this.rootCommand.Options.Add(this.logFileOption);
            this.rootCommand.Options.Add(this.logDebugOption);

            this.rootCommand.Arguments.Add(this.projectArgument);
            this.rootCommand.Options.Add(this.outputCodeOption);
            this.rootCommand.Options.Add(this.outputResourceOption);
            this.rootCommand.Options.Add(this.useRelaxedJsonEscaping);
            this.rootCommand.Options.Add(this.useMultiLine);
            this.rootCommand.Options.Add(this.registerEmbeddedResource);
            this.rootCommand.Options.Add(this.disableCompile);

            this.rootCommand.SetAction(parseResult => RunGeneratorCommandHandlerAsync(parseResult));
        }

        /// <summary>
        /// Run the command.
        /// </summary>
        /// <param name="args">Command arguments.</param>
        /// <returns></returns>
        public async Task<int> RunGeneratorCommandAsync(string[] args)
        {
            return await this.rootCommand.Parse(args).InvokeAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Run command
        /// </summary>
        private Task RunGeneratorCommandHandlerAsync(ParseResult parseResult)
        {
            return SetupServiceCollectionAndRunCommandAsync(parseResult, RunGeneratorCommandHandlerAsync);
        }

        private async Task SetupServiceCollectionAndRunCommandAsync(ParseResult parseResult, Func<ParseResult, IServiceProvider, Task<int>> commandHandler)
        {
            var serviceCollection = new ServiceCollection();

            var logDebug = parseResult.GetValue(this.logDebugOption);

            var logFile = parseResult.GetValue(this.logFileOption);
            if (logFile != null)
            {
                var fileLoggerConf = new LoggerConfiguration()
                    .WriteTo
                    .File(logFile.FullName, formatProvider: CultureInfo.InvariantCulture, rollOnFileSizeLimit: true, fileSizeLimitBytes: 5 * 1_024 * 1_024);

                if (logDebug)
                {
                    fileLoggerConf
                        .MinimumLevel
                        .Debug();
                }

                using var fileLogger = fileLoggerConf
                    .CreateLogger();

                using var loggerFactory = LoggerFactory.Create(
                    b =>
                    {
                        b.ClearProviders();
                        b.AddSerilog(fileLogger);
                    });

                serviceCollection.AddToolsGenerator(loggerFactory);

                var serviceProvider = serviceCollection.BuildServiceProvider();
                await using var _ = serviceProvider.ConfigureAwait(false);

                await commandHandler(parseResult, serviceProvider).ConfigureAwait(false);
            }
            else
            {
                serviceCollection.AddLogging(b =>
                {
                    b.AddConsole(opt =>
                    {
                        opt.FormatterName = BasicConsoleFormatter.Name;
                    })
                    .AddConsoleFormatter<BasicConsoleFormatter, ConsoleFormatterOptions>();

                    if (logDebug)
                    {
                        b.SetMinimumLevel(LogLevel.Debug);
                    }
                });

                serviceCollection.AddToolsGenerator();

                var serviceProvider = serviceCollection.BuildServiceProvider();
                await using var _ = serviceProvider.ConfigureAwait(false);

                await commandHandler(parseResult, serviceProvider).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Run generator command.
        /// </summary>
        private async Task<int> RunGeneratorCommandHandlerAsync(ParseResult parseResult, IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<GeneratorCommand>>();

            var projectFile = parseResult.GetValue(this.projectArgument);

            if (!projectFile.Exists)
            {
#pragma warning disable CA1848 // Use the LoggerMessage delegates
                logger.LogError("Could not find project file {ProjectFile}", projectFile);
#pragma warning restore CA1848 // Use the LoggerMessage delegates
                return -1;
            }

            var useRelaxedJsonEscapingOptionValue = parseResult.GetValue(this.useRelaxedJsonEscaping);

            var useMultiLineOptionValue = parseResult.GetValue(this.useMultiLine);

            var registerEmbeddedResourceOptionValue = parseResult.GetValue(this.registerEmbeddedResource);
            var disableCompileOptionValue = parseResult.GetValue(this.disableCompile);

            var generator = serviceProvider.GetRequiredService<ILocalizationGenerator>();
            var results = generator.Generate(
                projectFile.FullName,
                new GeneratorOptions(
                    useRelaxedJsonEscapingOptionValue,
                    useMultiLineOptionValue,
                    registerEmbeddedResourceOptionValue,
                    !disableCompileOptionValue));

            var full = projectFile.Directory?.FullName ?? Environment.CurrentDirectory;

            var outputResourceFile = parseResult.GetValue(this.outputResourceOption);
            if (outputResourceFile != null)
            {
                await File.WriteAllLinesAsync(outputResourceFile.FullName, results.GeneratedResourceFiles.Select(f => Path.GetRelativePath(full, f))).ConfigureAwait(false);
            }

            var outputCodeFile = parseResult.GetValue(this.outputCodeOption);
            if (outputCodeFile != null)
            {
                await File.WriteAllLinesAsync(outputCodeFile.FullName, results.GeneratedCodeFiles.Select(f => Path.GetRelativePath(full, f))).ConfigureAwait(false);
            }

#pragma warning disable CA1848 // Use the LoggerMessage delegates
            logger.LogInformation($"Generation competed.");
#pragma warning restore CA1848 // Use the LoggerMessage delegates

            return 0;
        }
    }

#pragma warning disable CA1812
    /// <summary>
    /// Basic console formatter.
    /// </summary>
    internal sealed class BasicConsoleFormatter : ConsoleFormatter
    {
        /// <summary>
        /// Formatter name.
        /// </summary>
        public const string Name = "basic_console";

        /// <summary>
        /// Setup formatter instance.
        /// </summary>
        public BasicConsoleFormatter()
            : base(Name)
        {
        }

        /// <inheritdoc/>
        public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
        {
#pragma warning disable IDE0072 // Add missing cases
            var prefix = logEntry.LogLevel switch
            {
                LogLevel.Critical => "Critical: ",
                LogLevel.Error => "Error: ",
                LogLevel.Warning => "Warning: ",
                _ => string.Empty,
            };
#pragma warning restore IDE0072 // Add missing cases

            var txt = prefix + logEntry.Formatter(logEntry.State, logEntry.Exception);

#pragma warning disable CA1062 // Validate arguments of public methods
            textWriter.WriteLine(txt);
#pragma warning restore CA1062 // Validate arguments of public methods
        }
    }
#pragma warning restore CA1812

}
