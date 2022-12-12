using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Shmuelie.Interop.Windows;

namespace Shmuelie.WinRTServer.Windows;

[Guid("AF86E2E0-B12D-4C6A-9C5A-D7AA65101E90")]
internal unsafe partial struct IInspectable
{
    public void** lpVtbl;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetIids(uint* iidCount, Guid** iids)
    {
        return ((delegate* unmanaged[Stdcall]<IInspectable*, uint*, Guid**, int>)(lpVtbl[3]))((IInspectable*)Unsafe.AsPointer(ref this), iidCount, iids);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetRuntimeClassName(void** className)
    {
        return ((delegate* unmanaged[Stdcall]<IInspectable*, void**, int>)(lpVtbl[4]))((IInspectable*)Unsafe.AsPointer(ref this), className);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetTrustLevel(TrustLevel* trustLevel)
    {
        return ((delegate* unmanaged[Stdcall]<IInspectable*, TrustLevel*, int>)(lpVtbl[5]))((IInspectable*)Unsafe.AsPointer(ref this), trustLevel);
    }
}
