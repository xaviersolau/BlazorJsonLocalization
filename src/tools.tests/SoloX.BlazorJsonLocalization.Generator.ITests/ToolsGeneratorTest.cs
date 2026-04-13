// ----------------------------------------------------------------------
// <copyright file="ToolsGeneratorTest.cs" company="Xavier Solau">
// Copyright © 2021-2026 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.Localization;
using SoloX.BlazorJsonLocalization.Attributes;
using SoloX.CodeQuality.Test.Helpers.XUnit;
using SoloX.GeneratorTools.Core.Test.Helpers.Snapshot;
using SoloX.GeneratorTools.Test;
using Xunit.Abstractions;

namespace SoloX.BlazorJsonLocalization.Generator.ITests
{
    public class ToolsGeneratorTest
    {
        private readonly ITestOutputHelper testOutputHelper;

        public ToolsGeneratorTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

#pragma warning disable CA1861 // Avoid constant arrays as arguments
        [Theory]
        [InlineData(
            @"./SampleBasic/ISimpleLocalizer.cs", @"./Component.cs",
            "Resources", new string[] { "Component.json", "Component.en.json", "Component.fr.json" })]
#pragma warning restore CA1861 // Avoid constant arrays as arguments
        public void GenerateBasicLocalizer(string interfaceFile, string componentFile, string expectedResourcePath, string[] expectedJsonFiles)
        {
            var snapshotName = nameof(this.GenerateBasicLocalizer)
                + Path.GetFileNameWithoutExtension(interfaceFile);

            GenerateSnapshot(snapshotName, expectedResourcePath, expectedJsonFiles, interfaceFile, componentFile);
        }

        private static void GenerateSnapshot(string snapshotName, string expectedResourcePath, string[] expectedJsonFiles, params string[] files)
        {
            SnapshotHelper.IsOverwriteEnable = true;

            var generator = new GeneratorHandler();

            var syntaxTrees = files.Select(f => CSharpSyntaxTree.ParseText(File.ReadAllText(f), path: f));

            var references = new PortableExecutableReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IStringLocalizer).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(LocalizerAttribute).Assembly.Location),
            };

            var compilation = CSharpCompilation.Create("Test", syntaxTrees, references);

            var projectFolder = "Folder";

            var props = new Dictionary<string, string>()
            {
                ["build_property.rootnamespace"] = @"MyNameSpace",
                ["build_property.targetframework"] = @"net9.0",
                ["build_property.projectdir"] = projectFolder,
            };

            var generatorDriver = CSharpGeneratorDriver.Create(
                [generator.AsSourceGenerator()],
                optionsProvider: new TestAnalyzerConfigOptionsProvider(props));

            var driver = generatorDriver.RunGenerators(compilation);

            var generatorResults = driver.GetRunResult();

            generatorResults.GeneratedTrees.Length.Should().Be(2);

            var snapshotGenerator = new SnapshotWriter();

            foreach (var generatedTree in generatorResults.GeneratedTrees)
            {
                var file = Path.GetFileName(generatedTree.FilePath);

                snapshotGenerator.Generate(file, file, w => w.Write(generatedTree.GetText().ToString()));
            }

            foreach (var expectedJsonFile in expectedJsonFiles)
            {
                var jsonFile = Path.Combine(projectFolder, expectedResourcePath, expectedJsonFile);
                File.Exists(jsonFile).Should().BeTrue("File does not exist {0}", jsonFile);
                snapshotGenerator.Generate(expectedJsonFile, expectedJsonFile, w => w.Write(File.ReadAllText(jsonFile)));
            }

            var location = SnapshotHelper.GetLocationFromCallingCodeProjectRoot(null);
            SnapshotHelper.AssertSnapshot(snapshotGenerator.GetAllGenerated(), snapshotName, location);
        }

        private sealed class TestAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
        {
            private sealed class TestAnalyzerConfigOptions : AnalyzerConfigOptions
            {
                private readonly IReadOnlyDictionary<string, string> keyValuePairs;

                public TestAnalyzerConfigOptions(IReadOnlyDictionary<string, string> keyValuePairs)
                {
                    this.keyValuePairs = keyValuePairs;
                }

                public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
                {
                    return this.keyValuePairs.TryGetValue(key, out value);
                }
            }

            private readonly TestAnalyzerConfigOptions testAnalyzerConfigOptions;

            public TestAnalyzerConfigOptionsProvider(IReadOnlyDictionary<string, string> keyValuePairs)
            {
                this.testAnalyzerConfigOptions = new TestAnalyzerConfigOptions(keyValuePairs);
            }

            public override AnalyzerConfigOptions GlobalOptions => this.testAnalyzerConfigOptions;

            public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
            {
                return this.testAnalyzerConfigOptions;
            }

            public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
            {
                return this.testAnalyzerConfigOptions;
            }
        }
    }
}