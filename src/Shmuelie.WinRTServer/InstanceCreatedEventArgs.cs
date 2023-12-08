using System;

namespace Shmuelie.WinRTServer;

/// <summary>
/// The event data for <see cref="ComServer.InstanceCreated"/>.
/// </summary>
/// <seealso cref="EventArgs"/>
/// <remarks>
/// Initializes a new instance of the <see cref="InstanceCreatedEventArgs"/> class.
/// </remarks>
/// <param name="instance">The created instance.</param>
public sealed class InstanceCreatedEventArgs(object instance) : EventArgs
{
    /// <summary>
    /// Gets the created instance.
    /// </summary>
    public object Instance
    {
        get;
    } = instance;
}
