// ----------------------------------------------------------------------
// <copyright file="Program.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SoloX.BlazorJsonLocalization.Tools.Core;
using SoloX.BlazorJsonLocalization.Tools.Extensions;

namespace SoloX.BlazorJsonLocalization.Tools
{
    /// <summary>
    /// CLI Tools Generator.
    /// </summary>
    public class Program
    {
        private readonly ILogger<Program> logger;
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Program"/> class.
        /// </summary>
        /// <param name="configuration">The configuration that contains all arguments.</param>
        public Program(IConfiguration configuration)
        {
            this.configuration = configuration;

            IServiceCollection sc = new ServiceCollection();

            sc.AddLogging(b => b.AddConsole());
            sc.AddSingleton(configuration);
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
        public static int Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddCommandLine(args);
            var config = builder.Build();

            return new Program(config).Run();
        }

        /// <summary>
        /// Run the tools command.
        /// </summary>
        /// <returns>Error code.</returns>
        public int Run()
        {
            var projectFile = this.configuration.GetValue<string>("project");

            if (string.IsNullOrEmpty(projectFile))
            {
#pragma warning disable CA1848 // Use the LoggerMessage delegates
                this.logger.LogError($"Missing project file parameter.");
#pragma warning restore CA1848 // Use the LoggerMessage delegates
                return -1;
            }

            if (!File.Exists(projectFile))
            {
#pragma warning disable CA1848 // Use the LoggerMessage delegates
#pragma warning disable CA2254 // Template should be a static expression
                this.logger.LogError($"Could not find project file {projectFile}");
#pragma warning restore CA2254 // Template should be a static expression
#pragma warning restore CA1848 // Use the LoggerMessage delegates
                return -1;
            }

            var generator = this.Service.GetService<IToolsGenerator>();
            generator.Generate(projectFile);

            return 0;
        }
    }
}
