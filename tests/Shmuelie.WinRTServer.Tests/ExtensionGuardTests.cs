using System;
using Shmuelie.WinRTServer;
using Xunit;
using StrategyBasedCom = Shmuelie.WinRTServer.StrategyBased.ComServerExtensions;
using StrategyBasedWinRt = Shmuelie.WinRTServer.StrategyBased.WinRtServerExtensions;

namespace Shmuelie.WinRTServer.Tests;

public sealed class ExtensionGuardTests
{
    [Fact]
    public void ComServerExtensions_RegisterClass_NullServer_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => StrategyBasedCom.RegisterClass<Thing, IOne>(null!));
    }

    [Fact]
    public void ComServerExtensions_RegisterClassTwoInterfaces_NullServer_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => StrategyBasedCom.RegisterClass<Thing, IOne, ITwo>(null!));
    }

    [Fact]
    public void WinRtServerExtensions_RegisterClass_NullServer_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => StrategyBasedWinRt.RegisterClass<Thing>(null!));
    }

    [Fact]
    public void ComServerExtensions_GetOrCreateComInterfaceForObject_NullObject_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => StrategyBasedCom.GetOrCreateComInterfaceForObject(null!));
    }

    private sealed class Thing : IOne, ITwo;

    private interface IOne;

    private interface ITwo;
}
