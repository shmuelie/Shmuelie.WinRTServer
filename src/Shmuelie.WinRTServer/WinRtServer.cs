using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using Shmuelie.Interop.Windows;
using static Shmuelie.Interop.Windows.ComBaseAPI;
using static Shmuelie.Interop.Windows.RoAPI;
using static Shmuelie.Interop.Windows.Windows;
using static Shmuelie.Interop.Windows.WinString;

namespace Shmuelie.WinRTServer;

/// <summary>
/// An Out of Process Windows Runtime Server.
/// </summary>
/// <see cref="IAsyncDisposable"/>
/// <threadsafety static="true" instance="false"/>
public sealed class WinRtServer : IAsyncDisposable
{
    private readonly Dictionary<string, BaseActivationFactory> factories = [];

    private readonly DllGetActivationFactory activationFactoryCallbackWrapper;

    private unsafe readonly delegate* unmanaged[Stdcall]<HSTRING, IActivationFactory**, int> activationFactoryCallbackPointer;

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

    private RO_REGISTRATION_COOKIE registrationCookie = RO_REGISTRATION_COOKIE.NULL;

    /// <summary>
    /// Initializes a new instance of the <see cref="WinRtServer"/> class.
    /// </summary>
    public unsafe WinRtServer()
    {
        activationFactoryCallbackWrapper = ActivationFactoryCallback;
        activationFactoryCallbackPointer = (delegate* unmanaged[Stdcall]<HSTRING, IActivationFactory**, int>)Marshal.GetFunctionPointerForDelegate(activationFactoryCallbackWrapper);

        int result = RoInitialize(RO_INIT_TYPE.RO_INIT_MULTITHREADED);
        if (result != S.S_OK && result != S.S_FALSE)
        {
            Marshal.ThrowExceptionForHR(result);
        }

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

        GC.Collect();
        for (LinkedListNode<WeakReference>? node = liveServers.First; node != null; node = node.Next)
        {
            if (!node.Value.IsAlive)
            {
                var previous = node.Previous;
                liveServers.Remove(node);
                if (previous is null)
                {
                    break;
                }
                node = previous;
            }
        }

        if (liveServers.Count == 0)
        {
            Empty?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Factory_InstanceCreated(object? sender, InstanceCreatedEventArgs e)
    {
        if (IsDisposed)
        {
            return;
        }

        liveServers.AddLast(new WeakReference(e.Instance));
        InstanceCreated?.Invoke(this, e);
        firstInstanceCreated?.TrySetResult(e.Instance);
    }

    /// <summary>
    /// Register an activation factory with the server.
    /// </summary>
    /// <param name="factory">The activation factory to register.</param>
    /// <returns><see langword="true"/> if <paramref name="factory"/> was registered; otherwise, <see langword="false"/>.</returns>
    /// <remarks>Only one factory can be registered for a Activatable Class ID.</remarks>
    /// <exception cref="ArgumentNullException"><paramref name="factory"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">The server is running.</exception>
    public bool RegisterActivationFactory(BaseActivationFactory factory)
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(nameof(WinRtServer));
        }
        if (IsRunning)
        {
            throw new InvalidOperationException("Can only add activation factories when server is not running");
        }
        if (factory is null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        if (factories.ContainsKey(factory.ActivatableClassId))
        {
            return false;
        }

        factories.Add(factory.ActivatableClassId, factory);
        return true;
    }

    /// <summary>
    /// Unregister an activation factory with the server.
    /// </summary>
    /// <param name="factory">The activation factory to unregister.</param>
    /// <returns><see langword="true"/> if <paramref name="factory"/> was unregistered; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="factory"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">The server is running.</exception>
    public bool UnregisterActivationFactory(BaseActivationFactory factory)
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(nameof(WinRtServer));
        }
        if (IsRunning)
        {
            throw new InvalidOperationException("Can only remove activation factories when server is not running");
        }
        if (factory is null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        return factories.Remove(factory.ActivatableClassId);
    }

