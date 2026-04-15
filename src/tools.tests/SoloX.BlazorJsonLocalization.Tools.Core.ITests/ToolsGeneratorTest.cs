// ----------------------------------------------------------------------
// <copyright file="ToolsGeneratorTest.cs" company="Xavier Solau">
// Copyright © 2021-2026 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Text;
using Microsoft.Extensions.DependencyInjection;
using SoloX.BlazorJsonLocalization.Tools.Core.Impl;
using SoloX.BlazorJsonLocalization.Tools.Extensions;
using SoloX.CodeQuality.Test.Helpers.Snapshot;
using SoloX.CodeQuality.Test.Helpers.XUnit;
using SoloX.GeneratorTools.Core.CSharp.Workspace;
using SoloX.GeneratorTools.Core.Generator.Impl;
using SoloX.GeneratorTools.Core.Test.Helpers.Snapshot;
using SoloX.GeneratorTools.Core.Utils;
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
        [InlineData(@"SampleBasic/ISimpleLocalizer.cs", 0, @"Component.cs", null)]
        public Task GenerateBasicLocalizerAsync(string interfaceFile, int idx, string componentFile, string jsonLocalization)
        {
            var snapshotName = nameof(this.GenerateBasicLocalizerAsync)
                + Path.GetFileNameWithoutExtension(interfaceFile) + idx;

            return this.GenerateSnapshotAsync(snapshotName, jsonLocalization, false, false, interfaceFile, componentFile);
        }

        [Theory]
        [InlineData(@"SampleMethodArg/ISimpleLocalizer.cs", 0, @"Component.cs", null)]
        public Task GenerateLocalizerMethodWithArgAsync(string interfaceFile, int idx, string componentFile, string jsonLocalization)
        {
            var snapshotName = nameof(this.GenerateLocalizerMethodWithArgAsync)
                + Path.GetFileNameWithoutExtension(interfaceFile) + idx;

            return this.GenerateSnapshotAsync(snapshotName, jsonLocalization, false, false, interfaceFile, componentFile);
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
        [InlineData(@"SampleWithSubLocalizer/ISimpleLocalizer.cs", 0, @"SampleWithSubLocalizer/ISimpleSubLocalizer.cs", @"Component.cs", null)]
        [InlineData(@"SampleWithSubLocalizer/ISimpleLocalizer.cs", 1, @"SampleWithSubLocalizer/ISimpleSubLocalizer.cs", @"Component.cs", SimpleSubJson1)]
        [InlineData(@"SampleWithSubLocalizer/ISimpleLocalizer.cs", 2, @"SampleWithSubLocalizer/ISimpleSubLocalizer.cs", @"Component.cs", SimpleSubJson2)]
        [InlineData(@"SampleWithSubLocalizer/ISimpleLocalizer.cs", 3, @"SampleWithSubLocalizer/ISimpleSubLocalizer.cs", @"Component.cs", SimpleSubJson3)]
        public Task GenerateSubLocalizerAsync(string interfaceFile, int idx, string subInterfaceFile, string componentFile, string jsonLocalization)
        {
            var snapshotName = nameof(this.GenerateSubLocalizerAsync)
                + Path.GetFileNameWithoutExtension(interfaceFile) + idx;

            return this.GenerateSnapshotAsync(snapshotName, jsonLocalization, false, false, interfaceFile, subInterfaceFile, componentFile);
        }

        [Theory]
        [InlineData(@"SampleSubLocalizerName/ISimple.cs", 0, @"SampleSubLocalizerName/ISimpleSub.cs", @"Component.cs", null)]
        public Task GenerateSubLocalizerNameAsync(string interfaceFile, int idx, string subInterfaceFile, string componentFile, string jsonLocalization)
        {
            var snapshotName = nameof(this.GenerateSubLocalizerNameAsync)
                + Path.GetFileNameWithoutExtension(interfaceFile) + idx;

            return this.GenerateSnapshotAsync(snapshotName, jsonLocalization, false, false, interfaceFile, subInterfaceFile, componentFile);
        }

        [Theory]
        [InlineData(@"SampleWithSubLocalizerTranslate/ISimpleLocalizer.cs", 0, @"SampleWithSubLocalizerTranslate/ISimpleSubLocalizer.cs", @"Component.cs", null)]
        [InlineData(@"SampleWithSubLocalizerTranslate/ISimpleLocalizer.cs", 1, @"SampleWithSubLocalizerTranslate/ISimpleSubLocalizer.cs", @"Component.cs", SimpleSubJson1)]
        public Task GenerateSubLocalizerWithTranslateAsync(string interfaceFile, int idx, string subInterfaceFile, string componentFile, string jsonLocalization)
        {
            var snapshotName = nameof(this.GenerateSubLocalizerWithTranslateAsync)
                + Path.GetFileNameWithoutExtension(interfaceFile) + idx;

            return this.GenerateSnapshotAsync(snapshotName, jsonLocalization, false, false, interfaceFile, subInterfaceFile, componentFile);
        }

        [Theory]
        [InlineData(@"SampleWithTranslate/ISimpleLocalizer.cs", 0, @"Component.cs")]
        public Task GenerateBasicLocalizerWithTranslateAndSpecialCharsAsync(string interfaceFile, int idx, string componentFile)
        {
            var snapshotName = nameof(this.GenerateBasicLocalizerWithTranslateAndSpecialCharsAsync)
                + Path.GetFileNameWithoutExtension(interfaceFile) + idx;

            return this.GenerateSnapshotAsync(snapshotName, null, true, false, interfaceFile, componentFile);
        }

        [Theory]
        [InlineData(@"SampleWithMultiLine/IMultiLineLocalizer.cs", 0, @"Component.cs", false, null)]
        [InlineData(@"SampleWithMultiLine/IMultiLineLocalizer.cs", 1, @"Component.cs", true, null)]
        [InlineData(@"SampleWithMultiLine/IMultiLineLocalizer.cs", 2, @"Component.cs", false, MultiLineJson1)]
        [InlineData(@"SampleWithMultiLine/IMultiLineLocalizer.cs", 3, @"Component.cs", true, MultiLineJson2)]
        [InlineData(@"SampleWithMultiLine/IMultiLineLocalizer.cs", 4, @"Component.cs", false, MultiLineJson2)]
        [InlineData(@"SampleWithMultiLine/IMultiLineLocalizer.cs", 5, @"Component.cs", true, MultiLineJson1)]
        public Task GenerateBasicLocalizerWithTranslateAndMultiLineAsync(string interfaceFile, int idx, string componentFile, bool multiLine, string jsonLocalization)
        {
            var snapshotName = nameof(this.GenerateBasicLocalizerWithTranslateAndMultiLineAsync)
                + Path.GetFileNameWithoutExtension(interfaceFile) + idx;

            return this.GenerateSnapshotAsync(snapshotName, jsonLocalization, true, multiLine, interfaceFile, componentFile);
        }

        private Task GenerateSnapshotAsync(string snapshotName, string jsonLocalization, bool useRelaxedJsonEscaping, bool useMultiLine, params string[] files)
        {
            var sc = new ServiceCollection();
            sc.AddTestLogging(this.testOutputHelper);
            sc.AddToolsGenerator();

            using (var sp = sc.BuildServiceProvider())
            {
                var workspaceFactory = sp.GetService<ICSharpWorkspaceFactory>();

                var workspace = workspaceFactory.CreateWorkspace();

                var registerEmbeddedResource = false;
                var registerCompile = true;

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

                var options = new GeneratorOptions(useRelaxedJsonEscaping, useMultiLine, registerEmbeddedResource, registerCompile) { NewLineSeparator = "\n" };

                generator.Generate(workspace, locator, snapshotWriter, locator, jsonReader, snapshotWriter, workspace.Files, options);

                var snapshotTest = SnapshotTestBuilder
                    .Create()
                    .WithThisFilePathLocation()
                    .WithTextStrategy()
                    .Build();

                return snapshotTest.CompareSnapshotAsync(snapshotName, snapshotWriter.GetAllGenerated(), forceReplaceSnapshot: false);
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