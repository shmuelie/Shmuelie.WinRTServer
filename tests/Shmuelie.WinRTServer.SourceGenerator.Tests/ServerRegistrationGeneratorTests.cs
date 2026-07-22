using System.Linq;
using Microsoft.CodeAnalysis;
using Xunit;

namespace Shmuelie.WinRTServer.SourceGenerator.Tests;

public sealed class ServerRegistrationGeneratorTests
{
    private const string OneInterface = """
        using System.Runtime.InteropServices;
        using Shmuelie.WinRTServer;

        namespace Demo;

        public interface IThing { }

        [ServerClass(typeof(IThing))]
        public sealed partial class Thing : IThing { }
        """;

    [Fact]
    public void NoAnnotatedTypes_GeneratesNothing()
    {
        (var sources, var diagnostics) = GeneratorTestHelper.Run("public class C { }");

        Assert.Empty(sources);
        Assert.Empty(diagnostics);
    }

    [Fact]
    public void AnnotatedType_GeneratesComAndWinRtRegistration()
    {
        (var sources, _) = GeneratorTestHelper.Run(OneInterface);

        string text = sources.AllGeneratedText();
        Assert.Contains("GeneratedServerRegistration", text);
        Assert.Contains("GeneralClassFactory<global::Demo.Thing, global::Demo.IThing>", text);
        Assert.Contains("GeneralActivationFactory<global::Demo.Thing>", text);
    }

    [Fact]
    public void WithoutDependencyInjection_DoesNotGenerateServices()
    {
        (var sources, _) = GeneratorTestHelper.Run(OneInterface);

        Assert.DoesNotContain(sources, s => s.HintName.Contains("Services"));
    }

    [Fact]
    public void WithDependencyInjection_GeneratesServices()
    {
        (var sources, _) = GeneratorTestHelper.Run(OneInterface, referenceDependencyInjection: true);

        string text = sources.AllGeneratedText();
        Assert.Contains("GeneratedServerServices", text);
        Assert.Contains("AddServerObjects", text);
        Assert.Contains("ServiceProviderClassFactory<global::Demo.Thing, global::Demo.IThing>", text);
    }

    [Fact]
    public void SingletonLifetime_GeneratesAddSingleton()
    {
        const string source = """
            using Shmuelie.WinRTServer;

            namespace Demo;

            public interface IThing { }

            [ServerClass(typeof(IThing), Lifetime = ServerObjectLifetime.Singleton)]
            public sealed partial class Thing : IThing { }
            """;

        (var sources, _) = GeneratorTestHelper.Run(source, referenceDependencyInjection: true);

        Assert.Contains("AddSingleton", sources.AllGeneratedText());
    }

    [Fact]
    public void MoreThanThreeInterfaces_ReportsDiagnostic()
    {
        const string source = """
            using Shmuelie.WinRTServer;

            namespace Demo;

            public interface I1 { }
            public interface I2 { }
            public interface I3 { }
            public interface I4 { }

            [ServerClass(typeof(I1), typeof(I2), typeof(I3), typeof(I4))]
            public sealed partial class Thing : I1, I2, I3, I4 { }
            """;

        (_, var diagnostics) = GeneratorTestHelper.Run(source);

        Assert.Contains(diagnostics, d => d.Id == "SWRS001");
    }

    [Fact]
    public void MultipleInterfaces_GeneratesMultiArityFactory()
    {
        const string source = """
            using Shmuelie.WinRTServer;

            namespace Demo;

            public interface I1 { }
            public interface I2 { }

            [ServerClass(typeof(I1), typeof(I2))]
            public sealed partial class Thing : I1, I2 { }
            """;

        (var sources, _) = GeneratorTestHelper.Run(source);

        Assert.Contains(
            "GeneralClassFactory<global::Demo.Thing, global::Demo.I1, global::Demo.I2>",
            sources.AllGeneratedText());
    }
}
