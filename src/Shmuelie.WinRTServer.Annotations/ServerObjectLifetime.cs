namespace Shmuelie.WinRTServer;

/// <summary>
/// The lifetime used when a server object is resolved from a dependency-injection container.
/// </summary>
public enum ServerObjectLifetime
{
    /// <summary>
    /// A new instance is created for every activation.
    /// </summary>
    Transient,

    /// <summary>
    /// A single instance is shared across all activations.
    /// </summary>
    Singleton,
}
