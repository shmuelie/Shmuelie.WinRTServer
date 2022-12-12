using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Shmuelie.Interop.Windows;

/// <summary>
/// Enables a class of objects to be created.
/// </summary>
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CreateInstance(IUnknown* pUnkOuter, Guid* riid, void** ppvObject)
    {
        return ((delegate* unmanaged[Stdcall]<IClassFactory*, IUnknown*, Guid*, void**, int>)(lpVtbl[3]))((IClassFactory*)Unsafe.AsPointer(ref this), pUnkOuter, riid, ppvObject);
    }

    /// <summary>
    /// Locks an object application open in memory. This enables instances to be created more quickly.
    /// </summary>
    /// <param name="fLock">If <c>TRUE</c>, increments the lock count; if <c>FALSE</c>, decrements the lock count.</param>
    /// <returns>This method can return the standard return values E_OUTOFMEMORY, E_UNEXPECTED, E_FAIL, and S_OK.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int LockServer(int fLock)
    {
        return ((delegate* unmanaged[Stdcall]<IClassFactory*, int, int>)(lpVtbl[4]))((IClassFactory*)Unsafe.AsPointer(ref this), fLock);
    }
}
