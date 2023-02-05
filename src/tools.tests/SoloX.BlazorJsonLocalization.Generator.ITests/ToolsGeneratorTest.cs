// ----------------------------------------------------------------------
// <copyright file="ToolsGeneratorTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

        [Theory]
        [InlineData(
            @"Samples/Sample1/ISimpleLocalizer.cs", @"Samples/Sample1/Component.cs",
            "Resources/Samples/Sample1", new string[] { "Component.json", "Component-en.json", "Component-fr.json" })]
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

            var generatorDriver = CSharpGeneratorDriver.Create(generator);

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
                var jsonFile = expectedResourcePath + '/' + expectedJsonFile;
                File.Exists(jsonFile).Should().BeTrue();
                snapshotGenerator.Generate(expectedJsonFile, expectedJsonFile, w => w.Write(File.ReadAllText(jsonFile)));
            }

            var location = SnapshotHelper.GetLocationFromCallingCodeProjectRoot(null);
            SnapshotHelper.AssertSnapshot(snapshotGenerator.GetAllGenerated(), snapshotName, location);
        }
    }
}