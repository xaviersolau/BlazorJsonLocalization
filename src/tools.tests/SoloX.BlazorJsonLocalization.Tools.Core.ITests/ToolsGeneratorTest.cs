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
        [InlineData(@"Resources/Sample1/ISimpleLocalizer.cs", @"Resources/Sample1/Sample.cs")]
        public void GenerateBasicLocalizer(string interfaceFile, string componentFile)
        {
            var snapshotName = nameof(this.GenerateBasicLocalizer)
                + Path.GetFileNameWithoutExtension(interfaceFile);

            this.GenerateSnapshot(snapshotName, interfaceFile, componentFile);
        }

        private void GenerateSnapshot(string snapshotName, params string[] files)
        {
            //SnapshotHelper.IsOverwriteEnable = true;

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

                var generator = new ToolsGenerator(
                    sp.GetService<IGeneratorLogger<ToolsGenerator>>(),
                    workspaceFactory);

                var inputs = new HashSet<string>();
                var locator = new RelativeLocator(string.Empty, "target.name.space");

                var snapshotGenerator = new SnapshotWriter();

                generator.Generate(workspace, locator, snapshotGenerator, locator, snapshotGenerator, workspace.Files);

                var location = SnapshotHelper.GetLocationFromCallingCodeProjectRoot(null);
                SnapshotHelper.AssertSnapshot(snapshotGenerator.GetAllGenerated(), snapshotName, location);
            }
        }
    }
}