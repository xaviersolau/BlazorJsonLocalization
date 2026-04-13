// ----------------------------------------------------------------------
// <copyright file="LocalizationGenerator.cs" company="Xavier Solau">
// Copyright © 2021-2026 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SoloX.BlazorJsonLocalization.Attributes;
using SoloX.BlazorJsonLocalization.Tools.Core.Patterns.Impl;
using SoloX.BlazorJsonLocalization.Tools.Core.Selectors;
using SoloX.GeneratorTools.Core.CSharp.Generator.Impl;
using SoloX.GeneratorTools.Core.CSharp.Generator.Selectors;
using SoloX.GeneratorTools.Core.CSharp.Workspace;
using SoloX.GeneratorTools.Core.Generator;
using SoloX.GeneratorTools.Core.Generator.Impl;
using SoloX.GeneratorTools.Core.Utils;

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

        private static readonly GeneratorOptions DefaultOptions = new GeneratorOptions(false, false, false, true);

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
        /// <param name="generatorOptions">Generator options.</param>
        public ILocalizationGeneratorResults Generate(string projectFile, GeneratorOptions? generatorOptions = null)
        {
            if (generatorOptions == null)
            {
                generatorOptions = DefaultOptions;
            }

            var inputFiles = new List<string>();
            var generatedCodeFiles = new List<string>();
            var generatedResourceFiles = new List<string>();

            this.logger.LogInformation($"Loading {Path.GetFileName(projectFile)}.");

            var workspace = this.workspaceFactory.CreateWorkspace();

            var project = workspace.RegisterProject(projectFile);

            var projectParameters = new ProjectParameters(project.ProjectPath, project.RootNameSpace);

            var locator = new RelativeLocator(
                Path.Combine(projectParameters.ProjectPath, "obj"),
                "none",
                suffix: "Impl");

            var fileWriter = new MemoryWriter(".g.cs", (fileName, textCode) =>
            {
                generatedCodeFiles.Add(fileName);

                var generatedCode = textCode.ReadToEnd();

                var previousCode = File.Exists(fileName) ? File.ReadAllText(fileName) : null;

                if (previousCode == null || previousCode != generatedCode)
                {
                    var location = Path.GetDirectoryName(fileName);
                    if (!Directory.Exists(location))
                    {
                        Directory.CreateDirectory(location);
                    }

                    this.logger.LogInformation($"Writing CS file: {fileName}.");
                    File.WriteAllText(fileName, generatedCode);
                }
                else
                {
                    this.logger.LogInformation($"No change in CS file: {fileName}.");
                }
            });

            var jsonLocator = new RelativeLocator(
                Path.Combine(project.ProjectPath, ResourcesFolderName),
                project.RootNameSpace);

            var jsonWriter = new MemoryWriter(".json", (l, s) =>
            {
                generatedResourceFiles.Add(l);

                var generatedJson = s.ReadToEnd();

                var previousJson = File.Exists(l) ? File.ReadAllText(l) : null;

                if (previousJson == null || previousJson != generatedJson)
                {
                    var location = Path.GetDirectoryName(l);
                    if (!Directory.Exists(location))
                    {
                        Directory.CreateDirectory(location);
                    }

                    this.logger.LogInformation($"Writing JSON file: {l}.");
                    File.WriteAllText(l, generatedJson);
                }
                else
                {
                    this.logger.LogInformation($"No change in JSON file: {l}.");
                }
            });

            var jsonReader = new FileReader(".json");

            // Generate with a filter on current project interface declarations.
            this.Generate(
                workspace,
                locator,
                fileWriter,
                jsonLocator,
                jsonReader,
                jsonWriter,
                project.Files,
                generatorOptions);

            if (generatorOptions.RegisterCompile)
            {
                GeneratePropsCodeFile(
                    projectParameters.ProjectPath,
                    project.ProjectFile,
                    Path.Combine(projectParameters.ProjectPath, "obj"),
                    generatedCodeFiles);
            }

            if (generatorOptions.RegisterEmbeddedResource)
            {
                GeneratePropsResourceFile(
                    projectParameters.ProjectPath,
                    project.ProjectFile,
                    Path.Combine(projectParameters.ProjectPath, "obj"),
                    generatedResourceFiles);
            }

            // Clean files
            RemovePreviousFilesIfAny(Path.Combine(projectParameters.ProjectPath, "obj", "LocalizationGenerator.Code.Output"), generatedCodeFiles);

            return new LocalizationGeneratorResults(
                inputFiles,
                generatedCodeFiles,
                generatedResourceFiles);
        }

        private static void RemovePreviousFilesIfAny(string fileListOutput, List<string> generatedFiles)
        {
            if (File.Exists(fileListOutput))
            {
                var fileSet = new HashSet<string>(generatedFiles);

                var oldList = File.ReadAllLines(fileListOutput);

                foreach (var oldFile in oldList)
                {
                    if (!fileSet.Contains(oldFile) && File.Exists(oldFile))
                    {
                        File.Delete(oldFile);
                    }
                }
            }

            File.WriteAllLines(fileListOutput, generatedFiles);
        }

        /// <summary>
        /// Process generation for the given compilation unit. It can be used by a code analyzer tool.
        /// </summary>
        /// <param name="compilation">Compilation unit to process.</param>
        /// <param name="projectParameters">Project parameters.</param>
        /// <param name="classes">Targeted interfaces.</param>
        /// <param name="context">Source production context to generate the C# code.</param>
        /// <param name="generatorOptions">Generator options.</param>
        public ILocalizationGeneratorResults Generate(
            ProjectParameters projectParameters,
            Compilation compilation,
            ImmutableArray<InterfaceDeclarationSyntax> classes,
            SourceProductionContext context,
            GeneratorOptions? generatorOptions = null)
        {
            if (projectParameters == null)
            {
                throw new ArgumentNullException(nameof(projectParameters));
            }

            if (compilation == null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            if (generatorOptions == null)
            {
                generatorOptions = DefaultOptions;
            }

            var inputFiles = new List<string>();
            var generatedCodeFiles = new List<string>();
            var generatedResourceFiles = new List<string>();

            var ns = projectParameters.RootNamespace;
            var projectFolder = projectParameters.ProjectPath;

            var workspace = this.workspaceFactory.CreateWorkspace(compilation);

            var locator = new RelativeLocator(Path.Combine(projectFolder), ns);
            var fileWriter = new MemoryWriter(".g.cs", (l, s) =>
            {
                generatedCodeFiles.Add(l);

                var name = l.StartsWith(projectFolder, StringComparison.Ordinal) ? l.Substring(projectFolder.Length) : l;

                name = name.Replace('/', '.').Replace('\\', '.').TrimStart('.');

                this.logger.LogInformation($"Adding source file: {name}.");

                context.AddSource(name, s.ReadToEnd());
            });

            var jsonLocator = new RelativeLocator(Path.Combine(projectFolder, ResourcesFolderName), ns);
            var jsonWriter = new FileWriter(".json", f =>
            {
                this.logger.LogInformation($"Writing JSON file: {f}.");

                generatedResourceFiles.Add(f);
            });
            var jsonReader = new FileReader(".json");

            // Generate with a filter on current project interface declarations.
            this.Generate(
                workspace,
                locator,
                fileWriter,
                jsonLocator,
                jsonReader,
                jsonWriter,
                workspace.SyntaxTrees.Where(s => compilation.ContainsSyntaxTree(s.SyntaxTree)),
                generatorOptions);

            if (generatorOptions.RegisterEmbeddedResource)
            {
                // For now this is not properly working since it is too late for MSBuild props. We need
                // to rebuild the project to take into account the embedded resources.
                GeneratePropsResourceFile(
                    projectParameters.ProjectPath,
                    $"{compilation.AssemblyName}.csproj",
                    Path.Combine(projectParameters.ProjectPath, "obj"),
                    generatedResourceFiles);
            }

            return new LocalizationGeneratorResults(
                inputFiles,
                generatedCodeFiles,
                generatedResourceFiles);
        }

        internal void Generate(
            ICSharpWorkspace workspace,
            ILocator locator,
            IWriter fileWriter,
            ILocator jsonLocator,
            IReader jsonReader,
            IWriter jsonWriter,
            IEnumerable<ICSharpFile> files,
            GeneratorOptions generatorOptions)
        {
            this.logger.LogDebug($"Register pattern files.");

            workspace.RegisterFile(GetContentFile("./Patterns/Itf/IMyObjectStringLocalizerPattern.cs"));
            workspace.RegisterFile(GetContentFile("./Patterns/Impl/MyObjectStringLocalizerPattern.cs"));
            workspace.RegisterFile(GetContentFile("./Patterns/Impl/MyObjectStringLocalizerPatternExtensions.cs"));

            workspace.RegisterFile(GetContentFile("./Patterns/Itf/IMyObjectSubStringLocalizerPattern.cs"));
            workspace.RegisterFile(GetContentFile("./Patterns/Impl/MyObjectSubStringLocalizerPattern.cs"));

            workspace.RegisterAssemblyTypes(
                typeof(SubLocalizerPropertySelector).Assembly,
                new[] { typeof(SubLocalizerPropertySelector), typeof(LocalizerPropertySelector), typeof(LocalizerArgumentSelector), typeof(LocalizerAttribute), typeof(SubLocalizerAttribute) });

            this.logger.LogInformation($"Processing data loading.");

            var resolver = workspace.DeepLoad();

            this.logger.LogInformation($"Generating localization source files.");

            this.logger.LogDebug($"Generating from pattern MyObjectStringLocalizerPattern.");

            var generator1 = new AutomatedGenerator(
                fileWriter,
                locator,
                resolver,
                typeof(MyObjectStringLocalizerPattern),
                this.logger,
                new SelectorResolver());

            generator1.AddIgnoreUsing("SoloX.BlazorJsonLocalization.Attributes", "SoloX.BlazorJsonLocalization.Tools.Core.Handlers", "SoloX.BlazorJsonLocalization.Tools.Core.Selectors");

            var gen1Items = generator1.Generate(files);

            this.logger.LogDebug($"Generating from pattern MyObjectStringLocalizerPatternExtensions.");

            var generator2 = new AutomatedGenerator(
                fileWriter,
                locator,
                resolver,
                typeof(MyObjectStringLocalizerPatternExtensions),
                this.logger,
                new SelectorResolver());

            generator2.AddIgnoreUsing("SoloX.BlazorJsonLocalization.Attributes", "SoloX.BlazorJsonLocalization.Tools.Core.Handlers", "SoloX.BlazorJsonLocalization.Tools.Core.Selectors");

            var gen2Items = generator2.Generate(files);

            this.logger.LogDebug($"Generating from pattern MyObjectSubStringLocalizerPattern.");

            var generator3 = new AutomatedGenerator(
                fileWriter,
                locator,
                resolver,
                typeof(MyObjectSubStringLocalizerPattern),
                this.logger,
                new SelectorResolver());

            generator3.AddIgnoreUsing("SoloX.BlazorJsonLocalization.Attributes", "SoloX.BlazorJsonLocalization.Tools.Core.Handlers", "SoloX.BlazorJsonLocalization.Tools.Core.Selectors");

            var gen3Items = generator3.Generate(files);

            this.logger.LogInformation($"Generating localization resource files.");

            var jsonGenerator = new JsonFileGenerator(
                jsonReader,
                jsonWriter,
                jsonLocator,
                resolver,
                new AttributeSelector<LocalizerAttribute>(),
                this.logger,
                ResourcesFolderName,
                generatorOptions);

            var jsonItems = jsonGenerator.Generate(files);
        }

        private static void GeneratePropsResourceFile(
            string projectPath,
            string projectFile,
            string objFile,
            List<string> generatedResourceFiles)
        {
            var absProjectPath = Path.GetFullPath(projectPath);

            if (!absProjectPath.EndsWith(Path.PathSeparator.ToString(), StringComparison.Ordinal))
            {
                absProjectPath = absProjectPath + Path.DirectorySeparatorChar;
            }

            var resourceFiles = generatedResourceFiles.Select(f => Path.Combine(".", f.Replace(absProjectPath, string.Empty)));

            var propsFile = Path.Combine(objFile, $"{projectFile}.LocalizationGenerator.Resource.g.props");
            var list = new List<string>();

            list.Add("<Project>");

            list.Add("  <ItemGroup>");

            foreach (var rfItem in resourceFiles!)
            {
                list.Add($"    <None Remove=\"{rfItem}\" />");
            }

            list.Add("  </ItemGroup>");
            list.Add("  <ItemGroup>");

            foreach (var rfItem in resourceFiles!)
            {
                list.Add($"    <EmbeddedResource Remove=\"{rfItem}\" />");
            }

            list.Add("  </ItemGroup>");
            list.Add("  <ItemGroup>");

            foreach (var rfItem in resourceFiles!)
            {
                list.Add($"    <EmbeddedResource Include=\"{rfItem}\" />");
            }

            list.Add("  </ItemGroup>");

            list.Add("</Project>");

            File.WriteAllLines(propsFile, list);
        }

        private static void GeneratePropsCodeFile(
            string projectPath,
            string projectFile,
            string objPath,
            List<string> generatedCodeFiles)
        {
            var absObjPath = Path.GetFullPath(objPath);

            if (!absObjPath.EndsWith(Path.PathSeparator.ToString(), StringComparison.Ordinal))
            {
                absObjPath = absObjPath + Path.DirectorySeparatorChar;
            }

            var codeFiles = generatedCodeFiles.Select(f => Path.Combine("$(MSBuildThisFileDirectory)", f.Replace(absObjPath, string.Empty)));

            var propsFile = Path.Combine(objPath, $"{projectFile}.LocalizationGenerator.Code.g.props");
            var list = new List<string>();

            list.Add("<Project>");

            list.Add("  <ItemGroup>");

            foreach (var rfItem in codeFiles!)
            {
                list.Add($"    <None Remove=\"{rfItem}\" />");
            }

            list.Add("  </ItemGroup>");

            list.Add("  <ItemGroup>");

            foreach (var rfItem in codeFiles!)
            {
                list.Add($"    <Compile Include=\"{rfItem}\" Link=\"{Path.GetFileName(rfItem)}\" Visible=\"false\" />");
            }

            list.Add("  </ItemGroup>");

            list.Add("</Project>");

            File.WriteAllLines(propsFile, list);
        }

        private static string GetContentFile(string contentFile)
        {
            var assembly = typeof(LocalizationGenerator).Assembly;

            if (!string.IsNullOrEmpty(assembly.Location))
            {
                var file = Path.Combine(Path.GetDirectoryName(assembly.Location), contentFile);

                if (File.Exists(file))
                {
                    return file;
                }
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
