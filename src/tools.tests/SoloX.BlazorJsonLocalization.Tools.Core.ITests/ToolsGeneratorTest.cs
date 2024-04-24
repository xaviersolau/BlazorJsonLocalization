// ----------------------------------------------------------------------
// <copyright file="ToolsGeneratorTest.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using SoloX.BlazorJsonLocalization.Tools.Core.Impl;
using SoloX.BlazorJsonLocalization.Tools.Extensions;
using SoloX.CodeQuality.Test.Helpers.XUnit;
using SoloX.GeneratorTools.Core.CSharp.Workspace;
using SoloX.GeneratorTools.Core.Generator.Impl;
using SoloX.GeneratorTools.Core.Test.Helpers.Snapshot;
using SoloX.GeneratorTools.Core.Utils;
using System.Text;
using Xunit.Abstractions;

namespace SoloX.BlazorJsonLocalization.Tools.Core.ITests
{
    public class ToolsGeneratorTest
    {
        private readonly ITestOutputHelper testOutputHelper;

        public ToolsGeneratorTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData(@"Sample1/ISimpleLocalizer.cs", 0, @"Component.cs", null)]
        public void GenerateBasicLocalizer(string interfaceFile, int idx, string componentFile, string jsonLocalization)
        {
            var snapshotName = nameof(this.GenerateBasicLocalizer)
                + Path.GetFileNameWithoutExtension(interfaceFile) + idx;

            this.GenerateSnapshot(snapshotName, jsonLocalization, false, false, interfaceFile, componentFile);
        }

        private const string SimpleSubJson1 = @"
        {
          ""SubLocalizer1"": {
            ""BasicSubProperty1"": ""SubLocalizer1BasicSubProperty1ValueFor{name}"",
            ""BasicSubProperty2"": ""SubLocalizer1BasicSubProperty2ValueFor{name}""
          },
          ""SubLocalizer2"": {
            ""BasicSubProperty1"": ""SubLocalizer2BasicSubProperty1ValueFor{name}"",
            ""BasicSubProperty2"": ""SubLocalizer2BasicSubProperty2ValueFor{name}""
          },
          ""BasicProperty"": ""BasicPropertyValueFor{name}""
        }";

        private const string SimpleSubJson2 = @"
        {
          ""SubLocalizer1"":  ""SubLocalizer1BasicSubProperty1ValueFor{name}"",
          ""SubLocalizer2"": {
            ""BasicSubProperty1"": ""SubLocalizer2BasicSubProperty1ValueFor{name}"",
            ""BasicSubProperty2"": ""SubLocalizer2BasicSubProperty2ValueFor{name}""
          },
          ""BasicProperty"": ""BasicPropertyValueFor{name}"",
          ""BasicPropertyUnused"": ""BasicPropertyValueFor{name}""
        }";

        private const string SimpleSubJson3 = @"Bad Json";

        private const string MultiLineJson1 = @"
        {
          ""BasicProperty"":  ""\n            Word11\n            word12\n            word13.""
        }";

        private const string MultiLineJson2 = @"
        {
          ""BasicProperty"":  [
            ""            Word21"",
            ""            word22"",
            ""            word23.""
          ]
        }";

        [Theory]
        [InlineData(@"Sample2/ISimpleLocalizer.cs", 0, @"Sample2/ISimpleSubLocalizer.cs", @"Component.cs", null)]
        [InlineData(@"Sample2/ISimpleLocalizer.cs", 1, @"Sample2/ISimpleSubLocalizer.cs", @"Component.cs", SimpleSubJson1)]
        [InlineData(@"Sample2/ISimpleLocalizer.cs", 2, @"Sample2/ISimpleSubLocalizer.cs", @"Component.cs", SimpleSubJson2)]
        [InlineData(@"Sample2/ISimpleLocalizer.cs", 3, @"Sample2/ISimpleSubLocalizer.cs", @"Component.cs", SimpleSubJson3)]
        public void GenerateSubLocalizer(string interfaceFile, int idx, string subInterfaceFile, string componentFile, string jsonLocalization)
        {
            var snapshotName = nameof(this.GenerateSubLocalizer)
                + Path.GetFileNameWithoutExtension(interfaceFile) + idx;

            this.GenerateSnapshot(snapshotName, jsonLocalization, false, false, interfaceFile, subInterfaceFile, componentFile);
        }

