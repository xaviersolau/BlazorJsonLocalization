// ----------------------------------------------------------------------
// <copyright file="LocalizationGenerator.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

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
using SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Impl;
using SoloX.BlazorJsonLocalization.Attributes;
using SoloX.BlazorJsonLocalization.Tools.Core.Selectors;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Impl
{
    /// <summary>
    /// Json localizer generator that will generate the String localizer classes and the Json resources.
    /// </summary>
    public class LocalizationGenerator : ILocalizationGenerator
    {
        private readonly IGeneratorLogger<LocalizationGenerator> logger;
        private readonly ICSharpWorkspaceFactory workspaceFactory;

        private const string ResourcesFolderName = "JsonResourcesFolder";

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizationGenerator"/> class.
        /// </summary>
        /// <param name="logger">Logger that will be used for logs.</param>
        /// <param name="workspaceFactory">The workspace factory to use.</param>
        public LocalizationGenerator(IGeneratorLogger<LocalizationGenerator> logger, ICSharpWorkspaceFactory workspaceFactory)
        {
            this.logger = logger;
            this.workspaceFactory = workspaceFactory;
        }

        /// <summary>
        /// Process generation for the given project file. It can be used from a CLI tool.
        /// </summary>
        /// <param name="projectFile">The project file to process.</param>
        public ILocalizationGeneratorResults Generate(string projectFile)
        {
            var inputFiles = new List<string>();
            var generatedCodeFiles = new List<string>();
            var generatedResourceFiles = new List<string>();

            var projectFolder = Path.GetDirectoryName(projectFile);

            this.logger.LogInformation($"Loading {Path.GetFileName(projectFile)}...");

            var workspace = this.workspaceFactory.CreateWorkspace();

            var project = workspace.RegisterProject(projectFile);

            var locator = new RelativeLocator(Path.Combine(projectFolder), project.RootNameSpace);
            var fileWriter = new FileWriter(".g.cs", generatedCodeFiles.Add);

            var jsonLocator = new RelativeLocator(Path.Combine(projectFolder, ResourcesFolderName), project.RootNameSpace);
            var jsonWriter = new FileWriter(".json", generatedResourceFiles.Add);
            var jsonReader = new FileReader(".json");

            // Generate with a filter on current project interface declarations.
            this.Generate(
                workspace,
                locator,
                fileWriter,
                jsonLocator,
                jsonReader,
                jsonWriter,
                project.Files);

            return new LocalizationGeneratorResults(
                inputFiles,
                generatedCodeFiles,
                generatedResourceFiles);
        }

        /// <summary>
        /// Process generation for the given compilation unit. It can be used by a code analyzer tool.
        /// </summary>
        /// <param name="compilation">Compilation unit to process.</param>
        /// <param name="classes">Targeted interfaces.</param>
        /// <param name="context">Source production context to generate the C# code.</param>
        public ILocalizationGeneratorResults Generate(Compilation compilation, ImmutableArray<InterfaceDeclarationSyntax> classes, SourceProductionContext context)
        {
            if (compilation == null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            var inputFiles = new List<string>();
            var generatedCodeFiles = new List<string>();
            var generatedResourceFiles = new List<string>();

            var (ns, projectFolder) = ProbRootNamespaceAndProjectFolder(compilation);

            var workspace = this.workspaceFactory.CreateWorkspace(compilation);

            var locator = new RelativeLocator(Path.Combine(projectFolder), ns);
            var fileWriter = new MemoryWriter(".g.cs", (l, s) =>
            {
                generatedCodeFiles.Add(l);

                var name = l.StartsWith(projectFolder, StringComparison.Ordinal) ? l.Substring(projectFolder.Length) : l;

                name = name.Replace('/', '.').Replace('\\', '.').TrimStart('.');

                context.AddSource(name, s.ReadToEnd());
            });

            var jsonLocator = new RelativeLocator(Path.Combine(projectFolder, ResourcesFolderName), ns);
            var jsonWriter = new FileWriter(".json", generatedResourceFiles.Add);
            var jsonReader = new FileReader(".json");

            // Generate with a filter on current project interface declarations.
            this.Generate(
                workspace,
                locator,
                fileWriter,
                jsonLocator,
                jsonReader,
                jsonWriter,
                workspace.SyntaxTrees.Where(s => compilation.ContainsSyntaxTree(s.SyntaxTree)));

            return new LocalizationGeneratorResults(
                inputFiles,
                generatedCodeFiles,
                generatedResourceFiles);
        }

        internal void Generate(ICSharpWorkspace workspace, ILocator locator, IWriter fileWriter, ILocator jsonLocator, IReader jsonReader, IWriter jsonWriter, IEnumerable<ICSharpFile> files)
        {
            workspace.RegisterFile(GetContentFile("./Patterns/Itf/IMyObjectStringLocalizerPattern.cs"));
            workspace.RegisterFile(GetContentFile("./Patterns/Impl/MyObjectStringLocalizerPattern.cs"));
            workspace.RegisterFile(GetContentFile("./Patterns/Impl/MyObjectStringLocalizerPatternExtensions.cs"));

            workspace.RegisterFile(GetContentFile("./Patterns/Itf/IMyObjectSubStringLocalizerPattern.cs"));
            workspace.RegisterFile(GetContentFile("./Patterns/Impl/MyObjectSubStringLocalizerPattern.cs"));

            workspace.RegisterAssemblyTypes(
                typeof(SubLocalizerPropertySelector).Assembly,
                new[] { typeof(SubLocalizerPropertySelector), typeof(LocalizerPropertySelector), typeof(LocalizerArgumentSelector), typeof(LocalizerAttribute), typeof(SubLocalizerAttribute) });

            var resolver = workspace.DeepLoad();

            var generator1 = new AutomatedGenerator(
                fileWriter,
                locator,
                resolver,
                typeof(MyObjectStringLocalizerPattern),
                new SelectorResolver(),
                this.logger);

            generator1.AddIgnoreUsing("SoloX.BlazorJsonLocalization.Attributes", "SoloX.BlazorJsonLocalization.Tools.Core.Handlers", "SoloX.BlazorJsonLocalization.Tools.Core.Selectors");

            var gen1Items = generator1.Generate(files);

            var generator2 = new AutomatedGenerator(
                fileWriter,
                locator,
                resolver,
                typeof(MyObjectStringLocalizerPatternExtensions),
                new SelectorResolver(),
                this.logger);

            generator2.AddIgnoreUsing("SoloX.BlazorJsonLocalization.Attributes", "SoloX.BlazorJsonLocalization.Tools.Core.Handlers", "SoloX.BlazorJsonLocalization.Tools.Core.Selectors");

            var gen2Items = generator2.Generate(files);

            var generator3 = new AutomatedGenerator(
                fileWriter,
                locator,
                resolver,
                typeof(MyObjectSubStringLocalizerPattern),
                new SelectorResolver(),
                this.logger);

            generator3.AddIgnoreUsing("SoloX.BlazorJsonLocalization.Attributes", "SoloX.BlazorJsonLocalization.Tools.Core.Handlers", "SoloX.BlazorJsonLocalization.Tools.Core.Selectors");

            var gen3Items = generator3.Generate(files);

            var jsonGenerator = new JsonFileGenerator(
                jsonReader,
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
            var assembly = typeof(LocalizationGenerator).Assembly;

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

        private static (string Namespace, string ProjectFolder) ProbRootNamespaceAndProjectFolder(Compilation compilation)
        {
            var commonFolder = FindCommonRootFolder(compilation.SyntaxTrees.Select(x => x.FilePath).ToArray());

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

            var idx = firstNs.IndexOf(intersect, StringComparison.Ordinal);
            if (idx > 0)
            {
                i++;
                rootNs = firstNs.Substring(0, idx - 1);

                foreach (var word in pathWords.Reverse().Skip(1))
                {
                    intersect = word + '.' + intersect;

                    idx = firstNs.IndexOf(intersect, StringComparison.Ordinal);

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

        private static string FindCommonRootFolder(IReadOnlyCollection<string> paths)
        {
            if (paths == null)
            {
                throw new ArgumentNullException(nameof(paths));
            }

            var root = paths.First();

            var len = root.Length;

            foreach (var path in paths.Skip(1))
            {
                len = Math.Min(len, path.Length);
                while (!root.Substring(0, len).Equals(path.Substring(0, len), StringComparison.Ordinal))
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
