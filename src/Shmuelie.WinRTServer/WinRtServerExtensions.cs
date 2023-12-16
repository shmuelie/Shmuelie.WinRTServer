using System;

namespace Shmuelie.WinRTServer;

/// <summary>
/// Extensions for <see cref="WinRtServer"/>.
/// </summary>
#if !NETSTANDARD
[System.Runtime.Versioning.SupportedOSPlatform("windows8.0")]
#endif
public static class WinRtServerExtensions
{
    /// <summary>
    /// Register a type with the server.
    /// </summary>
    /// <typeparam name="T">The type register.</typeparam>
    /// <param name="server">The instance.</param>
    /// <returns><see langword="true"/> if type was registered; otherwise, <see langword="false"/>.</returns>
    /// <remarks>Type can only be registered once.</remarks>
    /// <exception cref="ArgumentNullException"><paramref name="server"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">The server is running.</exception>
    public static bool RegisterClass<T>(this WinRtServer server) where T : class, new()
    {
        if (server is null)
        {
            throw new ArgumentNullException(nameof(server));
        }

        return server.RegisterActivationFactory(new GeneralActivationFactory<T>());
    }

    /// <summary>
    /// Register a type with the server.
    /// </summary>
    /// <typeparam name="T">The type register.</typeparam>
    /// <param name="server">The instance.</param>
    /// <param name="factory">Method to create instance of <typeparamref name="T"/>.</param>
    /// <returns><see langword="true"/> if type was registered; otherwise, <see langword="false"/>.</returns>
    /// <remarks>Type can only be registered once.</remarks>
    /// <exception cref="ArgumentNullException"><paramref name="server"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">The server is running.</exception>
    public static bool RegisterClass<T>(this WinRtServer server, Func<T> factory) where T : class
    {
        if (server is null)
        {
            throw new ArgumentNullException(nameof(server));
        }

        return server.RegisterActivationFactory(new DelegateActivationFactory<T>(factory));
    }

    /// <summary>
    /// Register an activation factory with the server.
    /// </summary>
    /// <typeparam name="T">The type of the factory to register.</typeparam>
    /// <param name="server">The instance.</param>
    /// <returns><see langword="true"/> if an instance of <typeparamref name="T"/> was registered; otherwise, <see langword="false"/>.</returns>
    /// <remarks>Only one factory can be registered for a Activatable Class ID.</remarks>
    /// <exception cref="ArgumentNullException"><paramref name="server"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">The server is running.</exception>
    public static bool RegisterActivationFactory<T>(this WinRtServer server) where T : BaseActivationFactory, new()
    {
        if (server is null)
        {
            throw new ArgumentNullException(nameof(server));
        }

        return server.RegisterActivationFactory(new T());
    }
}
