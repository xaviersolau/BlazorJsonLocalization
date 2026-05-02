// ----------------------------------------------------------------------
// <copyright file="ToolsCommandTest.cs" company="Xavier Solau">
// Copyright © 2021-2026 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Shouldly;
using SoloX.CodeQuality.Test.Helpers;
using SoloX.CodeQuality.Test.Helpers.Solution;
using SoloX.GeneratorTools.Core.Test.Helpers;

namespace SoloX.BlazorJsonLocalization.Tools.Command.ITests
{
    public class ToolsCommandTest
    {
        [Fact]
        public async Task ItShouldGenerateLocalizerFiles()
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
                                .AddNugetOrg();
                        });
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
                var generatorCommand = new GeneratorCommand();

                var projectFile = Path.Combine(root, solutionName, projectName, $"{projectName}.csproj");

                var res = await generatorCommand.RunGeneratorCommandAsync([projectFile, "--registerEmbeddedResource"]);

                res.ShouldBe(0);

                var processResult = solution.Build(configuration: configurationName);

                processResult.ExitCode.ShouldBe(0);

                var objPath = Path.Combine(root, solutionName, projectName, "obj");
                var resourcePath = Path.Combine(root, solutionName, projectName, "Resources");
                var binaryPath = Path.Combine(root, solutionName, projectName, $"bin", configurationName, framework);

                IEnumerable<string> generatedFiles = [
                    Path.Combine(objPath, $"Impl/TestStringLocalizer.g.cs"),
                    Path.Combine(objPath, $"Impl/TestStringLocalizer.g.cs"),
                    Path.Combine(objPath, $"SampleProject.csproj.LocalizationGenerator.Code.g.props"),
                    Path.Combine(objPath, $"SampleProject.csproj.LocalizationGenerator.Resource.g.props")
                    ];

                generatedFiles.Select(File.Exists)
                    .ShouldNotContain(false, "All files must be generated in obj folder.");

                IEnumerable<string> generatedResources = [
                    Path.Combine(resourcePath, $"Test.en.json"),
                    Path.Combine(resourcePath, $"Test.fr.json"),
                    Path.Combine(resourcePath, $"Test.json")
                    ];

                generatedResources.Select(File.Exists)
                    .ShouldNotContain(false, "All Json files must be generated in Resources folder.");

                IEnumerable<string> assemblyResourceFiles = [
                    Path.Combine(binaryPath, "fr", $"{projectName}.resources.dll"),
                    Path.Combine(binaryPath, "en", $"{projectName}.resources.dll")
                    ];

                assemblyResourceFiles.Select(File.Exists)
                    .ShouldNotContain(false, "Satellite resource assemblies must be generated.");

                var assemblyFile = Path.Combine(binaryPath, $"{projectName}.dll");

                MetadataAssemblyProbe.LoadMetadataAssemblyAndAssert(assemblyFile, decResolver =>
                {
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
