// ----------------------------------------------------------------------
// <copyright file="GeneratorCommand.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using SoloX.BlazorJsonLocalization.Tools.Core;
using SoloX.BlazorJsonLocalization.Tools.Extensions;
using Serilog;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Globalization;

namespace SoloX.BlazorJsonLocalization.Tools
{
    /// <summary>
    /// Generator command.
    /// </summary>
    public class GeneratorCommand
    {
        private readonly RootCommand rootCommand = new RootCommand("Localization generation command.");

        private readonly Argument<FileInfo> projectArgument = new Argument<FileInfo>("projectFile", "CS Project to run generation.");
        private readonly Option<FileInfo> outputCodeOption = new Option<FileInfo>("--outputCode", "File to store CS output file list.");
        private readonly Option<FileInfo> outputResourceOption = new Option<FileInfo>("--outputResource", "File to store Json output resource file list.");
        private readonly Option<FileInfo> logFileOption = new Option<FileInfo>("--logFile", "File to write logging.");
        private readonly Option<bool> logDebugOption = new Option<bool>("--logDebug", "Enable debug logging.");

        private readonly Command versionCommand = new Command("version", "Display software version.");

        /// <summary>
        /// Setup GeneratorCommand instance.
        /// </summary>
        public GeneratorCommand()
        {
            this.rootCommand.AddGlobalOption(this.logFileOption);
            this.rootCommand.AddGlobalOption(this.logDebugOption);

            this.rootCommand.AddArgument(this.projectArgument);
            this.rootCommand.AddOption(this.outputCodeOption);
            this.rootCommand.AddOption(this.outputResourceOption);

            this.rootCommand.SetHandler(RunGeneratorCommandHandlerAsync);

            this.versionCommand.SetHandler(RunVersionCommandHandlerAsync);

            this.rootCommand.AddCommand(this.versionCommand);
        }

        /// <summary>
        /// Run the command.
        /// </summary>
        /// <param name="args">Command arguments.</param>
        /// <returns></returns>
        public async Task RunGeneratorCommandAsync(string[] args)
        {
            await this.rootCommand.InvokeAsync(args).ConfigureAwait(false);
        }

        /// <summary>
        /// Run command
        /// </summary>
        private Task RunGeneratorCommandHandlerAsync(InvocationContext invocationContext)
        {
            return SetupServiceCollectionAndRunCommandAsync(invocationContext, RunGeneratorCommandHandlerAsync);
        }

        private Task RunVersionCommandHandlerAsync(InvocationContext invocationContext)
        {
            return SetupServiceCollectionAndRunCommandAsync(invocationContext, RunVersionCommandHandlerAsync);
        }

        private async Task SetupServiceCollectionAndRunCommandAsync(InvocationContext invocationContext, Func<InvocationContext, IServiceProvider, Task> commandHandler)
        {
            var serviceCollection = new ServiceCollection();

            var logDebug = invocationContext.BindingContext.ParseResult.GetValueForOption(this.logDebugOption);

            var logFile = invocationContext.BindingContext.ParseResult.GetValueForOption(this.logFileOption);
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

                await commandHandler(invocationContext, serviceProvider).ConfigureAwait(false);
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

                await commandHandler(invocationContext, serviceProvider).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Run version command.
        /// </summary>
        private static Task RunVersionCommandHandlerAsync(InvocationContext invocationContext, IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<GeneratorCommand>>();

#pragma warning disable CA1848 // Use the LoggerMessage delegates
#pragma warning disable CA2254 // Template should be a static expression
            logger.LogInformation("v2.0.3");
#pragma warning restore CA2254 // Template should be a static expression
#pragma warning restore CA1848 // Use the LoggerMessage delegates

            return Task.CompletedTask;
        }

        /// <summary>
        /// Run generator command.
        /// </summary>
        private async Task RunGeneratorCommandHandlerAsync(InvocationContext invocationContext, IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<GeneratorCommand>>();

            var projectFile = invocationContext.BindingContext.ParseResult.GetValueForArgument(this.projectArgument);

            if (!projectFile.Exists)
            {
#pragma warning disable CA1848 // Use the LoggerMessage delegates
#pragma warning disable CA2254 // Template should be a static expression
                logger.LogError($"Could not find project file {projectFile}");
#pragma warning restore CA2254 // Template should be a static expression
#pragma warning restore CA1848 // Use the LoggerMessage delegates
                invocationContext.ExitCode = -1;
                return;
            }

            var generator = serviceProvider.GetRequiredService<ILocalizationGenerator>();
            var results = generator.Generate(projectFile.FullName);

            var full = projectFile.Directory?.FullName ?? Environment.CurrentDirectory;

            var outputResourceFile = invocationContext.BindingContext.ParseResult.GetValueForOption(this.outputResourceOption);
            if (outputResourceFile != null)
            {
                await File.WriteAllLinesAsync(outputResourceFile.FullName, results.GeneratedResourceFiles.Select(f => Path.GetRelativePath(full, f))).ConfigureAwait(false);
            }

            var outputCodeFile = invocationContext.BindingContext.ParseResult.GetValueForOption(this.outputCodeOption);
            if (outputCodeFile != null)
            {
                await File.WriteAllLinesAsync(outputCodeFile.FullName, results.GeneratedCodeFiles.Select(f => Path.GetRelativePath(full, f))).ConfigureAwait(false);
            }

#pragma warning disable CA1848 // Use the LoggerMessage delegates
            logger.LogInformation($"Generation competed.");
#pragma warning restore CA1848 // Use the LoggerMessage delegates
        }
    }

    /// <summary>
    /// Basic console formatter.
    /// </summary>
    public class BasicConsoleFormatter : ConsoleFormatter
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
            var txt = logEntry.Formatter(logEntry.State, logEntry.Exception);

#pragma warning disable CA1062 // Validate arguments of public methods
            textWriter.WriteLine(txt);
#pragma warning restore CA1062 // Validate arguments of public methods
        }
    }
}
