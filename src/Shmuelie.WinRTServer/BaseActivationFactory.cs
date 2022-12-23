using System.Runtime.InteropServices.WindowsRuntime;

namespace Shmuelie.WinRTServer;

/// <summary>
/// Base for a WinRT Activation Factory for a .NET type.
/// </summary>
/// <seealso cref="IActivationFactory"/>
public abstract class BaseActivationFactory : IActivationFactory
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
}
