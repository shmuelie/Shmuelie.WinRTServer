using System;

namespace Shmuelie.WinRTServer;

/// <summary>
/// General activation factory for .NET types using generics.
/// </summary>
/// <typeparam name="T">The type the factory creates.</typeparam>
/// <seealso cref="BaseActivationFactory"/>
public sealed class GeneralActivationFactory<T> : BaseActivationFactory where T : class, new()
{
    /// <inheritdoc/>
    public override string ActivatableClassId => typeof(T).FullName ?? throw new InvalidOperationException($"Unable to get activation class ID for type {typeof(T)}");

    /// <inheritdoc/>
    public override object ActivateInstance()
    {
        return new T();
    }
}
