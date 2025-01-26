using System;

namespace Shmuelie.WinRTServer;

/// <summary>
/// Common interface for <see cref="ComServer"/> and <see cref="WinRtServer"/>.
/// </summary>
/// <seealso cref="IDisposable"/>
public interface IServer : IDisposable
{
    /// <summary>
    /// Starts the server.
    /// </summary>
    void Start();

    /// <summary>
    /// Stops the server.
    /// </summary>
    void Stop();

    /// <summary>
    /// Occurs when the server creates an object.
    /// </summary>
    event EventHandler<InstanceCreatedEventArgs>? InstanceCreated;

    /// <summary>
    /// Gets a value indicating whether the server is running.
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// Gets a value indicating whether the server is disposed.
    /// </summary>
    bool IsDisposed { get; }
}
