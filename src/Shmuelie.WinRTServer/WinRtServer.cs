using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Shmuelie.Interop.Windows;
using static Shmuelie.Interop.Windows.RoAPI;
using static Shmuelie.Interop.Windows.WinString;

namespace Shmuelie.WinRTServer;

/// <summary>
/// An Out of Process Windows Runtime Server.
/// </summary>
/// <see cref="IAsyncDisposable"/>
/// <threadsafety static="true" instance="false"/>
public sealed unsafe class WinRtServer
{
    /// <summary>
    /// If Windows Runtime was initialized before starting server, do not uninitialize on close.
    /// </summary>
    private readonly bool shouldNotUninitialize;

    private readonly Dictionary<string, BaseActivationFactory> factories = new();

    private readonly DllGetActivationFactory activationFactoryCallbackWrapper;

    private readonly delegate* unmanaged[Stdcall]<HSTRING, IActivationFactory**, int> activationFactoryCallbackPointer;

    private RO_REGISTRATION_COOKIE registrationCookie = RO_REGISTRATION_COOKIE.NULL;

    /// <summary>
    /// Initializes a new instance of the <see cref="WinRtServer"/> class.
    /// </summary>
    public WinRtServer()
    {
        activationFactoryCallbackWrapper = ActivationFactoryCallback;
        activationFactoryCallbackPointer = (delegate* unmanaged[Stdcall]<HSTRING, IActivationFactory**, int>)Marshal.GetFunctionPointerForDelegate(activationFactoryCallbackWrapper);
        int result = RoInitialize(RO_INIT_TYPE.RO_INIT_MULTITHREADED);
        if (result == S.S_OK)
        {
            return;
        }
        if (result == S.S_FALSE)
        {
            shouldNotUninitialize = true;
            return;
        }
        Marshal.ThrowExceptionForHR(result);
    }

    /// <summary>
    /// Register an activation factory with the server.
    /// </summary>
    /// <param name="factory">The activation factory to register.</param>
    /// <returns><see langword="true"/> if <paramref name="factory"/> was registered; otherwise, <see langword="false"/>.</returns>
    /// <remarks>Only one factory can be registered for a Activatable Class ID.</remarks>
    /// <exception cref="ObjectDisposedException">The instance is disposed.</exception>
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

        factory.InstanceCreated += Factory_InstanceCreated;

        factories.Add(factory.ActivatableClassId, factory);
        return true;
    }

    private int ActivationFactoryCallback(HSTRING activatableClassId, IActivationFactory** factory)
    {
        if (activatableClassId.Value is null || factory is null)
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

    private void Factory_InstanceCreated(object? sender, InstanceCreatedEventArgs e)
    {
        throw new NotImplementedException();
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
    public bool IsRunning => registrationCookie.Value is not null;

    /// <summary>
    /// Starts the server.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The instance is disposed.</exception>
    public void Start()
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(nameof(WinRtServer));
        }
        if (IsRunning)
        {
            return;
        }

        string[] managedActivatableClassIds = factories.Keys.ToArray();
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
                    WindowsDeleteString(activatableClassIds[activatableClassIdIndex]);
                }
                Marshal.FreeHGlobal((IntPtr)activatableClassIds);
            }
        }
    }

    /// <summary>
    /// Stops the server.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The instance is disposed.</exception>
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
    }
}
