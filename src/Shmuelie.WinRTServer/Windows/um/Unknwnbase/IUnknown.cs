using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Shmuelie.Interop.Windows;

/// <summary>
/// A struct mapping the native COM <c>IUnknown</c> interface.
/// </summary>
[Guid("00000000-0000-0000-C000-000000000046")]
internal unsafe struct IUnknown
{
#pragma warning disable CS0649 // This field maps to the native object directly
    /// <summary>
    /// The vtable pointer for the current instance.
    /// </summary>
    /// <remarks>
    /// This field is never initialized in C#.
    /// </remarks>
    private readonly void** lpVtbl;
#pragma warning restore CS0649

    /// <summary>
    /// Queries a COM object for a pointer to one of its interface; identifying the interface by a reference to its interface identifier (IID).
    /// If the COM object implements the interface, then it returns a pointer to that interface after calling <see cref="AddRef"/> on it.
    /// </summary>
    /// <seealso href="https://docs.microsoft.com/en-us/windows/win32/api/unknwn/nf-unknwn-iunknown-queryinterface(refiid_void)"><c>IUnknown.QueryInterface(REFIID, void**)</c></seealso>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int QueryInterface(Guid* riid, void** ppvObject)
    {
        return ((delegate* unmanaged[Stdcall]<IUnknown*, Guid*, void**, int>)lpVtbl[0])((IUnknown*)Unsafe.AsPointer(ref this), riid, ppvObject);
    }

    /// <summary>
    /// Increments the reference count for an interface pointer to a COM object. You should call this method whenever you make a copy of an interface pointer.
    /// </summary>
    /// <seealso href="https://docs.microsoft.com/en-us/windows/win32/api/unknwn/nf-unknwn-iunknown-addref"><c>IUnknown.AddRef()</c></seealso>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef()
    {
        return ((delegate* unmanaged[Stdcall]<IUnknown*, uint>)lpVtbl[1])((IUnknown*)Unsafe.AsPointer(ref this));
    }

    /// <summary>
    /// Decrements the reference count for an interface on a COM object.
    /// </summary>
    /// <seealso href="https://docs.microsoft.com/en-us/windows/win32/api/unknwn/nf-unknwn-iunknown-release"><c>IUnknown.Release()</c></seealso>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release()
    {
        return ((delegate* unmanaged[Stdcall]<IUnknown*, uint>)lpVtbl[2])((IUnknown*)Unsafe.AsPointer(ref this));
    }
}
