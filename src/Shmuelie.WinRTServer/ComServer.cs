using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using Shmuelie.Interop.Windows;
using static Shmuelie.Interop.Windows.ComBaseAPI;
using static Shmuelie.Interop.Windows.Windows;

namespace Shmuelie.WinRTServer;

/// <summary>
/// An Out of Process COM Server.
/// </summary>
/// <see cref="IAsyncDisposable"/>
/// <threadsafety static="true" instance="false"/>
public sealed class ComServer : IAsyncDisposable
{
    /// <summary>
    /// Map of class factories and the registration cookie from the CLSID that the factory creates.
    /// </summary>
    private readonly Dictionary<Guid, (BaseClassFactory factory, uint cookie)> factories = new();

    /// <summary>
    /// Collection of created instances.
    /// </summary>
    private readonly LinkedList<WeakReference> liveServers = new();

    /// <summary>
    /// Timer that checks if all created instances have been collected.
    /// </summary>
    private readonly Timer lifetimeCheckTimer;

    /// <summary>
    /// Tracks the creation of the first instance after server is started.
    /// </summary>
    private TaskCompletionSource<object>? firstInstanceCreated;

    /// <summary>
    /// Initializes a new instance of the <see cref="ComServer"/> class.
    /// </summary>
    public unsafe ComServer()
    {
        using ComPtr<IGlobalOptions> options = default;
        Guid clsid = IGlobalOptions.CLSID;
        if (CoCreateInstance(&clsid, null, (uint)CLSCTX.CLSCTX_INPROC_SERVER, __uuidof<IGlobalOptions>(), (void**)options.GetAddressOf()) == S.S_OK)
        {
            options.Get()->Set(GLOBALOPT_PROPERTIES.COMGLB_RO_SETTINGS, (nuint)GLOBALOPT_RO_FLAGS.COMGLB_FAST_RUNDOWN);
        }

        lifetimeCheckTimer = new()
        {
            Interval = 60000,
        };
        lifetimeCheckTimer.Elapsed += LifetimeCheckTimer_Elapsed;
    }

    /// <summary>
    /// Handles <see cref="Timer.Elapsed"/> event from <see cref="lifetimeCheckTimer"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="ElapsedEventArgs"/> object that contains the event data.</param>
    private void LifetimeCheckTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        if (IsDisposed)
        {
            return;
        }

        Marshal.ThrowExceptionForHR(CoSuspendClassObjects());
        uint? instanceCount = null;
        GC.Collect();
        for (LinkedListNode<WeakReference>? node = liveServers.First; node != null; node = node.Next)
        {
            if (!node.Value.IsAlive)
            {
                instanceCount = CoReleaseServerProcess();
                var previous = node.Previous;
                liveServers.Remove(node);
                if (previous is null)
                {
                    break;
                }
                node = previous;
            }
        }

