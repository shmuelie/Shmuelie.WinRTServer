using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace Shmuelie.WinRTServer.Tests;

public sealed class PollingServerLifetimeTests
{
    [Fact]
    public void Constructor_NullServer_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new PollingServerLifetime(null!));
    }

    [Fact]
    public void IsEmpty_BeforeAnyObject_IsFalse()
    {
        FakeServer server = new();
        using PollingServerLifetime lifetime = new(server, pollIntervalMilliseconds: 50);

        Assert.False(lifetime.IsEmpty);
    }

    [Fact]
    public async Task WaitForFirstObjectAsync_CompletesWithInstance()
    {
        FakeServer server = new();
        using PollingServerLifetime lifetime = new(server, pollIntervalMilliseconds: 50);
        object instance = new();

        Task<object?> wait = lifetime.WaitForFirstObjectAsync();
        server.RaiseInstanceCreated(instance);

        object? first = await wait.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Same(instance, first);
    }

    [Fact]
    public async Task WaitForFirstObjectAsync_ReturnsNull_WhenDisposedBeforeAnyObject()
    {
        FakeServer server = new();
        PollingServerLifetime lifetime = new(server, pollIntervalMilliseconds: 50);

        Task<object?> wait = lifetime.WaitForFirstObjectAsync();
        lifetime.Dispose();

        object? first = await wait.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Null(first);
    }

    [Fact]
    public async Task WaitUntilEmptyAsync_CompletesAfterObjectCollected()
    {
        FakeServer server = new();
        using PollingServerLifetime lifetime = new(server, pollIntervalMilliseconds: 50);

        CreateAndReleaseInstance(server);

        // The object has no remaining strong reference; the polling timer's GC pass
        // should observe it as collected and signal empty.
        await lifetime.WaitUntilEmptyAsync().WaitAsync(TimeSpan.FromSeconds(20));
        Assert.True(lifetime.IsEmpty);
    }

    [Fact]
    public async Task Empty_EventRaised_WhenAllObjectsReleased()
    {
        FakeServer server = new();
        using PollingServerLifetime lifetime = new(server, pollIntervalMilliseconds: 50);
        TaskCompletionSource emptyRaised = new(TaskCreationOptions.RunContinuationsAsynchronously);
        lifetime.Empty += (_, _) => emptyRaised.TrySetResult();

        CreateAndReleaseInstance(server);

        await emptyRaised.Task.WaitAsync(TimeSpan.FromSeconds(20));
    }

    [Fact]
    public async Task WaitUntilEmptyAsync_CompletesOnDispose()
    {
        FakeServer server = new();
        PollingServerLifetime lifetime = new(server, pollIntervalMilliseconds: 60000);
        object instance = new();
        server.RaiseInstanceCreated(instance);

        Task wait = lifetime.WaitUntilEmptyAsync();
        lifetime.Dispose();

        await wait.WaitAsync(TimeSpan.FromSeconds(5));
        GC.KeepAlive(instance);
    }

    // Kept in a non-inlined method so the created instance is not rooted by the caller's frame.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void CreateAndReleaseInstance(FakeServer server) => server.RaiseInstanceCreated(new object());
}
