
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Linq;
using SoloX.BlazorJsonLocalization.Tools.Core.Impl;
using SoloX.GeneratorTools.Core.Utils;
using SoloX.GeneratorTools.Core.CSharp.Workspace.Impl;
using System.Diagnostics;

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

            IncrementalValueProvider<(Compilation, ImmutableArray<InterfaceDeclarationSyntax>)> compilationAndClasses =
                context.CompilationProvider.Combine(classDeclarations.Collect());

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

            var logFactory = new LogFactory(context, classes.First());

            var csharpWorkspaceFactory = new CSharpWorkspaceFactory(logFactory);

            var generator = new ToolsGenerator(logFactory.CreateLogger<ToolsGenerator>(), csharpWorkspaceFactory);

            generator.Generate(compilation, classes, context);
        }

        public class LogFactory : IGeneratorLoggerFactory
        {
            private readonly SourceProductionContext context;
            private readonly InterfaceDeclarationSyntax interfaceDeclarationSyntax;

            public LogFactory(SourceProductionContext context, InterfaceDeclarationSyntax interfaceDeclarationSyntax)
            {
                this.context = context;
                this.interfaceDeclarationSyntax = interfaceDeclarationSyntax;
            }

            public IGeneratorLogger<TType> CreateLogger<TType>()
            {
                return new Logger<TType>(this.context, this.interfaceDeclarationSyntax);
            }

            public class Logger<TType> : IGeneratorLogger<TType>
            {
                private readonly SourceProductionContext context;
                private readonly InterfaceDeclarationSyntax interfaceDeclarationSyntax;

                public Logger(SourceProductionContext context, InterfaceDeclarationSyntax interfaceDeclarationSyntax)
                {
                    this.context = context;
                    this.interfaceDeclarationSyntax = interfaceDeclarationSyntax;
                }

                public void LogDebug(string message)
                {
                    Log(DiagnosticSeverity.Hidden, message);
                }

                public void LogDebug(Exception exception, string message)
                {
                    Log(DiagnosticSeverity.Hidden, exception, message);
                }

                public void LogError(string message)
                {
                    Log(DiagnosticSeverity.Error, message);
                }

                public void LogError(Exception exception, string message)
                {
                    Log(DiagnosticSeverity.Error, exception, message);
                }

                public void LogInformation(string message)
                {
                    Log(DiagnosticSeverity.Info, message);
                }

                public void LogInformation(Exception exception, string message)
                {
                    Log(DiagnosticSeverity.Info, exception, message);
                }

                public void LogWarning(string message)
                {
                    Log(DiagnosticSeverity.Warning, message);
                }

                public void LogWarning(Exception exception, string message)
                {
                    Log(DiagnosticSeverity.Warning, exception, message);
                }

                private void Log(DiagnosticSeverity diagnosticSeverity, string message)
                {
                    this.context.ReportDiagnostic(
                        Diagnostic.Create(new DiagnosticDescriptor("XS9999", "BlazorJsonLocalization", message, "Generator", diagnosticSeverity, true),
                        this.interfaceDeclarationSyntax.GetLocation()));
                }

                private void Log(DiagnosticSeverity diagnosticSeverity, Exception exception, string message)
                {
                    this.context.ReportDiagnostic(
                        Diagnostic.Create(new DiagnosticDescriptor("XS9999", "BlazorJsonLocalization", message + ": " + exception.Message, "Generator", diagnosticSeverity, true),
                        this.interfaceDeclarationSyntax.GetLocation()));
                }
            }
        }

        internal static bool IsSyntaxTargetForGeneration(SyntaxNode node) => node is InterfaceDeclarationSyntax;

        internal static InterfaceDeclarationSyntax GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
        {
            var methodDeclarationSyntax = (InterfaceDeclarationSyntax)context.Node;

            return methodDeclarationSyntax;
        }
    }
}