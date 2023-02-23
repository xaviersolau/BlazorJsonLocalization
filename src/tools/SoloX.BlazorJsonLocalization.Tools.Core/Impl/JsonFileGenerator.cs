﻿// ----------------------------------------------------------------------
// <copyright file="JsonFileGenerator.cs" company="Xavier Solau">
// Copyright © 2021 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using SoloX.BlazorJsonLocalization.Tools.Core.Impl.Localization;
using SoloX.GeneratorTools.Core.CSharp.Generator;
using SoloX.GeneratorTools.Core.CSharp.Generator.Evaluator;
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

                var attributeSyntax = localizerAttribute.SyntaxNodeProvider.SyntaxNode;

                if (attributeSyntax.ArgumentList != null)
                {
                    var constEvaluator = new ConstantExpressionSyntaxEvaluator<string>();
                    var folder = constEvaluator.Visit(attributeSyntax.ArgumentList.Arguments.First().Expression);

                    if (string.IsNullOrEmpty(folder))
                    {
                        location = location.Replace(this.resourcesFolderName + Path.PathSeparator, string.Empty);
                    }
                    else
                    {
                        location = location.Replace(this.resourcesFolderName, folder);
                    }

                    var constArrayEvaluator = new ConstantExpressionSyntaxEvaluator<string[]>();
                    var languages = constArrayEvaluator.Visit(attributeSyntax.ArgumentList.Arguments.Skip(1).First().Expression);

                    var jsonName = ((IGenericDeclarationUse)genericDeclaration.Extends.First()).GenericParameters.First().Declaration.Name;

                    var map = BuildLocalizationMap(genericDeclaration);

                    WriteLanguageJsonFile(location, map, jsonName);

                    foreach (var language in languages)
                    {
                        var name = $"{jsonName}-{language}";

                        WriteLanguageJsonFile(location, map, name);
                    }
                }
            }

            return generatedItems;
        }

        private static ALocalizationData BuildLocalizationMap(IGenericDeclaration<SyntaxNode> genericDeclaration)
        {
            var map = new Dictionary<string, ALocalizationData>();

            foreach (var member in genericDeclaration.Members)
            {
                if (member is IPropertyDeclaration propertyDeclaration)
                {
                    var propertyType = propertyDeclaration.PropertyType.Declaration;
                    if (propertyType is IGenericDeclaration<SyntaxNode> genericSubDeclaration)
                    {
                        var subMap = BuildLocalizationMap(genericSubDeclaration);

                        map.Add(propertyDeclaration.Name, subMap);
                    }
                    else
                    {
                        map.Add(propertyDeclaration.Name, new LocalizationValue(propertyDeclaration.Name));
                    }
                }
                else if (member is IMethodDeclaration methodDeclaration)
                {
                    map.Add(methodDeclaration.Name, new LocalizationValue(methodDeclaration.Name));
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
        }
    }
}
