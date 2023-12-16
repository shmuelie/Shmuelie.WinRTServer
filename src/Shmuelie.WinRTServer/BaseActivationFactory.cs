﻿using System;

namespace Shmuelie.WinRTServer;

/// <summary>
/// Base for a WinRT Activation Factory for a .NET type.
/// </summary>
#if !NETSTANDARD
[System.Runtime.Versioning.SupportedOSPlatform("windows8.0")]
#endif
public abstract class BaseActivationFactory
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

    /// <summary>
    /// Raises the <see cref="InstanceCreated"/> event.
    /// </summary>
    /// <param name="instance">The created instance.</param>
    /// <event cref="InstanceCreated"/>
    internal void OnInstanceCreated(object instance)
    {
        InstanceCreated?.Invoke(this, new InstanceCreatedEventArgs(instance));
    }
}
