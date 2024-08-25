using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Shmuelie.WinRTServer.Windows.Com.Marshalling;

namespace Shmuelie.WinRTServer.Windows.Com;

[GeneratedComInterface]
[Guid("AF86E2E0-B12D-4C6A-9C5A-D7AA65101E90")]
internal unsafe partial interface IInspectable
{
    [PreserveSig]
    [return: MarshalUsing(typeof(HResultMarshaller))]
    global::Windows.Win32.Foundation.HRESULT GetIids(uint* iidCount, Guid** iids);

    [PreserveSig]
    [return: MarshalUsing(typeof(HResultMarshaller))]
    global::Windows.Win32.Foundation.HRESULT GetRuntimeClassName([MarshalUsing(typeof(HStringMarshaller))] global::Windows.Win32.System.WinRT.HSTRING* className);

    [PreserveSig]
    [return: MarshalUsing(typeof(HResultMarshaller))]
    global::Windows.Win32.Foundation.HRESULT GetTrustLevel([MarshalUsing(typeof(TrustLevelMarshaller))] global::Windows.Win32.System.WinRT.TrustLevel* trustLevel);
}
