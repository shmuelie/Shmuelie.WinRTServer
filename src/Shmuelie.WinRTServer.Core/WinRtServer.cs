using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.Versioning;
using Shmuelie.WinRTServer.Internal;
using Shmuelie.WinRTServer.Internal.Windows;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;
using Windows.Win32.System.WinRT;
using static Windows.Win32.PInvoke;
using unsafe DllActivationCallback = delegate* unmanaged[Stdcall]<Windows.Win32.System.WinRT.HSTRING, Windows.Win32.System.WinRT.IActivationFactory**, Windows.Win32.Foundation.HRESULT>;

namespace Shmuelie.WinRTServer;

/// <summary>
/// An Out of Process Windows Runtime Server.
/// </summary>
/// <remarks>
/// <para>Allows for types to be created using WinRT activation instead of COM activation like <see cref="ComServer"/>.</para>
/// <para>The server does not track the lifetime of the objects it creates. To keep the process alive until all created
/// objects have been released, use an external lifecycle helper that subscribes to <see cref="InstanceCreated"/>.</para>
/// <code language="cs">
/// <![CDATA[
/// using WinRtServer server = new WinRtServer();
/// server.RegisterClass<RemoteThing>();
/// server.Start();
/// await lifetime.WaitUntilEmptyAsync();
/// ]]>
/// </code>
/// </remarks>
/// <see cref="IDisposable"/>
/// <threadsafety static="true" instance="false"/>
[SupportedOSPlatform("windows8.0")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1724", Justification = "No better idea")]
public sealed class WinRtServer : IServer, IDisposable
{
    /// <summary>
    /// Mapping of Activatable Class IDs to activation factories and their <see cref="ComWrappers"/> implementation.
    /// </summary>
    private readonly Dictionary<string, (BaseActivationFactory Factory, ComWrappers Wrapper)> factories = [];

    private readonly unsafe DllGetActivationFactory activationFactoryCallbackWrapper;

    private unsafe readonly DllActivationCallback activationFactoryCallbackPointer;
    private readonly StrategyBasedComWrappers comWrappers = new();

    private RO_REGISTRATION_COOKIE registrationCookie = (RO_REGISTRATION_COOKIE)0;

    /// <summary>
    /// Initializes a new instance of the <see cref="WinRtServer"/> class.
    /// </summary>
    public WinRtServer()
        : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WinRtServer"/> class with the specified options.
    /// </summary>
    /// <param name="options">Configuration for the server, or <see langword="null"/> to use the defaults.</param>
    public unsafe WinRtServer(ServerOptions? options)
    {
        ServerRuntimeOptions runtimeOptions = (options ?? new ServerOptions()).RuntimeOptions;

        activationFactoryCallbackWrapper = ActivationFactoryCallback;
        activationFactoryCallbackPointer = (DllActivationCallback)Marshal.GetFunctionPointerForDelegate(activationFactoryCallbackWrapper);

        HRESULT result = RoInitialize(RO_INIT_TYPE.RO_INIT_MULTITHREADED);
        if (result != HRESULT.S_OK && result != HRESULT.S_FALSE)
        {
            result.ThrowOnFailure();
        }

        using ComPtr<IGlobalOptions> globalOptions = default;
        Guid clsid = CLSID_GlobalOptions;
        Guid iid = IGlobalOptions.IID_Guid;
        if (CoCreateInstance(&clsid, null, CLSCTX.CLSCTX_INPROC_SERVER, &iid, (void**)globalOptions.GetAddressOf()) == HRESULT.S_OK)
        {
            globalOptions.Get()->Set(GLOBALOPT_PROPERTIES.COMGLB_RO_SETTINGS, (nuint)(GLOBALOPT_RO_FLAGS)runtimeOptions);
        }
    }

    private void Factory_InstanceCreated(object? sender, InstanceCreatedEventArgs e)
    {
        if (IsDisposed)
        {
            return;
        }

        InstanceCreated?.Invoke(this, e);
    }

    /// <summary>
    /// Register an activation factory with the server.
    /// </summary>
    /// <param name="factory">The activation factory to register.</param>
    /// <param name="comWrappers">The implementation of <see cref="ComWrappers"/> to use for wrapping.</param>
    /// <returns><see langword="true"/> if <paramref name="factory"/> was registered; otherwise, <see langword="false"/>.</returns>
    /// <remarks>Only one factory can be registered for a Activatable Class ID.</remarks>
    /// <exception cref="ArgumentNullException"><paramref name="factory"/> or <paramref name="comWrappers"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">The server is running.</exception>
    public bool RegisterActivationFactory(BaseActivationFactory factory, ComWrappers comWrappers)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);
        if (IsRunning)
        {
            throw new InvalidOperationException("Can only add activation factories when server is not running");
        }
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(comWrappers);

        if (factories.ContainsKey(factory.ActivatableClassId))
        {
            return false;
        }

        factory.InstanceCreated += Factory_InstanceCreated;
        factories.Add(factory.ActivatableClassId, (factory, comWrappers));
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
        ObjectDisposedException.ThrowIf(IsDisposed, this);
        if (IsRunning)
        {
            throw new InvalidOperationException("Can only remove activation factories when server is not running");
        }
        ArgumentNullException.ThrowIfNull(factory);

        if (factories.Remove(factory.ActivatableClassId))
        {
            factory.InstanceCreated -= Factory_InstanceCreated;
            return true;
        }
        return false;
    }

    private unsafe HRESULT ActivationFactoryCallback(HSTRING activatableClassId, IActivationFactory** factory)
    {
        if (activatableClassId == HSTRING.Null || factory is null)
        {
            return HRESULT.E_INVALIDARG;
        }

        if (!factories.TryGetValue(activatableClassId.AsString(), out var managedFactory))
        {
            factory = null;
            return HRESULT.E_NOINTERFACE;
        }

        var unknown = comWrappers.GetOrCreateComInterfaceForObject(new BaseActivationFactoryWrapper(managedFactory.Factory, managedFactory.Wrapper), CreateComInterfaceFlags.None);
        var hr = (HRESULT)Marshal.QueryInterface(unknown, in global::Windows.Win32.System.WinRT.IActivationFactory.IID_Guid, out nint ppv);
        *factory = (IActivationFactory*)ppv;
        if (unknown != 0)
        {
            Marshal.Release(unknown);
        }

        return hr;
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
    public bool IsRunning => registrationCookie != 0;

    /// <summary>
    /// Starts the server.
    /// </summary>
    /// <remarks>Calling <see cref="Start"/> is non-blocking.</remarks>
    public unsafe void Start()
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);
        if (IsRunning)
        {
            return;
        }

        string[] managedActivatableClassIds = [.. factories.Keys];
        HSTRING* activatableClassIds = null;
        DllActivationCallback* activationFactoryCallbacks = null;
        try
        {
            activatableClassIds = (HSTRING*)Marshal.AllocHGlobal(sizeof(HSTRING) * managedActivatableClassIds.Length);
            for (int activatableClassIdIndex = 0; activatableClassIdIndex < managedActivatableClassIds.Length; activatableClassIdIndex++)
            {
                string managedActivatableClassId = managedActivatableClassIds[activatableClassIdIndex];
                fixed (char* managedActivatableClassIdPtr = managedActivatableClassId)
                {
                    WindowsCreateString((PCWSTR)managedActivatableClassIdPtr, (uint)managedActivatableClassId.Length, &activatableClassIds[activatableClassIdIndex]).ThrowOnFailure();
                }
            }

            activationFactoryCallbacks = (DllActivationCallback*)Marshal.AllocHGlobal(sizeof(DllActivationCallback*) * managedActivatableClassIds.Length);
            for (int activationFactoryCallbackIndex = 0; activationFactoryCallbackIndex < managedActivatableClassIds.Length; activationFactoryCallbackIndex++)
            {
                activationFactoryCallbacks[activationFactoryCallbackIndex] = activationFactoryCallbackPointer;
            }

            fixed (RO_REGISTRATION_COOKIE* cookie = &registrationCookie)
            {
                RoRegisterActivationFactories(activatableClassIds, activationFactoryCallbacks, (uint)managedActivatableClassIds.Length, cookie).ThrowOnFailure();
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
    }

    /// <summary>
    /// Stops the server.
    /// </summary>
    public void Stop()
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);
        if (!IsRunning)
        {
            return;
        }

        RoRevokeActivationFactories(registrationCookie);
        registrationCookie = (RO_REGISTRATION_COOKIE)0;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        try
        {
            if (registrationCookie != 0)
            {
                RoRevokeActivationFactories(registrationCookie);
                registrationCookie = (RO_REGISTRATION_COOKIE)0;
            }

            foreach ((BaseActivationFactory factory, ComWrappers _) in factories.Values)
            {
                factory.InstanceCreated -= Factory_InstanceCreated;
            }
        }
        finally
        {
            IsDisposed = true;
        }
    }

    /// <summary>
    /// Occurs when the server creates an object.
    /// </summary>
    public event EventHandler<InstanceCreatedEventArgs>? InstanceCreated;
}
