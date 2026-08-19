using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace Shmuelie.WinRTServer;

/// <summary>
/// General class factory for .NET types using generics.
/// </summary>
/// <typeparam name="T">Type the factory creates.</typeparam>
/// <typeparam name="TInterface">Interface that <typeparamref name="T"/> implements.</typeparam>
/// <seealso cref="BaseClassFactory"/>
[SupportedOSPlatform("windows6.0.6000")]
public sealed class GeneralClassFactory<T, TInterface> : BaseClassFactory where T : class, TInterface, new()
{
    /// <inheritdoc/>
    protected internal override Guid Clsid => typeof(T).GUID;

    /// <inheritdoc/>
    protected internal override IReadOnlyList<Guid> Iids { get; } = [typeof(TInterface).GUID];

    /// <inheritdoc/>
    protected internal override object CreateInstance()
    {
        return new T();
    }
}

/// <summary>
/// General class factory for .NET types that expose two interfaces.
/// </summary>
/// <typeparam name="T">Type the factory creates.</typeparam>
/// <typeparam name="TInterface1">First interface that <typeparamref name="T"/> implements.</typeparam>
/// <typeparam name="TInterface2">Second interface that <typeparamref name="T"/> implements.</typeparam>
/// <seealso cref="BaseClassFactory"/>
[SupportedOSPlatform("windows6.0.6000")]
public sealed class GeneralClassFactory<T, TInterface1, TInterface2> : BaseClassFactory where T : class, TInterface1, TInterface2, new()
{
    /// <inheritdoc/>
    protected internal override Guid Clsid => typeof(T).GUID;

    /// <inheritdoc/>
    protected internal override IReadOnlyList<Guid> Iids { get; } = [typeof(TInterface1).GUID, typeof(TInterface2).GUID];

    /// <inheritdoc/>
    protected internal override object CreateInstance()
    {
        return new T();
    }
}

/// <summary>
/// General class factory for .NET types that expose three interfaces.
/// </summary>
/// <typeparam name="T">Type the factory creates.</typeparam>
/// <typeparam name="TInterface1">First interface that <typeparamref name="T"/> implements.</typeparam>
/// <typeparam name="TInterface2">Second interface that <typeparamref name="T"/> implements.</typeparam>
/// <typeparam name="TInterface3">Third interface that <typeparamref name="T"/> implements.</typeparam>
/// <seealso cref="BaseClassFactory"/>
[SupportedOSPlatform("windows6.0.6000")]
public sealed class GeneralClassFactory<T, TInterface1, TInterface2, TInterface3> : BaseClassFactory where T : class, TInterface1, TInterface2, TInterface3, new()
{
    /// <inheritdoc/>
    protected internal override Guid Clsid => typeof(T).GUID;

    /// <inheritdoc/>
    protected internal override IReadOnlyList<Guid> Iids { get; } = [typeof(TInterface1).GUID, typeof(TInterface2).GUID, typeof(TInterface3).GUID];

    /// <inheritdoc/>
    protected internal override object CreateInstance()
    {
        return new T();
    }
}
