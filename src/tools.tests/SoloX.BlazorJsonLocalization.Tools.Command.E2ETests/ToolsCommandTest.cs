// ----------------------------------------------------------------------
// <copyright file="ToolsCommandTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using FluentAssertions;
using SoloX.CodeQuality.Test.Helpers;
using SoloX.CodeQuality.Test.Helpers.Solution;
using SoloX.GeneratorTools.Core.Test.Helpers;

namespace SoloX.BlazorJsonLocalization.Tools.Command.E2ETests
{
    public class ToolsCommandTest
    {
        [Fact]
        public void ItShouldGenerateLocalizerFiles()
        {
            var configurationName = ProjectBinProbe.GetConfiguration<ToolsCommandTest>();
            var framework = ProjectBinProbe.GetFramework<ToolsCommandTest>();

            var root = new RandomGenerator().RandomString(4);

            var solutionName = "GeneratorTest";
            var projectName = "SampleProject";

            IEnumerable<(string key, string value)> replaceTxt = [
                ("SoloX.BlazorJsonLocalization.Generator.Sample", projectName),
                ];

            var solution = new SolutionBuilder(root, solutionName)
                .WithNugetConfig(@"PkgCache", configuration =>
                {
                    configuration
                        .UsePackageSources(src =>
                        {
                            src.Clear()
                                .Add(@$"../../../../../../libs/SoloX.BlazorJsonLocalization.Attributes/bin/{configurationName}")
                                .Add(@$"../../../../../../tools/SoloX.BlazorJsonLocalization.Tools.Command/bin/{configurationName}")
                                .AddNugetOrg();
                        });
                })
                .WithDotnetTools(configuration =>
                {
                    configuration.UseTool("SoloX.BlazorJsonLocalization.Tools.Command");
                })
                .WithProject(projectName, "classlib", framework, configuration =>
                {
                    configuration
                        .UsePackageReference("Microsoft.Extensions.Localization.Abstractions")
                        .UsePackageReference("SoloX.BlazorJsonLocalization.Attributes")
                        .UseFiles(files =>
                        {
                            files
                                .Remove("Class1.cs")
                                .Add(@"../Resources/ITestStringLocalizer.cs", "ITestStringLocalizer.cs", replaceTxt)
                                .Add(@"../Resources/Test.cs", "Test.cs", replaceTxt);
                        });
                })
                .Build();

            try
            {
                var actResult = solution.RunTool("localizationgen", $"{projectName}.csproj", projectName);

                actResult.ExitCode.Should().Be(0);

                var processResult = solution.Build(configuration: configurationName);

                processResult.ExitCode.Should().Be(0);

                var assemblyFile = Path.Combine(root, solutionName, projectName, "bin", configurationName, framework, $"{projectName}.dll");

                MetadataAssemblyProbe.LoadMetadataAssemblyAndAssert(assemblyFile, decResolver =>
                {
                    var generatedDecl = decResolver.Find("SampleProject.TestStringLocalizer");

                    generatedDecl.Should().NotBeNull();
                });
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }
    }
}
