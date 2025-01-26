using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Shmuelie.WinRTServer.Lifecycle;

/// <summary>
/// Manage the lifecycle of objects created by a server using a <see cref="ConditionalWeakTable{TKey, TValue}"/>.
/// </summary>
/// <seealso cref="IServer"/>
public sealed class ConditionalLifecycleManager
{
    private sealed class Notifier(Action action)
    {
        ~Notifier() => action();
    }

    private readonly ConditionalWeakTable<object, Notifier> _notifiers = new();
    private int _count;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConditionalLifecycleManager"/> class.
    /// </summary>
    /// <param name="server">The <see cref="IServer"/> to manage the lifecycle for.</param>
    /// <exception cref="ArgumentNullException"><paramref name="server"/> is <see langword="null"/>.</exception>
    public ConditionalLifecycleManager(IServer server)
    {
        ArgumentNullException.ThrowIfNull(server);

        server.InstanceCreated += OnInstanceCreated;
    }

    private void OnInstanceCreated(object? sender, InstanceCreatedEventArgs e)
    {
        Interlocked.Increment(ref _count);
        _notifiers.Add(e.Instance, new Notifier(() =>
        {
            if (Interlocked.Decrement(ref _count) == 0)
            {
                Empty?.Invoke(this, EventArgs.Empty);
            }
        }));
    }

    /// <summary>
    /// Occurs when all instances created by the server are collected.
    /// </summary>
    public event EventHandler? Empty;
}
