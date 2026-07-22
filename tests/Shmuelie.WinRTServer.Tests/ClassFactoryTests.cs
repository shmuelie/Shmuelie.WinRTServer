using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Shmuelie.WinRTServer;
using Xunit;

namespace Shmuelie.WinRTServer.Tests;

public sealed class ClassFactoryTests
{
    [Fact]
    public void GeneralClassFactory_ExposesClsidAndIid()
    {
        GeneralClassFactory<Thing, IOne> factory = new();

        Assert.Equal(typeof(Thing).GUID, factory.Clsid);
        Assert.Equal(new[] { typeof(IOne).GUID }, factory.Iids);
    }

    [Fact]
    public void GeneralClassFactory_CreateInstance_ReturnsNewInstances()
    {
        GeneralClassFactory<Thing, IOne> factory = new();

        object a = factory.CreateInstance();
        object b = factory.CreateInstance();

        Assert.IsType<Thing>(a);
        Assert.NotSame(a, b);
    }

    [Fact]
    public void GeneralClassFactory_TwoInterfaces_ExposesBothIids()
    {
        GeneralClassFactory<Thing, IOne, ITwo> factory = new();

        Assert.Equal(new[] { typeof(IOne).GUID, typeof(ITwo).GUID }, factory.Iids);
    }

    [Fact]
    public void GeneralClassFactory_ThreeInterfaces_ExposesAllIids()
    {
        GeneralClassFactory<Thing, IOne, ITwo, IThree> factory = new();

        Assert.Equal(
            new[] { typeof(IOne).GUID, typeof(ITwo).GUID, typeof(IThree).GUID },
            factory.Iids);
    }

    [Fact]
    public void DelegateClassFactory_UsesSuppliedDelegate()
    {
        Thing expected = new();
        DelegateClassFactory<Thing, IOne> factory = new(() => expected);

        Assert.Same(expected, factory.CreateInstance());
        Assert.Equal(typeof(Thing).GUID, factory.Clsid);
        Assert.Equal(new[] { typeof(IOne).GUID }, factory.Iids);
    }

    [Fact]
    public void ServiceProviderClassFactory_ResolvesFromProvider()
    {
        Thing expected = new();
        FakeServiceProvider provider = new(typeof(Thing), expected);
        ServiceProviderClassFactory<Thing, IOne> factory = new(provider);

        Assert.Same(expected, factory.CreateInstance());
    }

    [Fact]
    public void ServiceProviderClassFactory_NullProvider_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ServiceProviderClassFactory<Thing, IOne>(null!));
    }

    [Fact]
    public void ServiceProviderClassFactory_UnregisteredService_Throws()
    {
        FakeServiceProvider provider = new(typeof(Thing), null);
        ServiceProviderClassFactory<Thing, IOne> factory = new(provider);

        Assert.Throws<InvalidOperationException>(() => factory.CreateInstance());
    }

    [Guid("6F9619FF-8B86-D011-B42D-00CF4FC964FF")]
    private sealed class Thing : IOne, ITwo, IThree;

    [Guid("00000000-0000-0000-0000-000000000101")]
    private interface IOne;

    [Guid("00000000-0000-0000-0000-000000000102")]
    private interface ITwo;

    [Guid("00000000-0000-0000-0000-000000000103")]
    private interface IThree;

    private sealed class FakeServiceProvider(Type serviceType, object? instance) : IServiceProvider
    {
        public object? GetService(Type type) => type == serviceType ? instance : null;
    }
}
