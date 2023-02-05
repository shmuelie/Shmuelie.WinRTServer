using System;

namespace Shmuelie.WinRTServer;

/// <summary>
/// Extensions for <see cref="WinRtServer"/>.
/// </summary>
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
}
