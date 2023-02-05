using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Shmuelie.Interop.Windows;

/// <summary>
/// Provides functionality required for all Windows Runtime classes.
/// </summary>
/// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/inspectable/nn-inspectable-iinspectable">IInspectable interface (inspectable.h)</seealso>
[Guid("AF86E2E0-B12D-4C6A-9C5A-D7AA65101E90")]
internal unsafe partial struct IInspectable
{
#pragma warning disable CS0649 // This field maps to the native object directly
    public void** lpVtbl;
#pragma warning restore CS0649

    /// <inheritdoc cref="IUnknown.QueryInterface" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int QueryInterface(Guid* riid, void** ppvObject)
    {
        return ((delegate* unmanaged[Stdcall]<IInspectable*, Guid*, void**, int>)(lpVtbl[0]))((IInspectable*)Unsafe.AsPointer(ref this), riid, ppvObject);
    }

    /// <inheritdoc cref="IUnknown.AddRef" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef()
    {
        return ((delegate* unmanaged[Stdcall]<IInspectable*, uint>)(lpVtbl[1]))((IInspectable*)Unsafe.AsPointer(ref this));
    }

    /// <inheritdoc cref="IUnknown.Release" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release()
    {
        return ((delegate* unmanaged[Stdcall]<IInspectable*, uint>)(lpVtbl[2]))((IInspectable*)Unsafe.AsPointer(ref this));
    }

    /// <summary>
    /// Gets the interfaces that are implemented by the current Windows Runtime class.
    /// </summary>
    /// <param name="iidCount">The number of interfaces that are implemented by the current Windows Runtime object, excluding the <see cref="IUnknown"/> and <see cref="IInspectable"/> implementations.</param>
    /// <param name="iids">A pointer to an array that contains an IID for each interface implemented by the current Windows Runtime object. The <see cref="IUnknown"/> and <see cref="IInspectable"/> interfaces are excluded.</param>
    /// <returns>
    /// <para>This function can return the following values.</para>
    /// <list type="table">
    ///     <listheader>
    ///         <description>Return code</description>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <description><c>S_OK</c></description>
    ///         <description>The <see cref="HSTRING"/> was created successfully.</description>
    ///     </item>
    ///     <item>
    ///         <description><c>E_OUTOFMEMORY</c></description>
    ///         <description>Failed to allocate <paramref name="iids"/>.</description>
    ///     </item>
    /// </list>
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetIids(uint* iidCount, Guid** iids)
    {
        return ((delegate* unmanaged[Stdcall]<IInspectable*, uint*, Guid**, int>)(lpVtbl[3]))((IInspectable*)Unsafe.AsPointer(ref this), iidCount, iids);
    }

    /// <summary>
    /// Gets the fully qualified name of the current Windows Runtime object.
    /// </summary>
    /// <param name="className">The fully qualified name of the current Windows Runtime object.</param>
    /// <returns>
    /// <para>This function can return the following values.</para>
    /// <list type="table">
    ///     <listheader>
    ///         <description>Return code</description>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <description><c>S_OK</c></description>
    ///         <description>The <paramref name="className"/> string was created successfully.</description>
    ///     </item>
    ///     <item>
    ///         <description><c>E_OUTOFMEMORY</c></description>
    ///         <description>Failed to allocate <paramref name="className"/> string.</description>
    ///     </item>
    ///     <item>
    ///         <description><c>E_ILLEGAL_METHOD_CALL</c></description>
    ///         <description><paramref name="className"/> refers to a class factory or a static interface.</description>
    ///     </item>
    /// </list>
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetRuntimeClassName(void** className)
    {
        return ((delegate* unmanaged[Stdcall]<IInspectable*, void**, int>)(lpVtbl[4]))((IInspectable*)Unsafe.AsPointer(ref this), className);
    }

    /// <summary>
    /// Gets the trust level of the current Windows Runtime object.
    /// </summary>
    /// <param name="trustLevel">The trust level of the current Windows Runtime object. The default is <see cref="TrustLevel.BaseTrust"/>.</param>
    /// <returns>This method always returns <c>S_OK</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetTrustLevel(TrustLevel* trustLevel)
    {
        return ((delegate* unmanaged[Stdcall]<IInspectable*, TrustLevel*, int>)(lpVtbl[5]))((IInspectable*)Unsafe.AsPointer(ref this), trustLevel);
    }
}
