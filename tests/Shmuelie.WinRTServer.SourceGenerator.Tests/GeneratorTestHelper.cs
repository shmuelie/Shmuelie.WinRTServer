using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shmuelie.WinRTServer.SourceGenerator;

namespace Shmuelie.WinRTServer.SourceGenerator.Tests;

internal static class GeneratorTestHelper
{
    /// <summary>
    /// Runs <see cref="ServerRegistrationGenerator"/> over <paramref name="source"/> and returns the result.
    /// </summary>
    /// <param name="source">The C# source to feed the generator.</param>
    /// <param name="referenceDependencyInjection">
    /// When <see langword="true"/>, adds a reference to the DI abstractions so the DI-generation path runs.
    /// </param>
    public static (ImmutableArray<GeneratedSourceResult> Sources, ImmutableArray<Diagnostic> Diagnostics) Run(
        string source,
        bool referenceDependencyInjection = false)
    {
        List<MetadataReference> references =
        [
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ServerClassAttribute).Assembly.Location),
        ];

        // netstandard / runtime facades so the netstandard2.0 attribute and its
        // typeof(...) arguments bind fully (otherwise constructor args come back empty).
        string coreDir = System.IO.Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        references.Add(MetadataReference.CreateFromFile(System.IO.Path.Combine(coreDir, "System.Runtime.dll")));
        references.Add(MetadataReference.CreateFromFile(System.IO.Path.Combine(coreDir, "netstandard.dll")));

        List<SyntaxTree> trees = [CSharpSyntaxTree.ParseText(source)];

        if (referenceDependencyInjection)
        {
            references.Add(MetadataReference.CreateFromFile(
                typeof(Microsoft.Extensions.DependencyInjection.IServiceCollection).Assembly.Location));

            // The generator only emits DI helpers when the DI package's factory type is present.
            // Provide a stub so the trigger fires without referencing the windows-only DI assembly.
            trees.Add(CSharpSyntaxTree.ParseText(
                "namespace Shmuelie.WinRTServer { public class ServiceProviderClassFactory<T, TInterface> { } }"));
        }

        CSharpCompilation compilation = CSharpCompilation.Create(
            "GeneratorTestAssembly",
            trees,
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        GeneratorDriver driver = CSharpGeneratorDriver.Create(new ServerRegistrationGenerator());
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out _);

        GeneratorRunResult result = driver.GetRunResult().Results.Single();
        return (result.GeneratedSources, result.Diagnostics);
    }

    public static string AllGeneratedText(this ImmutableArray<GeneratedSourceResult> sources) =>
        string.Join("\n", sources.Select(s => s.SourceText.ToString()));
}
