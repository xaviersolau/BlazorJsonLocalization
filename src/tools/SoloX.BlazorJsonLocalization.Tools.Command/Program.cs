// ----------------------------------------------------------------------
// <copyright file="Program.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SoloX.BlazorJsonLocalization.Tools.Core;
using SoloX.BlazorJsonLocalization.Tools.Extensions;
using System.CommandLine;

namespace SoloX.BlazorJsonLocalization.Tools
{
    /// <summary>
    /// CLI Tools Generator.
    /// </summary>
    public class Program
    {
        private readonly ILogger<Program> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Program"/> class.
        /// </summary>
        public Program()
        {
            IServiceCollection sc = new ServiceCollection();

            sc.AddLogging(b => b.AddConsole());
            sc.AddToolsGenerator();

            this.Service = sc.BuildServiceProvider();

            this.logger = this.Service.GetRequiredService<ILogger<Program>>();
        }

        private ServiceProvider Service { get; }

        /// <summary>
        /// CLI Tools Generator entry point.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task Main(string[] args)
        {
            await new Program().RunAsync(args).ConfigureAwait(false);
        }

        /// <summary>
        /// Run the tools command.
        /// </summary>
        /// <returns>Error code.</returns>
        public async Task RunAsync(string[] args)
        {
            var rootCommand = new RootCommand("Localization generation tool");

            var projectArgument = new Argument<FileInfo>("projectFile", "CS Project to run generation.");

            rootCommand.AddArgument(projectArgument);

            rootCommand.SetHandler(
                ctx =>
                {
                    var projectFile = ctx.BindingContext.ParseResult.GetValueForArgument(projectArgument);

                    if (!projectFile.Exists)
                    {
#pragma warning disable CA1848 // Use the LoggerMessage delegates
#pragma warning disable CA2254 // Template should be a static expression
                        this.logger.LogError($"Could not find project file {projectFile}");
#pragma warning restore CA2254 // Template should be a static expression
#pragma warning restore CA1848 // Use the LoggerMessage delegates
                        ctx.ExitCode = -1;
                    }

                    var generator = this.Service.GetService<IToolsGenerator>();
                    generator.Generate(projectFile.FullName);

                });

            await rootCommand.InvokeAsync(args).ConfigureAwait(false);
        }
    }
}
