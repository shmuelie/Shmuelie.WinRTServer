using System;

namespace Shmuelie.WinRTServer;

/// <summary>
/// Marks an implementation class so that a factory and registration code are generated for it, allowing it to be
/// activated out-of-process by a <c>ComServer</c> or <c>WinRtServer</c>.
/// </summary>
/// <remarks>
/// <para>The interfaces passed to the constructor are the COM interfaces the class is registered for. When one or
/// more interfaces are supplied, COM (class factory) registration is generated. WinRT (activation factory)
/// registration is always generated using the class name as the activatable class id.</para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ServerClassAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServerClassAttribute"/> class.
    /// </summary>
    /// <param name="interfaces">The COM interfaces the class should be registered for.</param>
    public ServerClassAttribute(params Type[] interfaces)
    {
        Interfaces = interfaces ?? Array.Empty<Type>();
    }

    /// <summary>
    /// Gets the COM interfaces the class is registered for.
    /// </summary>
    public Type[] Interfaces
    {
        get;
    }

    /// <summary>
    /// Gets or sets the lifetime used when the class is resolved from a dependency-injection container.
    /// </summary>
    public ServerObjectLifetime Lifetime
    {
        get;
        set;
    } = ServerObjectLifetime.Transient;
}
