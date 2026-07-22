using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Xunit;

namespace Shmuelie.WinRTServer.Tests;

public sealed class ReferenceCountedServerLifetimeTests
{
    [Fact]
    public void Constructor_NullServer_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ReferenceCountedServerLifetime(null!));
    }

    [Fact]
    public void ComWrappers_IsAvailable()
    {
        FakeServer server = new();
        using ReferenceCountedServerLifetime lifetime = new(server);

        Assert.NotNull(lifetime.ComWrappers);
        Assert.IsAssignableFrom<ComWrappers>(lifetime.ComWrappers);
    }

    [Fact]
    public void IsEmpty_BeforeAnyReference_IsFalse()
    {
        FakeServer server = new();
        using ReferenceCountedServerLifetime lifetime = new(server);

        Assert.False(lifetime.IsEmpty);
    }

    [Fact]
    public void IsEmpty_WhileReferenceOutstanding_IsFalse()
    {
        FakeServer server = new();
        using ReferenceCountedServerLifetime lifetime = new(server);

        lifetime.OnReferenceAdded();

        Assert.False(lifetime.IsEmpty);
    }

    [Fact]
    public async Task WaitUntilEmptyAsync_CompletesWhenReferencesReleasedDeterministically()
    {
        FakeServer server = new();
        using ReferenceCountedServerLifetime lifetime = new(server);

        lifetime.OnReferenceAdded();
        lifetime.OnReferenceAdded();
        lifetime.OnReferenceReleased();

        Assert.False(lifetime.IsEmpty);

        lifetime.OnReferenceReleased();

        Assert.True(lifetime.IsEmpty);
        await lifetime.WaitUntilEmptyAsync().WaitAsync(TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Empty_EventRaised_WhenLastReferenceReleased()
    {
        FakeServer server = new();
        using ReferenceCountedServerLifetime lifetime = new(server);
        TaskCompletionSource emptyRaised = new(TaskCreationOptions.RunContinuationsAsynchronously);
        lifetime.Empty += (_, _) => emptyRaised.TrySetResult();

        lifetime.OnReferenceAdded();
        lifetime.OnReferenceReleased();

        await emptyRaised.Task.WaitAsync(TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task WaitForFirstObjectAsync_CompletesWithInstance()
    {
        FakeServer server = new();
        using ReferenceCountedServerLifetime lifetime = new(server);
        object instance = new();

        Task<object?> wait = lifetime.WaitForFirstObjectAsync();
        server.RaiseInstanceCreated(instance);

        object? first = await wait.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Same(instance, first);
    }

    [Fact]
    public async Task WaitUntilEmptyAsync_CompletesOnDispose()
    {
        FakeServer server = new();
        ReferenceCountedServerLifetime lifetime = new(server);
        lifetime.OnReferenceAdded();

        Task wait = lifetime.WaitUntilEmptyAsync();
        lifetime.Dispose();

        await wait.WaitAsync(TimeSpan.FromSeconds(5));
    }
}
