using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Shmuelie.WinRTServer.Internal;

namespace Shmuelie.WinRTServer;

/// <summary>
/// An <see cref="IServerLifetime"/> that tracks live objects deterministically by composing a counting
/// <see cref="System.Runtime.InteropServices.Marshalling.IIUnknownStrategy"/> over a fully implemented
/// <see cref="StrategyBasedComWrappers"/> instance.
/// </summary>
/// <remarks>
/// <para>Unlike <see cref="PollingServerLifetime"/>, this helper does not rely on garbage collection. Register the
/// server's classes using the <see cref="ComWrappers"/> exposed by this helper so that reference operations flow
/// through the counting strategy.</para>
/// <code language="cs">
/// <![CDATA[
/// using ComServer server = new ComServer();
/// using ReferenceCountedServerLifetime lifetime = new ReferenceCountedServerLifetime(server);
/// server.RegisterClass<RemoteThing, IRemoteThing>(lifetime.ComWrappers);
/// server.Start();
/// await lifetime.WaitUntilEmptyAsync();
/// ]]>
/// </code>
/// </remarks>
/// <threadsafety static="true" instance="true"/>
[SupportedOSPlatform("windows6.0.6000")]
public sealed class ReferenceCountedServerLifetime : IServerLifetime
{
    private readonly IServer server;
    private readonly object gate = new();
    private readonly TaskCompletionSource firstInstanceSignal = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private WeakReference? firstInstance;
    private TaskCompletionSource<bool>? emptyTcs;
    private long referenceCount;
    private bool everReferenced;
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceCountedServerLifetime"/> class.
    /// </summary>
    /// <param name="server">The server whose objects should be tracked.</param>
    /// <exception cref="ArgumentNullException"><paramref name="server"/> is <see langword="null"/>.</exception>
    public ReferenceCountedServerLifetime(IServer server)
    {
        ArgumentNullException.ThrowIfNull(server);

        this.server = server;
        server.InstanceCreated += Server_InstanceCreated;
        ComWrappers = new LifetimeTrackingComWrappers(this);
    }

    /// <summary>
    /// Gets the <see cref="ComWrappers"/> that must be used when registering classes so their references are tracked.
    /// </summary>
    public ComWrappers ComWrappers
    {
        get;
    }

    private void Server_InstanceCreated(object? sender, InstanceCreatedEventArgs e)
    {
        lock (gate)
        {
            firstInstance ??= new WeakReference(e.Instance);
        }

        firstInstanceSignal.TrySetResult();
    }

    internal void OnReferenceAdded()
    {
        lock (gate)
        {
            everReferenced = true;
            referenceCount++;
        }
    }

    internal void OnReferenceReleased()
    {
        bool empty;
        lock (gate)
        {
            if (referenceCount > 0)
            {
                referenceCount--;
            }

            empty = everReferenced && referenceCount == 0;
        }

        if (empty)
        {
            SignalEmpty();
        }
    }

    private void SignalEmpty()
    {
        Empty?.Invoke(this, EventArgs.Empty);

        TaskCompletionSource<bool>? local;
        lock (gate)
        {
            local = emptyTcs;
        }
        local?.TrySetResult(true);
    }

    /// <inheritdoc/>
    public bool IsEmpty
    {
        get
        {
            lock (gate)
            {
                return everReferenced && referenceCount == 0;
            }
        }
    }

    /// <inheritdoc/>
    public async Task<object?> WaitForFirstObjectAsync()
    {
        await firstInstanceSignal.Task.ConfigureAwait(false);
        lock (gate)
        {
            return firstInstance?.Target;
        }
    }

    /// <inheritdoc/>
    public Task WaitUntilEmptyAsync()
    {
        lock (gate)
        {
            if (everReferenced && referenceCount == 0)
            {
                return Task.CompletedTask;
            }

            emptyTcs ??= new(TaskCreationOptions.RunContinuationsAsynchronously);
            return emptyTcs.Task;
        }
    }

    /// <inheritdoc/>
    public event EventHandler? Empty;

    /// <inheritdoc/>
    public void Dispose()
    {
        TaskCompletionSource<bool>? local;
        lock (gate)
        {
            if (disposed)
            {
                return;
            }
            disposed = true;
            local = emptyTcs;
        }

        server.InstanceCreated -= Server_InstanceCreated;
        firstInstanceSignal.TrySetResult();
        local?.TrySetResult(true);
    }
}
