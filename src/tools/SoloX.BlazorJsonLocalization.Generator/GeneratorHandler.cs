
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
//using SoloX.GeneratorTools.Core.CSharp.Workspace.Impl;
//using SoloX.GeneratorTools.Core.CSharp.Workspace.Impl;
//using SoloX.GeneratorTools.Core.Generator;
//using SoloX.GeneratorTools.Core.Utils;
using System;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using SoloX.GeneratorTools.Core.CSharp.Workspace.Impl;
//using SoloX.GeneratorTools.Test.Impl;
//using Microsoft.Extensions.DependencyInjection;
//using SoloX.BlazorJsonLocalization.Generator;
//using SoloX.GeneratorTools.Core.CSharp.Model.Resolver;
//using SoloX.GeneratorTools.Core.CSharp.Workspace;
//using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
//using System.Runtime.Loader;
//using System.Diagnostics;
using System.Linq;
using SoloX.BlazorJsonLocalization.Tools.Core.Impl;
using SoloX.GeneratorTools.Core.Utils;
using SoloX.GeneratorTools.Core.CSharp.Workspace.Impl;

namespace SoloX.GeneratorTools.Test
{
    [Generator(LanguageNames.CSharp)]
    public class GeneratorHandler : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            IncrementalValuesProvider<InterfaceDeclarationSyntax> classDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(static (s, _) => IsSyntaxTargetForGeneration(s), static (ctx, _) => GetSemanticTargetForGeneration(ctx))
                .Where(static m => m is not null);

            //IncrementalValueProvider<(((AdditionalText, MetadataReference), Compilation), ImmutableArray<InterfaceDeclarationSyntax>)> compilationAndClasses =
            //    context.AdditionalTextsProvider.Combine(context.MetadataReferencesProvider).Combine(context.CompilationProvider).Combine(classDeclarations.Collect());

            //IncrementalValueProvider<((MetadataReference, Compilation), ImmutableArray<InterfaceDeclarationSyntax>)> compilationAndClasses =
            //    context.MetadataReferencesProvider.Combine(context.CompilationProvider).Combine(classDeclarations.Collect());

            IncrementalValueProvider<(Compilation, ImmutableArray<InterfaceDeclarationSyntax>)> compilationAndClasses =
                context.CompilationProvider.Combine(classDeclarations.Collect());

            //context.AdditionalTextsProvider.


            //var compilationAndClasses =
            //    context.AdditionalTextsProvider.Combine(context.MetadataReferencesProvider).Combine(context.CompilationProvider).Combine(classDeclarations.Collect());


            //if (!Debugger.IsAttached)
            //{
            //    Debugger.Launch();
            //}
            //else
            //{
            //    Debugger.Break();
            //}

            context.RegisterSourceOutput(compilationAndClasses, static (spc, source) => Execute(source.Item1, source.Item2, spc));
        }

        public static void Execute(Compilation compilation, ImmutableArray<InterfaceDeclarationSyntax> classes, SourceProductionContext context)
        {
            //if (!Debugger.IsAttached)
            //{
            //    Debugger.Launch();
            //}
            //else
            //{
            //    //Debugger.Break();
            //}

            //var loadCtx = new PrivateAssemblyLoadContext();

            var v = Environment.Version;

            var path = typeof(GeneratorHandler).Assembly.Location;


            var generatorType = typeof(ToolsGenerator);

            var logFactory = new LogFactory();

            var csharpWorkspaceFactory = new CSharpWorkspaceFactory(logFactory);

            var generator = new ToolsGenerator(logFactory.CreateLogger<ToolsGenerator>(), csharpWorkspaceFactory);

            generator.Generate(compilation, classes, context);

            //var memWriter = new MemoryWriter("test", (s, r) => { });


            //var generator = new TestGenerator(new CSharpWorkspaceFactory(null));



            //var directory = Path.GetDirectoryName(path);

            //var assemblyPath = Path.Combine(directory, "..", "..", "..", "tools", "Generator", "SoloX.BlazorJsonLocalization.Generator.dll");

            //assemblyPath = @"C:\Temp\Test\PkgCache\solox.generatortools.test\1.0.0\tools\Generator\SoloX.BlazorJsonLocalization.Generator.dll";

            //var toolAssembly = domain.Load(assemblyPath);


            //var serviceCollection = new ServiceCollection();

            //serviceCollection.AddLogging();
            //serviceCollection.AddToolsGenerator();

            //using (var service = serviceCollection.BuildServiceProvider())
            //{
            //    var toolsGenerator = service.GetRequiredService<ITestGenerator>();

            //    toolsGenerator.Generate(compilation, classes, context);
            //}

            //var factory = new CSharpWorkspaceFactory(new LogFactory());


            //var workspace = factory.CreateWorkspace(compilation);

            //var generator = new TestGenerator(factory);
            //generator.Generate(compilation, classes, context);


            //context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("XS1000", "Title", "This is a message", "XS.Generator", DiagnosticSeverity.Warning, true), null));

            //var assemblyReferences = compilation.GetUsedAssemblyReferences();


            var distinctClasses = classes.Distinct();

            foreach (var classDeclaration in distinctClasses)
            {
                context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("XS1000", "Title", "This is a message", "XS.Generator", DiagnosticSeverity.Warning, true), classDeclaration.GetLocation()));
            }
        }

        public class LogFactory : IGeneratorLoggerFactory
        {
            public IGeneratorLogger<TType> CreateLogger<TType>()
            {
                return new Logger<TType>();
            }

            public class Logger<TType> : IGeneratorLogger<TType>
            {
                public void LogDebug(string message)
                {
                }

                public void LogDebug(Exception exception, string message)
                {
                }

                public void LogError(string message)
                {
                }

                public void LogError(Exception exception, string message)
                {
                }

                public void LogInformation(string message)
                {
                }

                public void LogInformation(Exception exception, string message)
                {
                }

                public void LogWarning(string message)
                {
                }

                public void LogWarning(Exception exception, string message)
                {
                }
            }
        }

        internal static bool IsSyntaxTargetForGeneration(SyntaxNode node) =>
            node is InterfaceDeclarationSyntax interfaceDeclaration;

        internal static InterfaceDeclarationSyntax GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
        {
            var methodDeclarationSyntax = (InterfaceDeclarationSyntax)context.Node;

            return methodDeclarationSyntax;
        }
    }
}