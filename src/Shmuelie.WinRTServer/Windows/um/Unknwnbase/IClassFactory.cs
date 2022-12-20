using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Shmuelie.Interop.Windows;

/// <summary>
/// Enables a class of objects to be created.
/// </summary>
/// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/unknwn/nn-unknwn-iclassfactory">IClassFactory interface (unknwn.h)</seealso>
[Guid("00000001-0000-0000-C000-000000000046")]
internal unsafe partial struct IClassFactory
{
#pragma warning disable CS0649 // This field maps to the native object directly
    public void** lpVtbl;
#pragma warning restore CS0649

    /// <inheritdoc cref="IUnknown.QueryInterface" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int QueryInterface(Guid* riid, void** ppvObject)
    {
        return ((delegate* unmanaged[Stdcall]<IClassFactory*, Guid*, void**, int>)(lpVtbl[0]))((IClassFactory*)Unsafe.AsPointer(ref this), riid, ppvObject);
    }

    /// <inheritdoc cref="IUnknown.AddRef" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef()
    {
        return ((delegate* unmanaged[Stdcall]<IClassFactory*, uint>)(lpVtbl[1]))((IClassFactory*)Unsafe.AsPointer(ref this));
    }

    /// <inheritdoc cref="IUnknown.Release" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release()
    {
        return ((delegate* unmanaged[Stdcall]<IClassFactory*, uint>)(lpVtbl[2]))((IClassFactory*)Unsafe.AsPointer(ref this));
    }

    /// <summary>
    /// Creates an uninitialized object.
    /// </summary>
    /// <param name="pUnkOuter">If the object is being created as part of an aggregate, specify a pointer to the controlling <see cref="IUnknown"/> interface of the aggregate. Otherwise, this parameter must be <see langword="null"/>.</param>
    /// <param name="riid">A reference to the identifier of the interface to be used to communicate with the newly created object. If <paramref name="pUnkOuter"/> is <see langword="null"/>, this parameter is generally the IID of the initializing interface; if <paramref name="pUnkOuter"/> is not <see langword="null"/>, <paramref name="riid"/> must be IID of <see cref="IUnknown"/>.</param>
    /// <param name="ppvObject">The address of pointer variable that receives the interface pointer requested in <paramref name="riid"/>. Upon successful return, *<paramref name="ppvObject"/> contains the requested interface pointer. If the object does not support the interface specified in <paramref name="riid"/>, the implementation must set *<paramref name="ppvObject"/> to <see langword="null"/>.</param>
    /// <returns>
    /// <para>This method can return the standard return values <c>E_INVALIDARG</c>, <c>E_OUTOFMEMORY</c>, and <c>E_UNEXPECTED</c>, as well as the following values.</para>
    /// <list type="table">
    ///     <listheader>
    ///         <description>Return Code</description>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <description><c>S_OK</c></description>
    ///         <description>The specified object was created.</description>
    ///     </item>
    ///     <item>
    ///         <description><c>CLASS_E_NOAGGREGATION</c></description>
    ///         <description>The <paramref name="pUnkOuter"/> parameter was not <see langword="null"/> and the object does not support aggregation.</description>
    ///     </item>
    ///     <item>
    ///         <description><c>E_NOINTERFACE</c></description>
    ///         <description>The object that <paramref name="ppvObject"/> points to does not support the interface identified by <paramref name="riid"/>.</description>
    ///     </item>
    /// </list>
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CreateInstance(IUnknown* pUnkOuter, Guid* riid, void** ppvObject)
    {
        return ((delegate* unmanaged[Stdcall]<IClassFactory*, IUnknown*, Guid*, void**, int>)(lpVtbl[3]))((IClassFactory*)Unsafe.AsPointer(ref this), pUnkOuter, riid, ppvObject);
    }

    /// <summary>
    /// Locks an object application open in memory. This enables instances to be created more quickly.
    /// </summary>
    /// <param name="fLock">If <c>TRUE</c>, increments the lock count; if <c>FALSE</c>, decrements the lock count.</param>
    /// <returns>This method can return the standard return values <c>E_OUTOFMEMORY</c>, <c>E_UNEXPECTED</c>, <c>E_FAIL</c>, and <c>S_OK</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int LockServer(int fLock)
    {
        return ((delegate* unmanaged[Stdcall]<IClassFactory*, int, int>)(lpVtbl[4]))((IClassFactory*)Unsafe.AsPointer(ref this), fLock);
    }
}
