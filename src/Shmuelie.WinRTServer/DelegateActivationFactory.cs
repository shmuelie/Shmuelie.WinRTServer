using System;

namespace Shmuelie.WinRTServer;

/// <summary>
/// Delegate based activation factory for .NET types.
/// </summary>
/// <typeparam name="T">The type the factory creates.</typeparam>
/// <seealso cref="BaseActivationFactory"/>
/// <remarks>
/// Initializes a new instance of the <see cref="DelegateActivationFactory{T}"/> class.
/// </remarks>
/// <param name="factory">Delegate to create instances.</param>
public sealed class DelegateActivationFactory<T>(Func<T> factory) : BaseActivationFactory where T : class
{
    private readonly Func<T> factory = factory;

    /// <inheritdoc/>
    public override string ActivatableClassId => typeof(T).FullName ?? throw new InvalidOperationException($"Unable to get activation class ID for type {typeof(T)}");

    /// <inheritdoc/>
    public override object ActivateInstance()
    {
        return factory();
    }
}
