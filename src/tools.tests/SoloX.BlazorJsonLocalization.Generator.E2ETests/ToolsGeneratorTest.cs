// ----------------------------------------------------------------------
// <copyright file="ToolsGeneratorTest.cs" company="Xavier Solau">
// Copyright © 2021-2026 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Shouldly;
using SoloX.CodeQuality.Test.Helpers;
using SoloX.CodeQuality.Test.Helpers.Solution;
using SoloX.GeneratorTools.Core.Test.Helpers;
using Xunit.Abstractions;

namespace SoloX.BlazorJsonLocalization.Generator.E2ETests
{
    public class ToolsGeneratorTest
    {
        private readonly ITestOutputHelper testOutputHelper;

        public ToolsGeneratorTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ItShouldGenerateLocalizerFiles()
        {
            var configurationName = ProjectBinProbe.GetConfiguration<ToolsGeneratorTest>();
            var framework = ProjectBinProbe.GetFramework<ToolsGeneratorTest>();

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
                                .Add(@$"../../../../../../tools/SoloX.BlazorJsonLocalization.Tools.Core/bin/{configurationName}")
                                .Add(@$"../../../../../../tools/SoloX.BlazorJsonLocalization.Generator/bin/{configurationName}")
                                .AddNugetOrg();
                        });
                })
                .WithProject(projectName, "classlib", framework, configuration =>
                {
                    configuration
                        .UsePackageReference("Microsoft.Extensions.Localization.Abstractions")
                        .UsePackageReference("SoloX.BlazorJsonLocalization.Attributes")
                        .UsePackageReference("SoloX.BlazorJsonLocalization.Generator")
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
                var actResult = solution.Build(configuration: configurationName);

                actResult.ExitCode.ShouldBe(0);

                this.testOutputHelper.WriteLine(actResult.GetLogs());

                var assemblyFile = Path.Combine(root, solutionName, projectName, "bin", configurationName, framework, $"{projectName}.dll");

                MetadataAssemblyProbe.LoadMetadataAssemblyAndAssert(assemblyFile, decResolver =>
                {
                    var decl = decResolver.Find("SampleProject.ITestStringLocalizer");

                    decl.ShouldNotBeNull();

                    var generatedDecl = decResolver.Find("SampleProject.TestStringLocalizer");

                    generatedDecl.ShouldNotBeNull();
                });
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }
    }
}