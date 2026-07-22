using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Timers;

namespace Shmuelie.WinRTServer;

/// <summary>
/// An <see cref="IServerLifetime"/> that tracks live objects by holding <see cref="WeakReference"/>s and
/// periodically collecting garbage to detect when they have all been released.
/// </summary>
/// <remarks>
/// <para>This is the behavior that previously lived inside the servers: a timer runs every minute, forces a
/// garbage collection, and prunes references that are no longer alive. When the last tracked object is gone the
/// helper signals <see cref="Empty"/>.</para>
/// <para>Because it relies on garbage collection, "empty" is detected on a delay rather than deterministically.
/// Use <see cref="ReferenceCountedServerLifetime"/> for deterministic tracking.</para>
/// </remarks>
/// <threadsafety static="true" instance="true"/>
[SupportedOSPlatform("windows6.0.6000")]
public sealed class PollingServerLifetime : IServerLifetime
{
    private readonly IServer server;
    private readonly Timer lifetimeCheckTimer;
    private readonly List<WeakReference> liveServers = new();
    private readonly object gate = new();
    private readonly TaskCompletionSource<object?> firstInstance = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private TaskCompletionSource<bool>? emptyTcs;
    private bool everCreated;
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="PollingServerLifetime"/> class.
    /// </summary>
    /// <param name="server">The server whose objects should be tracked.</param>
    /// <param name="pollIntervalMilliseconds">How often, in milliseconds, to check for released objects.</param>
    /// <exception cref="ArgumentNullException"><paramref name="server"/> is <see langword="null"/>.</exception>
    public PollingServerLifetime(IServer server, double pollIntervalMilliseconds = 60000)
    {
        ArgumentNullException.ThrowIfNull(server);

        this.server = server;
        server.InstanceCreated += Server_InstanceCreated;
        lifetimeCheckTimer = new()
        {
            Interval = pollIntervalMilliseconds,
        };
        lifetimeCheckTimer.Elapsed += LifetimeCheckTimer_Elapsed;
        lifetimeCheckTimer.Start();
    }

    private void Server_InstanceCreated(object? sender, InstanceCreatedEventArgs e)
    {
        lock (gate)
        {
            everCreated = true;
            liveServers.Add(new WeakReference(e.Instance));
        }

        firstInstance.TrySetResult(e.Instance);
    }

    private void LifetimeCheckTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        bool empty;
        lock (gate)
        {
            if (disposed)
            {
                return;
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            for (int i = liveServers.Count - 1; i >= 0; i--)
            {
                if (!liveServers[i].IsAlive)
                {
                    liveServers.RemoveAt(i);
                }
            }

            empty = everCreated && liveServers.Count == 0;
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
                return everCreated && liveServers.Count == 0;
            }
        }
    }

    /// <inheritdoc/>
    public Task<object?> WaitForFirstObjectAsync() => firstInstance.Task;

    /// <inheritdoc/>
    public Task WaitUntilEmptyAsync()
    {
        lock (gate)
        {
            if (everCreated && liveServers.Count == 0)
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
        lifetimeCheckTimer.Stop();
        lifetimeCheckTimer.Dispose();
        firstInstance.TrySetResult(null);
        local?.TrySetResult(true);
    }
}
