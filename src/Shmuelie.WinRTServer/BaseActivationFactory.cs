using System;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Shmuelie.WinRTServer;

/// <summary>
/// Base for a WinRT Activation Factory for a .NET type.
/// </summary>
/// <seealso cref="IActivationFactory"/>
public abstract class BaseActivationFactory : IActivationFactory
{
    /// <inheritdoc/>
    public abstract object ActivateInstance();

    /// <summary>
    /// Gets the Activatable Class ID.
    /// </summary>
    public abstract string ActivatableClassId
    {
        get;
    }

    /// <summary>
    /// Occurs when a new instance is created.
    /// </summary>
    public event EventHandler<InstanceCreatedEventArgs>? InstanceCreated;

    internal void OnInstanceCreated(object instance)
    {
        InstanceCreated?.Invoke(this, new InstanceCreatedEventArgs(instance));
    }
}
