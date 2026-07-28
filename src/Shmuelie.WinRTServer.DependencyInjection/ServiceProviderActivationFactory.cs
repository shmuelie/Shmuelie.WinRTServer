using System;
using System.Runtime.Versioning;

namespace Shmuelie.WinRTServer;

/// <summary>
/// Activation factory that resolves instances from an <see cref="IServiceProvider"/>.
/// </summary>
/// <typeparam name="T">The type the factory creates.</typeparam>
/// <param name="provider">The service provider used to resolve instances.</param>
/// <seealso cref="BaseActivationFactory"/>
[SupportedOSPlatform("windows8.0")]
public sealed class ServiceProviderActivationFactory<T>(IServiceProvider provider) : BaseActivationFactory where T : class
{
    private readonly IServiceProvider provider = provider ?? throw new ArgumentNullException(nameof(provider));

    /// <inheritdoc/>
    public override string ActivatableClassId => typeof(T).FullName ?? throw new InvalidOperationException($"Unable to get activation class ID for type {typeof(T)}");

    /// <inheritdoc/>
    public override object ActivateInstance()
    {
        return provider.GetService(typeof(T)) ?? throw new InvalidOperationException($"No service for type '{typeof(T)}' has been registered.");
    }
}
