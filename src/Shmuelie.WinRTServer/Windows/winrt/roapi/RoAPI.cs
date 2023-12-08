using System.Runtime.InteropServices;

namespace Shmuelie.Interop.Windows;

/// <summary>
/// APIs from the <c>roapi</c> header.
/// </summary>
internal static unsafe
#if NET8_0_OR_GREATER
    partial
#endif
    class RoAPI
{
    /// <summary>
    /// Initializes the Windows Runtime on the current thread with the specified concurrency model.
    /// </summary>
    /// <param name="initType">The concurrency model for the thread. The default is <see cref="RO_INIT_TYPE.RO_INIT_MULTITHREADED"/>.</param>
    /// <returns>
    /// <para>This function can return the standard return values <c>E_INVALIDARG</c>, <c>E_OUTOFMEMORY</c>, and <c>E_UNEXPECTED</c>, as well as the following values.</para>
    /// <list type="table">
    ///     <listheader>
    ///         <description>Return code</description>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <description><c>S_OK</c></description>
    ///         <description>The Windows Runtime was initialized successfully on this thread.</description>
    ///     </item>
    ///     <item>
    ///         <description><c>S_FALSE</c></description>
    ///         <description>The Windows Runtime is already initialized on this thread.</description>
    ///     </item>
    ///     <item>
    ///         <description><c>RPC_E_CHANGED_MODE</c></description>
    ///         <description>A previous call to <see cref="RoInitialize(RO_INIT_TYPE)"/> specified the concurrency model for this thread as multithread apartment (MTA). This could also indicate that a change from neutral-threaded apartment to single-threaded apartment has occurred.</description>
    ///     </item>
    /// </list>
    /// </returns>
    /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/roapi/nf-roapi-roinitialize">RoInitialize function (roapi.h)</seealso>
#if NET8_0_OR_GREATER
    [LibraryImport("combase")]
#else
    [DllImport("combase", ExactSpelling = true)]
#endif
    public static
#if NET8_0_OR_GREATER
        partial
#else
        extern
#endif
        int RoInitialize(RO_INIT_TYPE initType);

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
    ///     <item>
    ///         <description><c>E_POINTER</c></description>
    ///         <description><paramref name="cookie"/> is <see langword="null"/></description>
    ///     </item>
    ///     <item>
    ///         <description><c>CO_E_NOT_SUPPORTED</c></description>
    ///         <description>The thread is in a neutral apartment.</description>
    ///     </item>
    ///     <item>
    ///         <description><c>CO_E_NOTINITIALIZED</c></description>
    ///         <description>The thread has not been initialized in the Windows Runtime by calling the <see cref="RoInitialize(RO_INIT_TYPE)"/> function.</description>
    ///     </item>
    ///     <item>
    ///         <description><c>CO_E_ALREADYINITIALIZED</c></description>
    ///         <description>The factory has been initialized already.</description>
    ///     </item>
    ///     <item>
    ///         <description><c>REGDB_E_CLASSNOTREG</c></description>
    ///         <description>The class is not registered as OutOfProc.</description>
    ///     </item>
    /// </list>
    /// </returns>
    /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/roapi/nf-roapi-roregisteractivationfactories">RoRegisterActivationFactories function (roapi.h)</seealso>
#if NET8_0_OR_GREATER
    [LibraryImport("combase")]
#else
    [DllImport("combase", ExactSpelling = true)]
#endif
    public static
#if NET8_0_OR_GREATER
        partial
#else
        extern
#endif
        int RoRegisterActivationFactories(HSTRING* activatableClassIds, delegate* unmanaged[Stdcall]<HSTRING, IActivationFactory**, int>* activationFactoryCallbacks, uint count, RO_REGISTRATION_COOKIE* cookie);

    /// <summary>
    /// Removes an array of registered activation factories from the Windows Runtime.
    /// </summary>
    /// <param name="cookie">A cookie that identifies the registered factories to remove.</param>
    /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/roapi/nf-roapi-rorevokeactivationfactories">RoRevokeActivationFactories function (roapi.h)</seealso>
#if NET8_0_OR_GREATER
    [LibraryImport("combase")]
#else
    [DllImport("combase", ExactSpelling = true)]
#endif
    public static
#if NET8_0_OR_GREATER
        partial
#else
        extern
#endif
        void RoRevokeActivationFactories(RO_REGISTRATION_COOKIE cookie);
}
