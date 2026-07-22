using System;
using System.Runtime.InteropServices;
using Shmuelie.WinRTServer;
using Xunit;

namespace Shmuelie.WinRTServer.Tests;

public sealed class ActivationFactoryTests
{
    [Fact]
    public void GeneralActivationFactory_UsesTypeFullNameAndCreates()
    {
        GeneralActivationFactory<Thing> factory = new();

        Assert.Equal(typeof(Thing).FullName, factory.ActivatableClassId);
        Assert.IsType<Thing>(factory.ActivateInstance());
    }

    [Fact]
    public void DelegateActivationFactory_UsesSuppliedDelegate()
    {
        Thing expected = new();
        DelegateActivationFactory<Thing> factory = new(() => expected);

        Assert.Same(expected, factory.ActivateInstance());
        Assert.Equal(typeof(Thing).FullName, factory.ActivatableClassId);
    }

    [Fact]
    public void ServiceProviderActivationFactory_ResolvesFromProvider()
    {
        Thing expected = new();
        ServiceProviderActivationFactory<Thing> factory =
            new(new FakeServiceProvider(typeof(Thing), expected));

        Assert.Same(expected, factory.ActivateInstance());
    }

    [Fact]
    public void ServiceProviderActivationFactory_NullProvider_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ServiceProviderActivationFactory<Thing>(null!));
    }

    [Fact]
    public void ServiceProviderActivationFactory_UnregisteredService_Throws()
    {
        ServiceProviderActivationFactory<Thing> factory =
            new(new FakeServiceProvider(typeof(Thing), null));

        Assert.Throws<InvalidOperationException>(() => factory.ActivateInstance());
    }

    [Guid("6F9619FF-8B86-D011-B42D-00CF4FC96401")]
    public sealed class Thing;

    private sealed class FakeServiceProvider(Type serviceType, object? instance) : IServiceProvider
    {
        public object? GetService(Type type) => type == serviceType ? instance : null;
    }
}
