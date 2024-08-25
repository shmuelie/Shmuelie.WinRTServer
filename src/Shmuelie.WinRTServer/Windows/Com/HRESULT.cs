using System.Runtime.InteropServices.Marshalling;
using Shmuelie.WinRTServer.Windows.Com.Marshalling;

namespace Windows.Win32.Foundation;

[NativeMarshalling(typeof(HResultMarshaller))]
partial struct HRESULT;
