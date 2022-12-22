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
using SoloX.GeneratorTools.Core.CSharp.Generator.Attributes;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Impl
{
    public class ToolsGenerator : IToolsGenerator
    {
        private readonly IGeneratorLogger<ToolsGenerator> logger;
        private readonly ICSharpWorkspaceFactory workspaceFactory;

        private const string ResourcesFolderName = "JsonResourcesFolder";

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

            var jsonLocator = new RelativeLocator(Path.Combine(projectFolder, ResourcesFolderName), project.RootNameSpace);
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
            var (ns, projectFolder) = ProbRootNamespaceAndProjectFolder(compilation);

            var workspace = this.workspaceFactory.CreateWorkspace(compilation);

            var locator = new RelativeLocator(Path.Combine(projectFolder), ns);
            var fileWriter = new MemoryWriter(".g.cs", (l, s) =>
            {
                var name = l.StartsWith(projectFolder) ? l.Substring(projectFolder.Length) : l;

                name = name.Replace('/', '.').Replace('\\', '.').TrimStart('.');

                context.AddSource(name, s.ReadToEnd());
            });

            var jsonLocator = new RelativeLocator(Path.Combine(projectFolder, ResourcesFolderName), ns);
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

        private static TAttribute FindAttribute<TAttribute>(Type type)
            where TAttribute : Attribute
        {
            var attributes = type.GetCustomAttributes(typeof(TAttribute), false);
            return (TAttribute)attributes.FirstOrDefault();
        }

        internal void Generate(ICSharpWorkspace workspace, ILocator locator, IWriter fileWriter, ILocator jsonLocator, IWriter jsonWriter, IEnumerable<ICSharpFile> files)
        {
            workspace.RegisterFile(GetContentFile("./Patterns/Itf/IMyObjectStringLocalizerPattern.cs"));
            workspace.RegisterFile(GetContentFile("./Patterns/Impl/MyObjectStringLocalizerPattern.cs"));
            workspace.RegisterFile(GetContentFile("./Patterns/Impl/MyObjectStringLocalizerPatternExtensions.cs"));

            var resolver = workspace.DeepLoad();

            /// debug
            var patternAttribute = FindAttribute<PatternAttribute>(typeof(MyObjectStringLocalizerPattern));

            var selector = patternAttribute.Selector;

            var declarations = selector.GetDeclarations(files);

            var repeatAttribute = FindAttribute<RepeatAttribute>(typeof(MyObjectStringLocalizerPattern));

            var pattern = resolver.Find(typeof(MyObjectStringLocalizerPattern).FullName);

            var patternPattern = resolver.Resolve(repeatAttribute.RepeatPattern, pattern.Single());

            var generator1 = new AutomatedGenerator(
                fileWriter,
                locator,
                resolver,
                typeof(MyObjectStringLocalizerPattern),
                this.logger);

            generator1.AddIgnoreUsing("SoloX.BlazorJsonLocalization.Attributes", "SoloX.BlazorJsonLocalization.Tools.Core.Handlers");

            var gen1Items = generator1.Generate(files);

            var generator2 = new AutomatedGenerator(
                fileWriter,
                locator,
                resolver,
                typeof(MyObjectStringLocalizerPatternExtensions),
                this.logger);

            generator2.AddIgnoreUsing("SoloX.BlazorJsonLocalization.Attributes", "SoloX.BlazorJsonLocalization.Tools.Core.Handlers");

            var gen2Items = generator2.Generate(files);

            var jsonGenerator = new JsonFileGenerator(
                jsonWriter,
                jsonLocator,
                resolver,
                new AttributeSelector<LocalizerAttribute>(),
                this.logger,
                ResourcesFolderName);

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

        private (string Namespace, string ProjectFolder) ProbRootNamespaceAndProjectFolder(Compilation compilation)
        {
            var commonFolder = FindCommonRootFolder(compilation.SyntaxTrees.Select(x => x.FilePath));

            var pathWords = commonFolder.TrimEnd('\\', '/').Split('\\', '/');

            var nsList = new HashSet<string>();
            var nsBuilder = new StringBuilder();

            foreach (var item in compilation.Assembly.NamespaceNames.Concat(new string[] { string.Empty }))
            {
                if (string.IsNullOrEmpty(item))
                {
                    if (nsBuilder.Length > 0)
                    {
                        nsList.Add(nsBuilder.ToString());
                        nsBuilder.Clear();
                    }
                }
                else
                {
                    if (nsBuilder.Length > 0)
                    {
                        nsBuilder.Append('.');
                    }
                    nsBuilder.Append(item);
                }
            }

            var firstNs = nsList.First();

            var i = 0;
            var intersect = pathWords.Reverse().First();

            var rootNs = compilation.Assembly.Name;

            var idx = firstNs.IndexOf(intersect);
            if (idx > 0)
            {
                i++;
                rootNs = firstNs.Substring(0, idx - 1);

                foreach (var word in pathWords.Reverse().Skip(1))
                {
                    intersect = word + '.' + intersect;

                    idx = firstNs.IndexOf(intersect);

                    if (idx > 0)
                    {
                        rootNs = firstNs.Substring(0, idx - 1);

                        i++;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            var projectFolder = string.Join("/", pathWords.Reverse().Skip(i).Reverse());

            if (string.IsNullOrEmpty(projectFolder))
            {
                projectFolder = ".";
            }

            return (rootNs, projectFolder);
        }

        private string FindCommonRootFolder(IEnumerable<string> paths)
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
    }
}
