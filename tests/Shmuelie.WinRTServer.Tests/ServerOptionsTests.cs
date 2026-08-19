using System;
using Shmuelie.WinRTServer;
using Xunit;

namespace Shmuelie.WinRTServer.Tests;

public sealed class ServerOptionsTests
{
    [Fact]
    public void ServerOptions_DefaultsToFastRundown()
    {
        ServerOptions options = new();

        Assert.Equal(ServerRuntimeOptions.FastRundown, options.RuntimeOptions);
    }

    [Fact]
    public void ServerOptions_RuntimeOptions_IsSettable()
    {
        ServerOptions options = new()
        {
            RuntimeOptions = ServerRuntimeOptions.None,
        };

        Assert.Equal(ServerRuntimeOptions.None, options.RuntimeOptions);
    }

    [Fact]
    public void ServerRuntimeOptions_FastRundown_HasExpectedFlagValue()
    {
        // Mirrors GLOBALOPT_RO_FLAGS.COMGLB_FAST_RUNDOWN (0x8).
        Assert.Equal(0x8, (int)ServerRuntimeOptions.FastRundown);
        Assert.Equal(0, (int)ServerRuntimeOptions.None);
    }

    [Fact]
    public void ServerRuntimeOptions_IsFlags()
    {
        ServerRuntimeOptions combined =
            ServerRuntimeOptions.FastRundown | ServerRuntimeOptions.StaModalLoopRemoveTouchMessages;

        Assert.True(combined.HasFlag(ServerRuntimeOptions.FastRundown));
        Assert.True(combined.HasFlag(ServerRuntimeOptions.StaModalLoopRemoveTouchMessages));
    }

    [Fact]
    public void InstanceCreatedEventArgs_ExposesInstance()
    {
        object instance = new();
        InstanceCreatedEventArgs args = new(instance);

        Assert.Same(instance, args.Instance);
        Assert.IsAssignableFrom<EventArgs>(args);
    }
}
