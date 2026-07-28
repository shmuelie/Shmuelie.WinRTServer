using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Shmuelie.WinRTServer.SourceGenerator;

/// <summary>
/// Generates class/activation factories and registration helpers for types annotated with
/// <c>[ServerClass]</c>.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class ServerRegistrationGenerator : IIncrementalGenerator
{
    private const string AttributeName = "Shmuelie.WinRTServer.ServerClassAttribute";

    private static readonly DiagnosticDescriptor TooManyInterfaces = new(
        id: "SWRS001",
        title: "Too many interfaces for generated COM registration",
        messageFormat: "Class '{0}' declares more than three interfaces; generated COM registration supports at most three. Register it manually.",
        category: "Shmuelie.WinRTServer",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<ServerClassInfo> classes = context.SyntaxProvider.ForAttributeWithMetadataName(
            AttributeName,
            predicate: static (node, _) => true,
            transform: static (ctx, _) => GetInfo(ctx))
            .Where(static info => info is not null)!;

        IncrementalValueProvider<bool> hasDependencyInjection = context.CompilationProvider.Select(
            static (compilation, _) =>
                compilation.GetTypeByMetadataName("Microsoft.Extensions.DependencyInjection.IServiceCollection") is not null &&
                compilation.GetTypeByMetadataName("Shmuelie.WinRTServer.ServiceProviderClassFactory`2") is not null);

        IncrementalValueProvider<(ImmutableArray<ServerClassInfo> Classes, bool HasDependencyInjection)> combined =
            classes.Collect().Combine(hasDependencyInjection);

        context.RegisterSourceOutput(combined, static (spc, data) => Execute(spc, data.Classes, data.HasDependencyInjection));
    }

    private static ServerClassInfo? GetInfo(GeneratorAttributeSyntaxContext context)
    {
        if (context.TargetSymbol is not INamedTypeSymbol type)
        {
            return null;
        }

        AttributeData attribute = context.Attributes[0];

        List<string> interfaces = new();
        if (attribute.ConstructorArguments.Length > 0)
        {
            foreach (TypedConstant value in attribute.ConstructorArguments[0].Values)
            {
                if (value.Value is INamedTypeSymbol interfaceType)
                {
                    interfaces.Add(interfaceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
                }
            }
        }

        string lifetime = "Transient";
        foreach (KeyValuePair<string, TypedConstant> named in attribute.NamedArguments)
        {
            if (named.Key == "Lifetime" && named.Value.Value is int lifetimeValue)
            {
                lifetime = lifetimeValue == 1 ? "Singleton" : "Transient";
            }
        }

        return new ServerClassInfo(
            type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            interfaces.ToImmutableArray(),
            lifetime,
            type.Locations.FirstOrDefault());
    }

    private static void Execute(SourceProductionContext context, ImmutableArray<ServerClassInfo> items, bool hasDependencyInjection)
    {
        if (items.IsDefaultOrEmpty)
        {
            return;
        }

        StringBuilder com = new();
        StringBuilder winrt = new();
        StringBuilder diServices = new();
        StringBuilder diCom = new();
        StringBuilder diWinrt = new();

        foreach (ServerClassInfo item in items)
        {
            winrt.Append("            server.RegisterActivationFactory(new global::Shmuelie.WinRTServer.GeneralActivationFactory<")
                 .Append(item.ClassName)
                 .AppendLine(">(), comWrappers);");

            if (hasDependencyInjection)
            {
                diServices.Append("            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.Add")
                          .Append(item.Lifetime)
                          .Append('(')
                          .Append("services, typeof(").Append(item.ClassName).AppendLine("));");

                diWinrt.Append("            server.RegisterActivationFactory(new global::Shmuelie.WinRTServer.ServiceProviderActivationFactory<")
                       .Append(item.ClassName)
                       .AppendLine(">(provider), comWrappers);");
            }

            if (item.Interfaces.Length == 0)
            {
                continue;
            }

            if (item.Interfaces.Length > 3)
            {
                if (item.Location is not null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(TooManyInterfaces, item.Location, item.ClassName));
                }
                continue;
            }

            com.Append("            server.RegisterClassFactory(new global::Shmuelie.WinRTServer.GeneralClassFactory<")
               .Append(item.ClassName);
            foreach (string @interface in item.Interfaces)
            {
                com.Append(", ").Append(@interface);
            }
            com.AppendLine(">(), comWrappers);");

            if (hasDependencyInjection)
            {
                diCom.Append("            server.RegisterClassFactory(new global::Shmuelie.WinRTServer.ServiceProviderClassFactory<")
                     .Append(item.ClassName);
                foreach (string @interface in item.Interfaces)
                {
                    diCom.Append(", ").Append(@interface);
                }
                diCom.AppendLine(">(provider), comWrappers);");
            }
        }

        string source =
$@"// <auto-generated/>
#nullable enable

namespace Shmuelie.WinRTServer.Generated
{{
    using System.Runtime.InteropServices;

    /// <summary>Registration helpers generated for types annotated with <c>[ServerClass]</c>.</summary>
    public static class GeneratedServerRegistration
    {{
        /// <summary>Registers every annotated class with the <see cref=""global::Shmuelie.WinRTServer.ComServer""/> for COM activation.</summary>
        public static void RegisterGeneratedClasses(this global::Shmuelie.WinRTServer.ComServer server, ComWrappers comWrappers)
        {{
            global::System.ArgumentNullException.ThrowIfNull(server);
{com}        }}

        /// <summary>Registers every annotated class with the <see cref=""global::Shmuelie.WinRTServer.WinRtServer""/> for WinRT activation.</summary>
        public static void RegisterGeneratedClasses(this global::Shmuelie.WinRTServer.WinRtServer server, ComWrappers comWrappers)
        {{
            global::System.ArgumentNullException.ThrowIfNull(server);
{winrt}        }}
    }}
}}
";

        context.AddSource("GeneratedServerRegistration.g.cs", SourceText.From(source, Encoding.UTF8));

        if (!hasDependencyInjection)
        {
            return;
        }

        string diSource =
$@"// <auto-generated/>
#nullable enable

namespace Shmuelie.WinRTServer.Generated
{{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>Dependency-injection helpers generated for types annotated with <c>[ServerClass]</c>.</summary>
    public static class GeneratedServerServices
    {{
        /// <summary>Registers every annotated class with the service collection using its declared lifetime.</summary>
        public static IServiceCollection AddServerObjects(this IServiceCollection services)
        {{
            global::System.ArgumentNullException.ThrowIfNull(services);
{diServices}            return services;
        }}

        /// <summary>Registers every annotated class with the <see cref=""global::Shmuelie.WinRTServer.ComServer""/>, resolving instances from <paramref name=""provider""/>.</summary>
        public static void RegisterGeneratedClasses(this global::Shmuelie.WinRTServer.ComServer server, IServiceProvider provider, ComWrappers comWrappers)
        {{
            global::System.ArgumentNullException.ThrowIfNull(server);
            global::System.ArgumentNullException.ThrowIfNull(provider);
{diCom}        }}

        /// <summary>Registers every annotated class with the <see cref=""global::Shmuelie.WinRTServer.WinRtServer""/>, resolving instances from <paramref name=""provider""/>.</summary>
        public static void RegisterGeneratedClasses(this global::Shmuelie.WinRTServer.WinRtServer server, IServiceProvider provider, ComWrappers comWrappers)
        {{
            global::System.ArgumentNullException.ThrowIfNull(server);
            global::System.ArgumentNullException.ThrowIfNull(provider);
{diWinrt}        }}
    }}
}}
";

        context.AddSource("GeneratedServerServices.g.cs", SourceText.From(diSource, Encoding.UTF8));
    }

    private sealed class ServerClassInfo
    {
        public ServerClassInfo(string className, ImmutableArray<string> interfaces, string lifetime, Location? location)
        {
            ClassName = className;
            Interfaces = interfaces;
            Lifetime = lifetime;
            Location = location;
        }

        public string ClassName { get; }

        public ImmutableArray<string> Interfaces { get; }

        public string Lifetime { get; }

        public Location? Location { get; }
    }
}
