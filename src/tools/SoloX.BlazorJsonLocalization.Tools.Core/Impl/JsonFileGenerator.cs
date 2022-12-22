using Microsoft.CodeAnalysis;
using SoloX.GeneratorTools.Core.CSharp.Generator;
using SoloX.GeneratorTools.Core.CSharp.Generator.Evaluator;
using SoloX.GeneratorTools.Core.CSharp.Generator.Selectors;
using SoloX.GeneratorTools.Core.CSharp.Model;
using SoloX.GeneratorTools.Core.CSharp.Model.Resolver;
using SoloX.GeneratorTools.Core.CSharp.Model.Use;
using SoloX.GeneratorTools.Core.CSharp.Workspace;
using SoloX.GeneratorTools.Core.Generator;
using SoloX.GeneratorTools.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Impl
{
    public class JsonFileGenerator
    {
        private IWriter writer;
        private ILocator locator;
        private IDeclarationResolver resolver;
        private ISelector selector;
        private readonly IGeneratorLogger logger;
        private readonly string resourcesFolderName;

        public JsonFileGenerator(IWriter writer, ILocator locator, IDeclarationResolver resolver, ISelector selector, IGeneratorLogger logger, string resourcesFolderName)
        {
            this.writer = writer;
            this.locator = locator;
            this.resolver = resolver;
            this.selector = selector;
            this.logger = logger;
            this.resourcesFolderName = resourcesFolderName;
        }

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

                var localizerAttribute = genericDeclaration.Attributes.First();

                var attributeSyntax = localizerAttribute.SyntaxNodeProvider.SyntaxNode;

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

            return generatedItems;
        }

        private static IDictionary<string, string> BuildLocalizationMap(IGenericDeclaration<SyntaxNode> genericDeclaration)
        {
            var map = new Dictionary<string, string>();

            foreach (var member in genericDeclaration.Members)
            {
                if (member is IPropertyDeclaration propertyDeclaration)
                {
                    map.Add(propertyDeclaration.Name, propertyDeclaration.Name);
                }
                else if (member is IMethodDeclaration methodDeclaration)
                {
                    map.Add(methodDeclaration.Name, methodDeclaration.Name);
                }
            }

            return map;
        }

        private void WriteLanguageJsonFile(string location, IDictionary<string, string> map, string jsonName)
        {
            this.writer.Generate(location, jsonName, textWriter =>
            {
                textWriter.Write(JsonSerializer.Serialize(map, new JsonSerializerOptions { WriteIndented = true }));
            });
        }
    }
}
