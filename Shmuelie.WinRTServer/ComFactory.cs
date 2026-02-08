using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;

namespace Shmuelie.WinRTServer;

[GeneratedComClass]
public abstract partial class ComFactory : IClassFactory
{
    public unsafe void CreateInstance([MarshalAs(UnmanagedType.Interface), Optional] object pUnkOuter, Guid* riid, [MarshalAs(UnmanagedType.Interface)] out object ppvObject)
    {
        if (pUnkOuter is not null)
        {
            throw new COMException() { HResult = HRESULT.CLASS_E_NOAGGREGATION };
        }
        ppvObject = CreateInstance(*riid);
    }

    protected abstract object CreateInstance(Guid riid);

    void IClassFactory.LockServer(BOOL fLock)
    {
    }
}
