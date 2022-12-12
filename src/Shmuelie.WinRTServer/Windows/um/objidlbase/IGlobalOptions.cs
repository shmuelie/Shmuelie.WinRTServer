using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Shmuelie.Interop.Windows;

[Guid("0000015B-0000-0000-C000-000000000046")]
internal unsafe partial struct IGlobalOptions
{
    public static readonly Guid CLSID = new(0x0000034b, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

#pragma warning disable CS0649
    public void** lpVtbl;
#pragma warning restore CS0649

    /// <inheritdoc cref="IUnknown.QueryInterface" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int QueryInterface(Guid* riid, void** ppvObject)
    {
        return ((delegate* unmanaged[Stdcall]<IGlobalOptions*, Guid*, void**, int>)(lpVtbl[0]))((IGlobalOptions*)Unsafe.AsPointer(ref this), riid, ppvObject);
    }

    /// <inheritdoc cref="IUnknown.AddRef" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef()
    {
        return ((delegate* unmanaged[Stdcall]<IGlobalOptions*, uint>)(lpVtbl[1]))((IGlobalOptions*)Unsafe.AsPointer(ref this));
    }

    /// <inheritdoc cref="IUnknown.Release" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release()
    {
        return ((delegate* unmanaged[Stdcall]<IGlobalOptions*, uint>)(lpVtbl[2]))((IGlobalOptions*)Unsafe.AsPointer(ref this));
    }

    /// <summary>
    /// Sets the specified global property of the COM runtime.
    /// </summary>
    /// <param name="dwProperty">The global property of the COM runtime. For a list of properties that can be set with this method, see <see cref="IGlobalOptions" />.</param>
    /// <param name="dwValue">The value of the property.<b>Important</b>  For the <c>COMGLB_APPID</c> property, this parameter must specify a pointer to the APPID GUID.</param>
    /// <returns>The return value is <c>S_OK</c> if the property was set successfully.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Set(GLOBALOPT_PROPERTIES dwProperty, nuint dwValue)
    {
        return ((delegate* unmanaged[Stdcall]<IGlobalOptions*, GLOBALOPT_PROPERTIES, nuint, int>)(lpVtbl[3]))((IGlobalOptions*)Unsafe.AsPointer(ref this), dwProperty, dwValue);
    }

    /// <summary>
    /// Queries the specified global property of the COM runtime.
    /// </summary>
    /// <param name="dwProperty">The global property of the COM runtime. For a list of properties that can be set with this method, see <see cref="IGlobalOptions" />.</param>
    /// <param name="pdwValue">The value of the property.<b>Important</b>  For the COMGLB_APPID property, this parameter receives a pointer to the AppID GUID.</param>
    /// <returns>The return value is S_OK if the property is queried successfully.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Query(GLOBALOPT_PROPERTIES dwProperty, nuint* pdwValue)
    {
        return ((delegate* unmanaged[Stdcall]<IGlobalOptions*, GLOBALOPT_PROPERTIES, nuint*, int>)(lpVtbl[4]))((IGlobalOptions*)Unsafe.AsPointer(ref this), dwProperty, pdwValue);
    }
}
