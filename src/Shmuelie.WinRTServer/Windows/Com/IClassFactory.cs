using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Shmuelie.WinRTServer.Windows.Com.Marshalling;

namespace Shmuelie.WinRTServer.Windows.Com;

[GeneratedComInterface]
[Guid("00000001-0000-0000-C000-000000000046")]
internal unsafe partial interface IClassFactory
{
    [PreserveSig]
    [return: MarshalUsing(typeof(HResultMarshaller))]
    global::Windows.Win32.Foundation.HRESULT CreateInstance(void* pUnkOuter, Guid* riid, void** ppvObject);

    [PreserveSig]
    [return: MarshalUsing(typeof(HResultMarshaller))]
    global::Windows.Win32.Foundation.HRESULT LockServer([MarshalUsing(typeof(BoolMarshaller))] global::Windows.Win32.Foundation.BOOL fLock);
}
