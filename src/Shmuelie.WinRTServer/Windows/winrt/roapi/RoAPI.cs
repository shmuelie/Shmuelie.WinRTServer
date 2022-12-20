using System.Runtime.InteropServices;

namespace Shmuelie.Interop.Windows;

/// <summary>
/// APIs from the <c>roapi</c> header.
/// </summary>
internal static unsafe class RoAPI
{
    [DllImport("combase", ExactSpelling = true)]
    public static extern int RoInitialize(RO_INIT_TYPE initType);

    /// <summary>
    /// Registers an array out-of-process activation factories for a Windows Runtime exe server.
    /// </summary>
    /// <param name="activatableClassIds">An array of class identifiers that are associated with activatable runtime classes.</param>
    /// <param name="activationFactoryCallbacks">An array of callback functions that you can use to retrieve the activation factories that correspond with <paramref name="activatableClassIds"/>.</param>
    /// <param name="count">The number of items in the <paramref name="activatableClassIds"/> and <paramref name="activationFactoryCallbacks"/> arrays.</param>
    /// <param name="cookie">A cookie that identifies the registered factories.</param>
    /// <returns>
    /// <para>This function can return one of these values.</para>
    /// <list type="table">
    ///     <listheader>
    ///         <description>Return code</description>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <description><c>S_OK</c></description>
    ///         <description>The activation factory was registered successfully.</description>
    ///     </item>
    /// </list>
    /// </returns>
    [DllImport("combase", ExactSpelling = true)]
    public static extern int RoRegisterActivationFactories(HSTRING* activatableClassIds, delegate* unmanaged[Stdcall]<HSTRING, IActivationFactory**, int>* activationFactoryCallbacks, uint count, RO_REGISTRATION_COOKIE* cookie);

    [DllImport("combase", ExactSpelling = true)]
    public static extern void RoRevokeActivationFactories(RO_REGISTRATION_COOKIE cookie);
}