        [Theory]
        [InlineData(@"Sample3/ISimpleLocalizer.cs", 0, @"Sample3/ISimpleSubLocalizer.cs", @"Component.cs", null)]
        [InlineData(@"Sample3/ISimpleLocalizer.cs", 1, @"Sample3/ISimpleSubLocalizer.cs", @"Component.cs", SimpleSubJson1)]
        public void GenerateSubLocalizerWithTranslate(string interfaceFile, int idx, string subInterfaceFile, string componentFile, string jsonLocalization)
        {
            var snapshotName = nameof(this.GenerateSubLocalizerWithTranslate)
                + Path.GetFileNameWithoutExtension(interfaceFile) + idx;

            this.GenerateSnapshot(snapshotName, jsonLocalization, false, false, interfaceFile, subInterfaceFile, componentFile);
        }

        [Theory]
        [InlineData(@"Sample4/ISimpleLocalizer.cs", 0, @"Component.cs")]
        public void GenerateBasicLocalizerWithTranslateAndSpecialChars(string interfaceFile, int idx, string componentFile)
        {
            var snapshotName = nameof(this.GenerateBasicLocalizerWithTranslateAndSpecialChars)
                + Path.GetFileNameWithoutExtension(interfaceFile) + idx;

            this.GenerateSnapshot(snapshotName, null, true, false, interfaceFile, componentFile);
        }

        [Theory]
        [InlineData(@"Sample5/IMultiLineLocalizer.cs", 0, @"Component.cs", false, null)]
        [InlineData(@"Sample5/IMultiLineLocalizer.cs", 1, @"Component.cs", true, null)]
        [InlineData(@"Sample5/IMultiLineLocalizer.cs", 2, @"Component.cs", false, MultiLineJson1)]
        [InlineData(@"Sample5/IMultiLineLocalizer.cs", 3, @"Component.cs", true, MultiLineJson2)]
        [InlineData(@"Sample5/IMultiLineLocalizer.cs", 4, @"Component.cs", false, MultiLineJson2)]
        [InlineData(@"Sample5/IMultiLineLocalizer.cs", 5, @"Component.cs", true, MultiLineJson1)]
        public void GenerateBasicLocalizerWithTranslateAndMultiLine(string interfaceFile, int idx, string componentFile, bool multiLine, string jsonLocalization)
        {
            var snapshotName = nameof(this.GenerateBasicLocalizerWithTranslateAndMultiLine)
                + Path.GetFileNameWithoutExtension(interfaceFile) + idx;

            this.GenerateSnapshot(snapshotName, jsonLocalization, true, multiLine, interfaceFile, componentFile);
        }

        private void GenerateSnapshot(string snapshotName, string jsonLocalization, bool useRelaxedJsonEscaping, bool useMultiLine, params string[] files)
        {
            SnapshotHelper.IsOverwriteEnable = true;

            var sc = new ServiceCollection();
            sc.AddTestLogging(this.testOutputHelper);
            sc.AddToolsGenerator();

            using (var sp = sc.BuildServiceProvider())
            {
                var workspaceFactory = sp.GetService<ICSharpWorkspaceFactory>();

                var workspace = workspaceFactory.CreateWorkspace();

                foreach (var file in files)
                {
                    workspace.RegisterFile(file);
                }

                var generator = new LocalizationGenerator(
                    sp.GetService<IGeneratorLogger<LocalizationGenerator>>(),
                    workspaceFactory);

                var inputs = new HashSet<string>();
                var locator = new RelativeLocator(string.Empty, "target.name.space");

                var snapshotWriter = new SnapshotWriter();
                var jsonReader = new TestReader(jsonLocalization);

                generator.Generate(workspace, locator, snapshotWriter, locator, jsonReader, snapshotWriter, workspace.Files, new GeneratorOptions(useRelaxedJsonEscaping, useMultiLine) { NewLineSeparator = "\n" });

                var location = SnapshotHelper.GetLocationFromCallingCodeProjectRoot(null);
                SnapshotHelper.AssertSnapshot(snapshotWriter.GetAllGenerated(), snapshotName, location);
            }
        }
    }

    public class TestReader : IReader
    {
        private readonly string text;

        public TestReader(string text)
        {
            this.text = text;
        }

        public void Read(string location, string name, Action<Stream> reader)
        {
            if (!string.IsNullOrEmpty(this.text))
            {
                var stream = new MemoryStream();

                var bytes = Encoding.UTF8.GetBytes(this.text.Replace("{name}", name, StringComparison.InvariantCulture));

                stream.Write(bytes);

                stream.Position = 0;

                reader(stream);
            }
        }
    }
}