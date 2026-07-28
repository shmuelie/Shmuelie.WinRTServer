using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace Shmuelie.WinRTServer;

/// <summary>
/// Delegate based class factory for .NET types.
/// </summary>
/// <typeparam name="T">Type the factory creates.</typeparam>
/// <typeparam name="TInterface">Interface that <typeparamref name="T"/> implements.</typeparam>
/// <param name="factory">Delegate to create instances.</param>
/// <seealso cref="BaseClassFactory"/>
[SupportedOSPlatform("windows6.0.6000")]
public sealed class DelegateClassFactory<T, TInterface>(Func<T> factory) : BaseClassFactory where T : class, TInterface
{
    private readonly Func<T> factory = factory;

    /// <inheritdoc/>
    protected internal override Guid Clsid => typeof(T).GUID;

    /// <inheritdoc/>
    protected internal override IReadOnlyList<Guid> Iids { get; } = [typeof(TInterface).GUID];

    /// <inheritdoc/>
    protected internal override object CreateInstance()
    {
        return factory();
    }
}

/// <summary>
/// Delegate based class factory for .NET types that expose two interfaces.
/// </summary>
/// <typeparam name="T">Type the factory creates.</typeparam>
/// <typeparam name="TInterface1">First interface that <typeparamref name="T"/> implements.</typeparam>
/// <typeparam name="TInterface2">Second interface that <typeparamref name="T"/> implements.</typeparam>
/// <param name="factory">Delegate to create instances.</param>
/// <seealso cref="BaseClassFactory"/>
[SupportedOSPlatform("windows6.0.6000")]
public sealed class DelegateClassFactory<T, TInterface1, TInterface2>(Func<T> factory) : BaseClassFactory where T : class, TInterface1, TInterface2
{
    private readonly Func<T> factory = factory;

    /// <inheritdoc/>
    protected internal override Guid Clsid => typeof(T).GUID;

    /// <inheritdoc/>
    protected internal override IReadOnlyList<Guid> Iids { get; } = [typeof(TInterface1).GUID, typeof(TInterface2).GUID];

    /// <inheritdoc/>
    protected internal override object CreateInstance()
    {
        return factory();
    }
}

/// <summary>
/// Delegate based class factory for .NET types that expose three interfaces.
/// </summary>
/// <typeparam name="T">Type the factory creates.</typeparam>
/// <typeparam name="TInterface1">First interface that <typeparamref name="T"/> implements.</typeparam>
/// <typeparam name="TInterface2">Second interface that <typeparamref name="T"/> implements.</typeparam>
/// <typeparam name="TInterface3">Third interface that <typeparamref name="T"/> implements.</typeparam>
/// <param name="factory">Delegate to create instances.</param>
/// <seealso cref="BaseClassFactory"/>
[SupportedOSPlatform("windows6.0.6000")]
public sealed class DelegateClassFactory<T, TInterface1, TInterface2, TInterface3>(Func<T> factory) : BaseClassFactory where T : class, TInterface1, TInterface2, TInterface3
{
    private readonly Func<T> factory = factory;

    /// <inheritdoc/>
    protected internal override Guid Clsid => typeof(T).GUID;

    /// <inheritdoc/>
    protected internal override IReadOnlyList<Guid> Iids { get; } = [typeof(TInterface1).GUID, typeof(TInterface2).GUID, typeof(TInterface3).GUID];

    /// <inheritdoc/>
    protected internal override object CreateInstance()
    {
        return factory();
    }
}
