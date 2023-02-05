using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Shmuelie.Interop.Windows;

/// <summary>
/// Enables classes to be activated by the Windows Runtime.
/// </summary>
/// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/activation/nn-activation-iactivationfactory">IActivationFactory interface (activation.h)</seealso>
[Guid("00000035-0000-0000-C000-000000000046")]
internal unsafe partial struct IActivationFactory
{
#pragma warning disable CS0649 // This field maps to the native object directly
    public void** lpVtbl;
#pragma warning restore CS0649

    /// <inheritdoc cref="IUnknown.QueryInterface" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int QueryInterface(Guid* riid, void** ppvObject)
    {
        return ((delegate* unmanaged[Stdcall]<IActivationFactory*, Guid*, void**, int>)(lpVtbl[0]))((IActivationFactory*)Unsafe.AsPointer(ref this), riid, ppvObject);
    }

    /// <inheritdoc cref="IUnknown.AddRef" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef()
    {
        return ((delegate* unmanaged[Stdcall]<IActivationFactory*, uint>)(lpVtbl[1]))((IActivationFactory*)Unsafe.AsPointer(ref this));
    }

    /// <inheritdoc cref="IUnknown.Release" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release()
    {
        return ((delegate* unmanaged[Stdcall]<IActivationFactory*, uint>)(lpVtbl[2]))((IActivationFactory*)Unsafe.AsPointer(ref this));
    }

    /// <inheritdoc cref="IInspectable.GetIids(uint*, Guid**)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetIids(uint* iidCount, Guid** iids)
    {
        return ((delegate* unmanaged[Stdcall]<IActivationFactory*, uint*, Guid**, int>)(lpVtbl[3]))((IActivationFactory*)Unsafe.AsPointer(ref this), iidCount, iids);
    }

    /// <inheritdoc cref="IInspectable.GetRuntimeClassName(void**)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetRuntimeClassName(HSTRING* className)
    {
        return ((delegate* unmanaged[Stdcall]<IActivationFactory*, HSTRING*, int>)(lpVtbl[4]))((IActivationFactory*)Unsafe.AsPointer(ref this), className);
    }

    /// <inheritdoc cref="IInspectable.GetTrustLevel(TrustLevel*)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetTrustLevel(TrustLevel* trustLevel)
    {
        return ((delegate* unmanaged[Stdcall]<IActivationFactory*, TrustLevel*, int>)(lpVtbl[5]))((IActivationFactory*)Unsafe.AsPointer(ref this), trustLevel);
    }

    /// <summary>
    /// Creates a new instance of the Windows Runtime class that is associated with the current activation factory.
    /// </summary>
    /// <param name="instance">A pointer to a new instance of the class that is associated with the current activation factory.</param>
    /// <returns>
    /// <para>This function can return the following values.</para>
    /// <list type="table">
    ///     <listheader>
    ///         <description>Return code</description>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <description><c>S_OK</c></description>
    ///         <description>The new class instance was created successfully.</description>
    ///     </item>
    ///     <item>
    ///         <description><c>E_INVALIDARG</c></description>
    ///         <description><paramref name="instance"/> is <see langword="null"/>.</description>
    ///     </item>
    ///     <item>
    ///         <description><c>E_NOINTERFACE</c></description>
    ///         <description>The <see cref="IInspectable"/> interface is not implemented by the class that is associated with the current activation factory.</description>
    ///     </item>
    ///     <item>
    ///         <description><c>E_OUTOFMEMORY</c></description>
    ///         <description>Failed to create an instance of the class.</description>
    ///     </item>
    /// </list>
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ActivateInstance(IInspectable** instance)
    {
        return ((delegate* unmanaged[Stdcall]<IActivationFactory*, IInspectable**, int>)(lpVtbl[6]))((IActivationFactory*)Unsafe.AsPointer(ref this), instance);
    }
}
