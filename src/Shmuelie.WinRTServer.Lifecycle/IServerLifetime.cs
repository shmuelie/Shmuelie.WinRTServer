using System;
using System.Threading.Tasks;

namespace Shmuelie.WinRTServer;

/// <summary>
/// Tracks the lifetime of the objects created by an <see cref="IServer"/> so that a host process can stay
/// alive until all created objects have been released.
/// </summary>
/// <remarks>
/// <para>Lifetime tracking is deliberately kept out of the servers themselves. Attach a helper to a started
/// server and await <see cref="WaitUntilEmptyAsync"/> to keep the process running until the last object is gone.</para>
/// </remarks>
/// <threadsafety static="true" instance="true"/>
public interface IServerLifetime : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether the server has created at least one object and all created objects have
    /// since been released.
    /// </summary>
    bool IsEmpty
    {
        get;
    }

    /// <summary>
    /// Wait for the server to have created its first object.
    /// </summary>
    /// <returns>The first object created, or <see langword="null"/> if the helper is disposed first.</returns>
    Task<object?> WaitForFirstObjectAsync();

    /// <summary>
    /// Wait until the server has created at least one object and all created objects have been released.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the server has no live objects.</returns>
    Task WaitUntilEmptyAsync();

    /// <summary>
    /// Occurs when the server has created at least one object and all created objects have been released.
    /// </summary>
    event EventHandler? Empty;
}
