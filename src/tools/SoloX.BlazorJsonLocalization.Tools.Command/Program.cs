using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SoloX.BlazorJsonLocalization.Tools.Core;
using SoloX.BlazorJsonLocalization.Tools.Extensions;
using System;
using System.IO;

namespace SoloX.BlazorJsonLocalization.Tools
{
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

            this.logger = this.Service.GetService<ILogger<Program>>();
        }

        private ServiceProvider Service { get; }

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
                this.logger.LogError($"Missing project file parameter.");
                return -1;
            }

            if (!File.Exists(projectFile))
            {
                this.logger.LogError($"Could not find project file {projectFile}");
                return -1;
            }

            var generator = this.Service.GetService<IToolsGenerator>();
            generator.Generate(projectFile);

            return 0;
        }
    }
}
