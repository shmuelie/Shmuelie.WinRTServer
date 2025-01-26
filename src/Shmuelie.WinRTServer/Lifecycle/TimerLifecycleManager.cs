using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shmuelie.WinRTServer.Lifecycle;

/// <summary>
/// Manage the lifecycle of objects created by a server using a background thread.
/// </summary>
/// <seealso cref="IServer"/>
/// <seealso cref="IDisposable"/>
public sealed class TimerLifecycleManager : IDisposable
{
    private readonly BlockingCollection<object> _instances = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="TimerLifecycleManager"/> class.
    /// </summary>
    /// <param name="server">The <see cref="IServer"/> to manage the lifecycle for.</param>
    /// <exception cref="ArgumentNullException"><paramref name="server"/> is <see langword="null"/>.</exception>
    public TimerLifecycleManager(IServer server)
    {
        ArgumentNullException.ThrowIfNull(server);

        server.InstanceCreated += OnInstanceCreated;

        _ = Task.Run(CheckingLoop);
    }

    /// <summary>
    /// Gets or sets the time to wait at most for checking if all instances are alive still.
    /// </summary>
    public TimeSpan WaitTime
    {
        get;
        set;
    } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Occurs when all instances created by the server are collected.
    /// </summary>
    public event EventHandler? Empty;

    private void OnInstanceCreated(object? sender, InstanceCreatedEventArgs e)
    {
        _instances.Add(e.Instance);
    }

    private void CheckingLoop()
    {
        LinkedList<WeakReference> references = new();
        while (true)
        {
            if (_instances.TryTake(out object? instance, WaitTime))
            {
                references.AddLast(new WeakReference(instance));
            }

            GC.Collect();
            for (LinkedListNode<WeakReference>? node = references.First; node != null; node = node.Next)
            {
                if (!node.Value.IsAlive)
                {
                    var previous = node.Previous;
                    references.Remove(node);
                    if (previous is null)
                    {
                        break;
                    }
                    node = previous;
                }
            }

            if (references.Count == 0)
            {
                Empty?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _instances.Dispose();
    }
}
