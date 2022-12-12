using System;

namespace Shmuelie.WinRTServer;

/// <summary>
/// The event data for <see cref="ComServer.InstanceCreated"/>.
/// </summary>
/// <seealso cref="EventArgs"/>
public sealed class InstanceCreatedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the created instance.
    /// </summary>
    public object Instance
    {
        get;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InstanceCreatedEventArgs"/> class.
    /// </summary>
    /// <param name="instance">The created instance.</param>
    public InstanceCreatedEventArgs(object instance)
    {
        Instance = instance;
    }
}
