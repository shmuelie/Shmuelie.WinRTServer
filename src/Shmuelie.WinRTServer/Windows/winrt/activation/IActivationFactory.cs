﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Shmuelie.Interop.Windows;

namespace Shmuelie.WinRTServer.Windows;

[Guid("00000035-0000-0000-C000-000000000046")]
internal unsafe partial struct IActivationFactory
{
    public void** lpVtbl;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetIids(uint* iidCount, Guid** iids)
    {
        return ((delegate* unmanaged[Stdcall]<IActivationFactory*, uint*, Guid**, int>)(lpVtbl[3]))((IActivationFactory*)Unsafe.AsPointer(ref this), iidCount, iids);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetRuntimeClassName(void** className)
    {
        return ((delegate* unmanaged[Stdcall]<IActivationFactory*, void**, int>)(lpVtbl[4]))((IActivationFactory*)Unsafe.AsPointer(ref this), className);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetTrustLevel(TrustLevel* trustLevel)
    {
        return ((delegate* unmanaged[Stdcall]<IActivationFactory*, TrustLevel*, int>)(lpVtbl[5]))((IActivationFactory*)Unsafe.AsPointer(ref this), trustLevel);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ActivateInstance(IInspectable** instance)
    {
        return ((delegate* unmanaged[Stdcall]<IActivationFactory*, IInspectable**, int>)(lpVtbl[6]))((IActivationFactory*)Unsafe.AsPointer(ref this), instance);
    }
}
