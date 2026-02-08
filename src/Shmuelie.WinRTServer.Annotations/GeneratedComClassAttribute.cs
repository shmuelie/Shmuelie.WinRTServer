using System;

namespace Shmuelie.WinRTServer.Annotations;

/// <summary>
/// Specifies that the attributed type will generate a COM factory class for the specified type when the specified interfaces are requested.
/// </summary>
/// <param name="type">The type the factory will produce.</param>
/// <param name="interfaces">The interfaces the factory will support generating <paramref name="type"/> for.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class GeneratedComClassAttribute(Type type, params Type[] interfaces) : Attribute
{
    public Type Type { get; } = type;
    public Type[] Interfaces { get; } = interfaces;
}

/// <summary>
/// Specifies that the attributed type will generate a COM factory class for the specified type.
/// </summary>
/// <typeparam name="T">The type the factory will produce.</typeparam>
[AttributeUsage(AttributeTargets.Class)]
public sealed class GeneratedComClassAttribute<T> : Attribute where T : class
{
}

/// <summary>
/// Specifies that the attributed type will generate a COM factory class for the specified type when the specified interface is requested.
/// </summary>
/// <typeparam name="T">The type the factory will produce.</typeparam>
/// <typeparam name="TInterface">The interface the factory will support generating <typeparamref name="T"/> for.</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class GeneratedComClassAttribute<T, TInterface> : Attribute where T : TInterface
{
}