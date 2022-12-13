using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SoloX.GeneratorTools.Core.CSharp.Generator.Impl;
using SoloX.GeneratorTools.Core.CSharp.Generator.Selectors;
using SoloX.GeneratorTools.Core.Utils;
using SoloX.GeneratorTools.Core.CSharp.Workspace;
using SoloX.GeneratorTools.Core.Generator;
using SoloX.GeneratorTools.Core.Generator.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Impl;
using SoloX.BlazorJsonLocalization.Attributes;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Impl
{
    public class ToolsGenerator : IToolsGenerator
    {
        private readonly IGeneratorLogger<ToolsGenerator> logger;
        private readonly ICSharpWorkspaceFactory workspaceFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolsGenerator"/> class.
        /// </summary>
        /// <param name="logger">Logger that will be used for logs.</param>
        /// <param name="workspaceFactory">The workspace factory to use.</param>
        public ToolsGenerator(IGeneratorLogger<ToolsGenerator> logger, ICSharpWorkspaceFactory workspaceFactory)
        {
            this.logger = logger;
            this.workspaceFactory = workspaceFactory;
        }

        public void Generate(string projectFile)
        {
            var projectFolder = Path.GetDirectoryName(projectFile);

            this.logger.LogInformation($"Loading {Path.GetFileName(projectFile)}...");

            var workspace = this.workspaceFactory.CreateWorkspace();

            var project = workspace.RegisterProject(projectFile);

            var locator = new RelativeLocator(Path.Combine(projectFolder), project.RootNameSpace);
            var fileWriter = new FileWriter(".g.cs");

            var jsonLocator = new RelativeLocator(Path.Combine(projectFolder, "Resources"), project.RootNameSpace);
            var jsonWriter = new FileWriter(".json");

            // Generate with a filter on current project interface declarations.
            this.Generate(
                workspace,
                locator,
                fileWriter,
                jsonLocator,
                jsonWriter,
                project.Files);
        }

        public void Generate(Compilation compilation, ImmutableArray<InterfaceDeclarationSyntax> classes, SourceProductionContext context)
        {
            var projectFolder = FindProjectFolder(compilation.SyntaxTrees.Select(x => x.FilePath));

            var workspace = this.workspaceFactory.CreateWorkspace(compilation);

            // var ns = compilation.Assembly.NamespaceNames.First(); TODO
            var ns = "SoloX.BlazorJsonLocalization.Generator.Sample";

            var locator = new RelativeLocator(Path.Combine(projectFolder), ns);
            var fileWriter = new MemoryWriter(".g.cs", (l, s) =>
            {
                var name = l.StartsWith(projectFolder) ? l.Substring(projectFolder.Length) : l;

                name = name.Replace('/', '.').Replace('\\', '.');

                context.AddSource(name, s.ReadToEnd());
            });

            var jsonLocator = new RelativeLocator(Path.Combine(projectFolder, "Resources"), ns);
            var jsonWriter = new FileWriter(".json");

            // Generate with a filter on current project interface declarations.
            this.Generate(
                workspace,
                locator,
                fileWriter,
                jsonLocator,
                jsonWriter,
                workspace.SyntaxTrees.Where(s => compilation.ContainsSyntaxTree(s.SyntaxTree)));
        }

        private string FindProjectFolder(IEnumerable<string> paths)
        {
            var root = paths.First();

            var len = root.Length;

            foreach (var path in paths.Skip(1))
            {
                len = Math.Min(len, path.Length);
                while (!root.Substring(0, len).Equals(path.Substring(0, len)))
                {
                    len--;

                    if (len == 0)
                    {
                        throw new InvalidOperationException("Unable to find common part");
                    }
                }

                root = root.Substring(0, len);
            }

            return root;
        }

        internal void Generate(ICSharpWorkspace workspace, ILocator locator, IWriter fileWriter, ILocator jsonLocator, IWriter jsonWriter, IEnumerable<ICSharpFile> files)
        {
            workspace.RegisterFile(GetContentFile("./Patterns/Itf/IMyObjectStringLocalizerPattern.cs"));
            workspace.RegisterFile(GetContentFile("./Patterns/Impl/MyObjectStringLocalizerPattern.cs"));
            workspace.RegisterFile(GetContentFile("./Patterns/Impl/MyObjectStringLocalizerPatternExtensions.cs"));

            var resolver = workspace.DeepLoad();

            var generator1 = new AutomatedGenerator(
                fileWriter,
                locator,
                resolver,
                typeof(MyObjectStringLocalizerPattern));

            generator1.AddIgnoreUsing("SoloX.BlazorJsonLocalization.Attributes", "SoloX.BlazorJsonLocalization.Tools.Core.Handlers");

            var gen1Items = generator1.Generate(files);

            var generator2 = new AutomatedGenerator(
                fileWriter,
                locator,
                resolver,
                typeof(MyObjectStringLocalizerPatternExtensions));

            generator2.AddIgnoreUsing("SoloX.BlazorJsonLocalization.Attributes", "SoloX.BlazorJsonLocalization.Tools.Core.Handlers");

            var gen2Items = generator2.Generate(files);

            var jsonGenerator = new JsonFileGenerator(
                jsonWriter,
                jsonLocator,
                resolver,
                new AttributeSelector<LocalizerAttribute>());

            var jsonItems = jsonGenerator.Generate(files);
        }

        private static string GetContentFile(string contentFile)
        {
            var assembly = typeof(ToolsGenerator).Assembly;

            var file = Path.Combine(Path.GetDirectoryName(assembly.Location), contentFile);

            if (File.Exists(file))
            {
                return file;
            }

            var res = assembly.GetManifestResourceNames();

            var resName = assembly.GetName().Name + contentFile.Substring(1).Replace('/', '.');

            using var stream = assembly.GetManifestResourceStream(resName);

            var tempFile = Path.GetTempFileName();

            using var tempStream = File.OpenWrite(tempFile);

            stream.CopyTo(tempStream);

            return tempFile;
        }
    }
}