        if (instanceCount == 0)
        {
            Empty?.Invoke(this, EventArgs.Empty);
        }
        if (lifetimeCheckTimer.Enabled)
        {
            Marshal.ThrowExceptionForHR(CoResumeClassObjects());
        }
    }

    /// <summary>
    /// Register a class factory with the server.
    /// </summary>
    /// <param name="factory">The class factory to register.</param>
    /// <returns><see langword="true"/> if <paramref name="factory"/> was registered; otherwise, <see langword="false"/>.</returns>
    /// <remarks>Only one factory can be registered for a CLSID.</remarks>
    /// <exception cref="ObjectDisposedException">The instance is disposed.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factory"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">The server is running.</exception>
    /// <seealso cref="UnregisterClassFactory(Guid)"/>
    public unsafe bool RegisterClassFactory(BaseClassFactory factory)
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(nameof(ComServer));
        }
        if (factory is null)
        {
            throw new ArgumentNullException(nameof(factory));
        }
        if (lifetimeCheckTimer.Enabled)
        {
            throw new InvalidOperationException("Can only add class factories when server is not running.");
        }

        Guid clsid = factory.Clsid;

        if (factories.ContainsKey(clsid))
        {
            return false;
        }

        factory.InstanceCreated += Factory_InstanceCreated;

        using ComPtr<BaseClassFactoryProxy> proxy = default;
        proxy.Attach(BaseClassFactoryProxy.Create(factory));

        uint cookie;
        Marshal.ThrowExceptionForHR(CoRegisterClassObject(&clsid, (IUnknown*)proxy.Get(), (uint)CLSCTX.CLSCTX_LOCAL_SERVER, (uint)(REGCLS.REGCLS_MULTIPLEUSE | REGCLS.REGCLS_SUSPENDED), &cookie));

        factories.Add(clsid, (factory, cookie));
        return true;
    }

    /// <summary>
    /// Unregister a class factory.
    /// </summary>
    /// <param name="clsid">The CLSID of the server to remove.</param>
    /// <returns><see langword="true"/> if the server was removed; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ObjectDisposedException">The instance is disposed.</exception>
    /// <exception cref="InvalidOperationException">The server is running.</exception>
    /// <seealso cref="RegisterClassFactory(BaseClassFactory)"/>
    public unsafe bool UnregisterClassFactory(Guid clsid)
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(nameof(ComServer));
        }
        if (lifetimeCheckTimer.Enabled)
        {
            throw new InvalidOperationException("Can only remove class factories when server is not running.");
        }

        if (!factories.TryGetValue(clsid, out (BaseClassFactory factory, uint cookie) data))
        {
            return false;
        }
        factories.Remove(clsid);

        data.factory.InstanceCreated -= Factory_InstanceCreated;

        Marshal.ThrowExceptionForHR(CoRevokeClassObject(data.cookie));
        return true;
    }

    private void Factory_InstanceCreated(object? sender, InstanceCreatedEventArgs e)
    {
        if (IsDisposed)
        {
            return;
        }

        firstInstanceCreated?.TrySetResult(e.Instance);
        liveServers.AddLast(new WeakReference(e.Instance));
        InstanceCreated?.Invoke(this, e);
    }

    /// <summary>
    /// Gets a value indicating whether the server is running.
    /// </summary>
    public bool IsRunning => lifetimeCheckTimer.Enabled;

    /// <summary>
    /// Starts the server.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The instance is disposed.</exception>
    public void Start()
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(nameof(ComServer));
        }
        if (lifetimeCheckTimer.Enabled)
        {
            return;
        }

        firstInstanceCreated = new();
        lifetimeCheckTimer.Start();
        Marshal.ThrowExceptionForHR(CoResumeClassObjects());
    }

    /// <summary>
    /// Stops the server.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The instance is disposed.</exception>
    public void Stop()
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(nameof(ComServer));
        }
        if (!lifetimeCheckTimer.Enabled)
        {
            return;
        }

        firstInstanceCreated = null;
        lifetimeCheckTimer.Stop();
        Marshal.ThrowExceptionForHR(CoSuspendClassObjects());
    }

    /// <summary>
    /// Wait for the server to have created an object since it was started.
    /// </summary>
    /// <returns>The first object created if the server is running; otherwise <see langword="null"/>.</returns>
    /// <exception cref="ObjectDisposedException">The instance is disposed.</exception>
    public async Task<object?> WaitForFirstObjectAsync()
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(nameof(ComServer));
        }

        TaskCompletionSource<object>? local = firstInstanceCreated;
        if (local is null)
        {
            return null;
        }
        return await local.Task;
    }

    /// <summary>
    /// Gets a value indicating whether the instance is disposed.
    /// </summary>
    public bool IsDisposed
    {
        get;
        private set;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (!IsDisposed)
        {
            try
            {
                _ = CoSuspendClassObjects();

                if (liveServers.Count != 0)
                {
                    TaskCompletionSource<bool> tcs = new();
                    void Ended(object? sender, EventArgs e)
                    {
                        tcs.SetResult(true);
                    }

                    Empty += Ended;
                    await tcs.Task;
                    Empty -= Ended;
                }

                lifetimeCheckTimer.Stop();
                lifetimeCheckTimer.Dispose();

                foreach (var clsid in factories.Keys)
                {
                    _ = UnregisterClassFactory(clsid);
                }
            }
            finally
            {
                IsDisposed = true;
            }
        }
    }

    /// <summary>
    /// Occurs when the server has no live objects.
    /// </summary>
    public event EventHandler? Empty;

    /// <summary>
    /// Occurs when the server creates an object.
    /// </summary>
    public event EventHandler<InstanceCreatedEventArgs>? InstanceCreated;
}
