using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace Shmuelie.WinRTServer;

/// <summary>
/// Class factory that resolves instances from an <see cref="IServiceProvider"/>.
/// </summary>
/// <typeparam name="T">Type the factory creates.</typeparam>
/// <typeparam name="TInterface">Interface that <typeparamref name="T"/> implements.</typeparam>
/// <param name="provider">The service provider used to resolve instances.</param>
/// <seealso cref="BaseClassFactory"/>
[SupportedOSPlatform("windows6.0.6000")]
public sealed class ServiceProviderClassFactory<T, TInterface>(IServiceProvider provider) : BaseClassFactory where T : class, TInterface
{
    private readonly IServiceProvider provider = provider ?? throw new ArgumentNullException(nameof(provider));

    /// <inheritdoc/>
    protected internal override Guid Clsid => typeof(T).GUID;

    /// <inheritdoc/>
    protected internal override IReadOnlyList<Guid> Iids { get; } = [typeof(TInterface).GUID];

    /// <inheritdoc/>
    protected internal override object CreateInstance()
    {
        return provider.GetService(typeof(T)) ?? throw new InvalidOperationException($"No service for type '{typeof(T)}' has been registered.");
    }
}

/// <summary>
/// Class factory that resolves instances that expose two interfaces from an <see cref="IServiceProvider"/>.
/// </summary>
/// <typeparam name="T">Type the factory creates.</typeparam>
/// <typeparam name="TInterface1">First interface that <typeparamref name="T"/> implements.</typeparam>
/// <typeparam name="TInterface2">Second interface that <typeparamref name="T"/> implements.</typeparam>
/// <param name="provider">The service provider used to resolve instances.</param>
/// <seealso cref="BaseClassFactory"/>
[SupportedOSPlatform("windows6.0.6000")]
public sealed class ServiceProviderClassFactory<T, TInterface1, TInterface2>(IServiceProvider provider) : BaseClassFactory where T : class, TInterface1, TInterface2
{
    private readonly IServiceProvider provider = provider ?? throw new ArgumentNullException(nameof(provider));

    /// <inheritdoc/>
    protected internal override Guid Clsid => typeof(T).GUID;

    /// <inheritdoc/>
    protected internal override IReadOnlyList<Guid> Iids { get; } = [typeof(TInterface1).GUID, typeof(TInterface2).GUID];

    /// <inheritdoc/>
    protected internal override object CreateInstance()
    {
        return provider.GetService(typeof(T)) ?? throw new InvalidOperationException($"No service for type '{typeof(T)}' has been registered.");
    }
}

/// <summary>
/// Class factory that resolves instances that expose three interfaces from an <see cref="IServiceProvider"/>.
/// </summary>
/// <typeparam name="T">Type the factory creates.</typeparam>
/// <typeparam name="TInterface1">First interface that <typeparamref name="T"/> implements.</typeparam>
/// <typeparam name="TInterface2">Second interface that <typeparamref name="T"/> implements.</typeparam>
/// <typeparam name="TInterface3">Third interface that <typeparamref name="T"/> implements.</typeparam>
/// <param name="provider">The service provider used to resolve instances.</param>
/// <seealso cref="BaseClassFactory"/>
[SupportedOSPlatform("windows6.0.6000")]
public sealed class ServiceProviderClassFactory<T, TInterface1, TInterface2, TInterface3>(IServiceProvider provider) : BaseClassFactory where T : class, TInterface1, TInterface2, TInterface3
{
    private readonly IServiceProvider provider = provider ?? throw new ArgumentNullException(nameof(provider));

    /// <inheritdoc/>
    protected internal override Guid Clsid => typeof(T).GUID;

    /// <inheritdoc/>
    protected internal override IReadOnlyList<Guid> Iids { get; } = [typeof(TInterface1).GUID, typeof(TInterface2).GUID, typeof(TInterface3).GUID];

    /// <inheritdoc/>
    protected internal override object CreateInstance()
    {
        return provider.GetService(typeof(T)) ?? throw new InvalidOperationException($"No service for type '{typeof(T)}' has been registered.");
    }
}
