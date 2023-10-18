// ----------------------------------------------------------------------
// <copyright file="JsonFileGenerator.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using SoloX.BlazorJsonLocalization.Attributes;
using SoloX.BlazorJsonLocalization.Tools.Core.Impl.Localization;
using SoloX.GeneratorTools.Core.CSharp.Generator;
using SoloX.GeneratorTools.Core.CSharp.Generator.Selectors;
using SoloX.GeneratorTools.Core.CSharp.Model;
using SoloX.GeneratorTools.Core.CSharp.Model.Resolver;
using SoloX.GeneratorTools.Core.CSharp.Model.Use;
using SoloX.GeneratorTools.Core.CSharp.Workspace;
using SoloX.GeneratorTools.Core.Generator;
using SoloX.GeneratorTools.Core.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Impl
{
    /// <summary>
    /// Json localization resource file generator.
    /// </summary>
    public class JsonFileGenerator
    {
        private readonly IReader reader;
        private readonly IWriter writer;
        private readonly ILocator locator;
        private readonly IDeclarationResolver resolver;
        private readonly ISelector selector;
        private readonly IGeneratorLogger logger;
        private readonly string resourcesFolderName;

        /// <summary>
        /// Setup instance.
        /// </summary>
        /// <param name="reader">Reader to get existing resources.</param>
        /// <param name="writer">Writer to write the generated resources.</param>
        /// <param name="locator">Locator that provide read resources location.</param>
        /// <param name="resolver">Declaration resolver.</param>
        /// <param name="selector">Declaration selector to get the Declarations to generate the resource from.</param>
        /// <param name="logger">Logger to log messages.</param>
        /// <param name="resourcesFolderName">Resource folder name used in the provided locator.</param>
        public JsonFileGenerator(IReader reader, IWriter writer, ILocator locator, IDeclarationResolver resolver, ISelector selector, IGeneratorLogger logger, string resourcesFolderName)
        {
            this.reader = reader;
            this.writer = writer;
            this.locator = locator;
            this.resolver = resolver;
            this.selector = selector;
            this.logger = logger;
            this.resourcesFolderName = resourcesFolderName;
        }

        /// <summary>
        /// Generate the resources from the given files.
        /// </summary>
        /// <param name="files">Files to generate the resource from.</param>
        /// <returns>The generated items.</returns>
        public IEnumerable<IGeneratedItem> Generate(IEnumerable<ICSharpFile> files)
        {
            var generatedItems = new List<IGeneratedItem>();

            var declarations = this.selector.GetDeclarations(files);

            this.logger.LogDebug($"Selected declarations: {string.Join(", ", declarations.Select(d => d.Name))}");

            foreach (var declaration in declarations)
            {
                this.logger.LogDebug($"Processing json for {declaration.Name}");

                var (location, nameSpace) = this.locator.ComputeTargetLocation(declaration.DeclarationNameSpace);

                var genericDeclaration = declaration as IGenericDeclaration<SyntaxNode>;

                var localizerAttribute = genericDeclaration.Attributes[0];

                var folder = localizerAttribute.ConstructorArguments.ElementAtOrDefault(0) as string;

                if (folder != null)
                {
                    if (string.IsNullOrEmpty(folder))
                    {
                        location = location.Replace(this.resourcesFolderName + Path.PathSeparator, string.Empty);
                    }
                    else
                    {
                        location = location.Replace(this.resourcesFolderName, folder);
                    }

                    var languages = localizerAttribute.ConstructorArguments.ElementAtOrDefault(1) as string[];

                    var jsonName = ((IGenericDeclarationUse)genericDeclaration.Extends.First()).GenericParameters.First().Declaration.Name;

                    var map = BuildLocalizationMap(genericDeclaration);

                    WriteLanguageJsonFile(location, map, jsonName);

                    foreach (var language in languages)
                    {
                        var name = $"{jsonName}.{language}";

                        WriteLanguageJsonFile(location, map, name);
                    }
                }
            }

            return generatedItems;
        }

        private static ALocalizationData BuildLocalizationMap(IGenericDeclaration<SyntaxNode> genericDeclaration)
        {
            return BuildLocalizationMap(genericDeclaration, null);
        }

        private static ALocalizationData BuildLocalizationMap(IGenericDeclaration<SyntaxNode> genericDeclaration, Dictionary<string, ALocalizationData> rootMap)
        {
            var map = new Dictionary<string, ALocalizationData>();

            if (rootMap == null)
            {
                rootMap = map;
            }

            foreach (var member in genericDeclaration.Members)
            {
                if (member is IPropertyDeclaration propertyDeclaration)
                {
                    var propertyType = propertyDeclaration.PropertyType.Declaration;
                    if (propertyType is IGenericDeclaration<SyntaxNode> genericSubDeclaration)
                    {
                        if (!rootMap.ContainsKey(propertyDeclaration.PropertyType.Declaration.FullName))
                        {
                            var subMap = BuildLocalizationMap(genericSubDeclaration, rootMap);

                            rootMap.Add(propertyDeclaration.PropertyType.Declaration.FullName, subMap);
                        }

                        var argMap = BuildLocalizationArgumentMap(genericSubDeclaration, propertyDeclaration.Attributes);

                        map.Add(propertyDeclaration.Name, argMap);
                    }
                    else if (propertyDeclaration.Attributes.Any(a => a.Name == nameof(TranslateArgAttribute)))
                    {
                        var attribute = propertyDeclaration.Attributes.First(a => a.Name == nameof(TranslateArgAttribute));
                        var txtValue = (string?)attribute.ConstructorArguments.FirstOrDefault();

                        map.Add(propertyDeclaration.Name, new LocalizationValue(txtValue ?? propertyDeclaration.Name));
                    }
                    else
                    {
                        var attribute = propertyDeclaration.Attributes.FirstOrDefault(a => a.Name == nameof(TranslateAttribute));
                        var txtValue = (string?)attribute?.ConstructorArguments?.FirstOrDefault();

                        map.Add(propertyDeclaration.Name, new LocalizationValue(txtValue ?? propertyDeclaration.Name));
                    }
                }
                else if (member is IMethodDeclaration methodDeclaration)
                {
                    var attribute = methodDeclaration.Attributes.FirstOrDefault(a => a.Name == nameof(TranslateAttribute));
                    var txtValue = (string?)attribute?.ConstructorArguments?.FirstOrDefault();

                    var i = 0;
                    var args = string.Join(" - ", methodDeclaration.Parameters.Select(p => $"{{{i++}}} = {p.Name}"));

                    map.Add(methodDeclaration.Name, new LocalizationValue(txtValue ?? (methodDeclaration.Name + " - " + args)));
                }
            }

            return new LocalizationMap(map);
        }

        private static ALocalizationData BuildLocalizationArgumentMap(IGenericDeclaration<SyntaxNode> genericDeclaration, IEnumerable<IAttributeUse> attributes)
        {
            var argumentMap = attributes.Where(a => a.Name == nameof(TranslateSubAttribute)).ToDictionary(x => (string)x.ConstructorArguments.First(), x => (string)x.ConstructorArguments.Skip(1).First());

            var map = new Dictionary<string, ALocalizationData>();

            foreach (var member in genericDeclaration.Members)
            {
                if (member is IPropertyDeclaration propertyDeclaration)
                {
                    var propertyType = propertyDeclaration.PropertyType.Declaration;
                    if (propertyType is IGenericDeclaration<SyntaxNode> genericSubDeclaration)
                    {
                        var subMap = BuildLocalizationArgumentMap(genericSubDeclaration, propertyDeclaration.Attributes);

                        map.Add(propertyDeclaration.Name, subMap);
                    }
                    else if (propertyDeclaration.Attributes.Any(a => a.Name == nameof(TranslateArgAttribute)))
                    {
                        var txtValue = argumentMap.TryGetValue(propertyDeclaration.Name, out var attValue) ? attValue : propertyDeclaration.Name;

                        map.Add(propertyDeclaration.Name, new LocalizationValue(txtValue));
                    }
                }
            }

            return new LocalizationMap(map);
        }

        private void WriteLanguageJsonFile(string location, ALocalizationData map, string jsonName)
        {
            ALocalizationData? sourceMap = null;
            try
            {
                this.reader.Read(location, jsonName, textReader =>
                {
                    sourceMap = JsonSerializer.Deserialize<ALocalizationData>(textReader);
                });
            }
            catch (JsonException)
            {
                // Nothing to do....
            }

            var dirty = true;

#pragma warning disable CA1508 // Avoid dead conditional code
            var targetMap = sourceMap == null
                ? map
                : map.Merge(sourceMap, string.Empty, out dirty);
#pragma warning restore CA1508 // Avoid dead conditional code

            if (dirty)
            {
                this.writer.Generate(location, jsonName, textWriter =>
                {
                    textWriter.Write(JsonSerializer.Serialize(targetMap, new JsonSerializerOptions { WriteIndented = true }));
                });
            }
            else
            {
                this.writer.Generate(location, jsonName, textWriter =>
                {
                    textWriter.Write(JsonSerializer.Serialize(sourceMap, new JsonSerializerOptions { WriteIndented = true }));
                });
            }
        }
    }
}