    private unsafe int ActivationFactoryCallback(HSTRING activatableClassId, IActivationFactory** factory)
    {
        if (activatableClassId == HSTRING.NULL || factory is null)
        {
            return E.E_INVALIDARG;
        }

        if (!factories.TryGetValue(activatableClassId.ToString(), out BaseActivationFactory? managedFactory))
        {
            factory = null;
            return E.E_NOINTERFACE;
        }

        *factory = (IActivationFactory*)BaseActivationFactoryProxy.Create(managedFactory);
        return S.S_OK;
    }

    /// <summary>
    /// Gets a value indicating whether the instance is disposed.
    /// </summary>
    public bool IsDisposed
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets a value indicating whether the server is running.
    /// </summary>
    public bool IsRunning => registrationCookie != RO_REGISTRATION_COOKIE.NULL;

    /// <summary>
    /// Starts the server.
    /// </summary>
    public unsafe void Start()
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(nameof(WinRtServer));
        }
        if (IsRunning)
        {
            return;
        }

        string[] managedActivatableClassIds = [.. factories.Keys];
        HSTRING* activatableClassIds = null;
        delegate* unmanaged[Stdcall]<HSTRING, IActivationFactory**, int>* activationFactoryCallbacks = null;
        try
        {
            activatableClassIds = (HSTRING*)Marshal.AllocHGlobal(sizeof(HSTRING) * managedActivatableClassIds.Length);
            for (int activatableClassIdIndex = 0; activatableClassIdIndex < managedActivatableClassIds.Length; activatableClassIdIndex++)
            {
                string managedActivatableClassId = managedActivatableClassIds[activatableClassIdIndex];
                fixed (char* managedActivatableClassIdPtr = managedActivatableClassId)
                {
                    Marshal.ThrowExceptionForHR(WindowsCreateString((ushort*)managedActivatableClassIdPtr, (uint)managedActivatableClassId.Length, &activatableClassIds[activatableClassIdIndex]));
                }
            }

            activationFactoryCallbacks = (delegate* unmanaged[Stdcall]<HSTRING, IActivationFactory**, int>*)Marshal.AllocHGlobal(sizeof(delegate* unmanaged[Stdcall]<HSTRING, IActivationFactory**, int>*) * managedActivatableClassIds.Length);
            for (int activationFactoryCallbackIndex = 0; activationFactoryCallbackIndex < managedActivatableClassIds.Length; activationFactoryCallbackIndex++)
            {
                activationFactoryCallbacks[activationFactoryCallbackIndex] = activationFactoryCallbackPointer;
            }

            fixed (RO_REGISTRATION_COOKIE* cookie = &registrationCookie)
            {
                Marshal.ThrowExceptionForHR(RoRegisterActivationFactories(activatableClassIds, activationFactoryCallbacks, (uint)managedActivatableClassIds.Length, cookie));
            }
        }
        finally
        {
            if (activationFactoryCallbacks is not null)
            {
                Marshal.FreeHGlobal((IntPtr)activationFactoryCallbacks);
            }
            if (activatableClassIds is not null)
            {
                for (int activatableClassIdIndex = 0; activatableClassIdIndex < managedActivatableClassIds.Length; activatableClassIdIndex++)
                {
                    _ = WindowsDeleteString(activatableClassIds[activatableClassIdIndex]);
                }
                Marshal.FreeHGlobal((IntPtr)activatableClassIds);
            }
        }

        firstInstanceCreated = new();
        lifetimeCheckTimer.Start();
    }

    /// <summary>
    /// Stops the server.
    /// </summary>
    public void Stop()
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(nameof(WinRtServer));
        }
        if (!IsRunning)
        {
            return;
        }

        RoRevokeActivationFactories(registrationCookie);
        registrationCookie = RO_REGISTRATION_COOKIE.NULL;

        firstInstanceCreated = null;
        lifetimeCheckTimer.Stop();
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
            throw new ObjectDisposedException(nameof(WinRtServer));
        }

        TaskCompletionSource<object>? local = firstInstanceCreated;
        if (local is null)
        {
            return null;
        }
        return await local.Task;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (!IsDisposed)
        {
            try
            {
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

                RoRevokeActivationFactories(registrationCookie);
                registrationCookie = RO_REGISTRATION_COOKIE.NULL;
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
