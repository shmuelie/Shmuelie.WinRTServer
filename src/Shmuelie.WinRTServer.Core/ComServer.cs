using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.Versioning;
using Shmuelie.WinRTServer.Internal;
using Shmuelie.WinRTServer.Internal.Windows;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;
using static Windows.Win32.PInvoke;

namespace Shmuelie.WinRTServer;

/// <summary>
/// An Out of Process COM Server.
/// </summary>
/// <remarks>
/// <para>Allows for types to be created using COM activation instead of WinRT activation like <see cref="WinRtServer"/>.</para>
/// <para>The server does not track the lifetime of the objects it creates. To keep the process alive until all created
/// objects have been released, use an external lifecycle helper that subscribes to <see cref="InstanceCreated"/>.</para>
/// <code language="cs">
/// <![CDATA[
/// using ComServer server = new ComServer();
/// server.RegisterClass<RemoteThing, IRemoteThing>();
/// server.Start();
/// await lifetime.WaitUntilEmptyAsync();
/// ]]>
/// </code>
/// </remarks>
/// <see cref="IDisposable"/>
/// <threadsafety static="true" instance="false"/>
[SupportedOSPlatform("windows6.0.6000")]
public sealed class ComServer : IServer, IDisposable
{
    /// <summary>
    /// Map of class factories and the registration cookie from the CLSID that the factory creates.
    /// </summary>
    private readonly Dictionary<Guid, (BaseClassFactory factory, uint cookie)> factories = [];

    private readonly StrategyBasedComWrappers comWrappers = new();

    /// <summary>
    /// Tracks whether the class objects are currently resumed (the server is running).
    /// </summary>
    private bool running;

    /// <summary>
    /// Initializes a new instance of the <see cref="ComServer"/> class.
    /// </summary>
    public ComServer()
        : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComServer"/> class with the specified options.
    /// </summary>
    /// <param name="options">Configuration for the server, or <see langword="null"/> to use the defaults.</param>
    public unsafe ComServer(ServerOptions? options)
    {
        ServerRuntimeOptions runtimeOptions = (options ?? new ServerOptions()).RuntimeOptions;

        using ComPtr<IGlobalOptions> globalOptions = default;
        Guid clsid = CLSID_GlobalOptions;
        Guid iid = IGlobalOptions.IID_Guid;
        if (CoCreateInstance(&clsid, null, CLSCTX.CLSCTX_INPROC_SERVER, &iid, (void**)globalOptions.GetAddressOf()) == HRESULT.S_OK)
        {
            globalOptions.Get()->Set(GLOBALOPT_PROPERTIES.COMGLB_RO_SETTINGS, (nuint)(GLOBALOPT_RO_FLAGS)runtimeOptions);
        }
    }

    /// <summary>
    /// Register a class factory with the server.
    /// </summary>
    /// <param name="factory">The class factory to register.</param>
    /// <param name="comWrappers">The implementation of <see cref="ComWrappers"/> to use for wrapping.</param>
    /// <returns><see langword="true"/> if <paramref name="factory"/> was registered; otherwise, <see langword="false"/>.</returns>
    /// <remarks>Only one factory can be registered for a CLSID.</remarks>
    /// <exception cref="ObjectDisposedException">The instance is disposed.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factory"/> or <paramref name="comWrappers"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">The server is running.</exception>
    /// <seealso cref="UnregisterClassFactory(Guid)"/>
    public unsafe bool RegisterClassFactory(BaseClassFactory factory, ComWrappers comWrappers)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(comWrappers);
        if (running)
        {
            throw new InvalidOperationException("Can only add class factories when server is not running.");
        }

        Guid clsid = factory.Clsid;

        if (factories.ContainsKey(clsid))
        {
            return false;
        }

        factory.InstanceCreated += Factory_InstanceCreated;

        nint wrapper = this.comWrappers.GetOrCreateComInterfaceForObject(new BaseClassFactoryWrapper(factory, comWrappers), CreateComInterfaceFlags.None);

        uint cookie;
        // REGCLS_AGILE lets COM call the factory directly from the MTA without marshaling, avoiding unexpected
        // reentrancy when registration happens on an STA thread. It is safe because the factory wrapper and the
        // objects it returns are ComWrappers CCWs, which are agile (they aggregate the free-threaded marshaler).
        CoRegisterClassObject(&clsid, (IUnknown*)wrapper, CLSCTX.CLSCTX_LOCAL_SERVER, (REGCLS.REGCLS_MULTIPLEUSE | REGCLS.REGCLS_SUSPENDED | REGCLS.REGCLS_AGILE), &cookie).ThrowOnFailure();

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
    /// <seealso cref="RegisterClassFactory(BaseClassFactory, ComWrappers)"/>
    public unsafe bool UnregisterClassFactory(Guid clsid)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);
        if (running)
        {
            throw new InvalidOperationException("Can only remove class factories when server is not running.");
        }

        if (!factories.TryGetValue(clsid, out (BaseClassFactory factory, uint cookie) data))
        {
            return false;
        }
        factories.Remove(clsid);

        data.factory.InstanceCreated -= Factory_InstanceCreated;

        CoRevokeClassObject(data.cookie).ThrowOnFailure();
        return true;
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
    /// Gets a value indicating whether the server is running.
    /// </summary>
    public bool IsRunning => running;

    /// <summary>
    /// Starts the server.
    /// </summary>
    /// <remarks>Calling <see cref="Start"/> is non-blocking.</remarks>
    /// <exception cref="ObjectDisposedException">The instance is disposed.</exception>
    public void Start()
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);
        if (running)
        {
            return;
        }

        running = true;
        CoResumeClassObjects().ThrowOnFailure();
    }

    /// <summary>
    /// Stops the server.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The instance is disposed.</exception>
    public void Stop()
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);
        if (!running)
        {
            return;
        }

        running = false;
        CoSuspendClassObjects().ThrowOnFailure();
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
    public void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        try
        {
            _ = CoSuspendClassObjects();
            running = false;

            foreach ((BaseClassFactory factory, uint cookie) in factories.Values)
            {
                factory.InstanceCreated -= Factory_InstanceCreated;
                _ = CoRevokeClassObject(cookie);
            }
            factories.Clear();
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
