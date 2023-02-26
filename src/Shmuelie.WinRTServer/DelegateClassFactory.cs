using System;

namespace Shmuelie.WinRTServer;

/// <summary>
/// Delegate based class factory for .NET types.
/// </summary>
/// <typeparam name="T">Type the factory creates.</typeparam>
/// <typeparam name="TInterface">Interface that <typeparamref name="T"/> implements.</typeparam>
/// <seealso cref="BaseClassFactory"/>
public sealed class DelegateClassFactory<T, TInterface> : BaseClassFactory where T : class, TInterface
{
    private readonly Func<T> factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateClassFactory{T, TInterface}"/> class.
    /// </summary>
    /// <param name="factory">Delegate to create instances.</param>
    public DelegateClassFactory(Func<T> factory)
    {
        this.factory = factory;
    }

    /// <inheritdoc/>
    protected internal override Guid Clsid => typeof(T).GUID;

    /// <inheritdoc/>
    protected internal override Guid Iid => typeof(TInterface).GUID;

    /// <inheritdoc/>
    protected internal override object CreateInstance()
    {
        return factory();
    }
}
