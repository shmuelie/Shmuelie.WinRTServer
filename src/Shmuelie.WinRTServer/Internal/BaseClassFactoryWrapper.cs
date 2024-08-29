using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Shmuelie.WinRTServer.Windows.Com;
using IUnknown = Windows.Win32.System.Com.IUnknown;
using static Windows.Win32.PInvoke;
using Windows.Win32.Foundation;

namespace Shmuelie.WinRTServer.Internal;

[GeneratedComClass]
internal partial class BaseClassFactoryWrapper(BaseClassFactory factory, ComWrappers comWrappers) : IClassFactory
{
    public unsafe HRESULT CreateInstance(void* pUnkOuter, Guid* riid, void** ppvObject)
    {
        if (pUnkOuter is not null)
        {
            return HRESULT.CLASS_E_NOAGGREGATION;
        }

        if (!riid->Equals(IUnknown.IID_Guid) && !riid->Equals(factory.Iid))
        {
            return HRESULT.E_NOINTERFACE;
        }

        try
        {
            var instance = factory.CreateInstance();
            var unknown = comWrappers.GetOrCreateComInterfaceForObject(instance, CreateComInterfaceFlags.None);

            if (riid->Equals(IUnknown.IID_Guid))
            {
                *ppvObject = (void*)unknown;
            }
            else
            {
                HRESULT hr = (HRESULT)Marshal.QueryInterface(unknown, ref *riid, out nint ppv);
                if (hr.Failed)
                {
                    return hr;
                }
                *ppvObject = (void*)ppv;
            }

            factory.OnInstanceCreated(instance);
        }
        catch (Exception e)
        {
            return (HRESULT)Marshal.GetHRForException(e);
        }
        return HRESULT.S_OK;
    }

    public HRESULT LockServer(BOOL fLock)
    {
        try
        {
            if (fLock != 0)
            {
                _ = CoAddRefServerProcess();
            }
            else
            {
                _ = CoReleaseServerProcess();
            }
        }
        catch (Exception e)
        {
            return (HRESULT)Marshal.GetHRForException(e);
        }
        return HRESULT.S_OK;
    }
}
