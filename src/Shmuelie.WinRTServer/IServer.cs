using System;

namespace Shmuelie.WinRTServer;

/// <summary>
/// A common surface for the out-of-process servers (<see cref="ComServer"/> and <see cref="WinRtServer"/>)
/// that external lifecycle helpers can attach to.
/// </summary>
public interface IServer
{
    /// <summary>
    /// Gets a value indicating whether the server is running.
    /// </summary>
    bool IsRunning
    {
        get;
    }

    /// <summary>
    /// Occurs when the server creates an object.
    /// </summary>
    event EventHandler<InstanceCreatedEventArgs>? InstanceCreated;
}
