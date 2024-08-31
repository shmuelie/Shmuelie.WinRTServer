using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Shmuelie.WinRTServer.Windows.Com;
using Windows.Win32.Foundation;
using static Windows.Win32.PInvoke;

namespace Shmuelie.WinRTServer.Internal;

[GeneratedComClass]
internal sealed partial class BaseActivationFactoryWrapper(BaseActivationFactory factory, ComWrappers comWrappers) : IActivationFactory
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "COM method, must not throw.")]
    public unsafe HRESULT ActivateInstance(void** instance)
    {
        if (instance is null)
        {
            return HRESULT.E_INVALIDARG;
        }

        bool shouldReleaseUnknown = false;
        nint unknown = 0;
        try
        {
            object managedInstance = factory.ActivateInstance();
            unknown = comWrappers.GetOrCreateComInterfaceForObject(managedInstance, CreateComInterfaceFlags.None);
            var riid = global::Windows.Win32.System.WinRT.IActivationFactory.IID_Guid;
            var hr = (HRESULT)Marshal.QueryInterface(unknown, ref riid, out nint ppv);
            shouldReleaseUnknown = true;
            if (hr.Failed)
            {
                return hr;
            }
            *instance = (void*)ppv;

            factory.OnInstanceCreated(managedInstance);
        }
        catch (Exception e)
        {
            return (HRESULT)Marshal.GetHRForException(e);
        }
        finally
        {
            if (shouldReleaseUnknown)
            {
                Marshal.Release(unknown);
            }
        }
        return HRESULT.S_OK;
    }

    public unsafe HRESULT GetIids(uint* iidCount, Guid** iids)
    {
        if (iidCount is null || iids is null)
        {
            return HRESULT.E_INVALIDARG;
        }

        *iidCount = 1;
        *iids = (Guid*)Marshal.AllocHGlobal(sizeof(Guid));
        *iids[0] = global::Windows.Win32.System.WinRT.IActivationFactory.IID_Guid;
        return HRESULT.S_OK;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "COM method, must not throw.")]
    public unsafe HRESULT GetRuntimeClassName(global::Windows.Win32.System.WinRT.HSTRING* className)
    {
        if (className is null)
        {
            return HRESULT.E_INVALIDARG;
        }

        try
        {
            string? fullName = factory.GetType().FullName;

            if (fullName is null)
            {
                return HRESULT.E_UNEXPECTED;
            }

            fixed (char* fullNamePtr = fullName)
            {
                return WindowsCreateString((PCWSTR)fullNamePtr, (uint)fullName.Length, className);
            }
        }
        catch (Exception e)
        {
            return (HRESULT)Marshal.GetHRForException(e);
        }
    }

    public unsafe HRESULT GetTrustLevel(global::Windows.Win32.System.WinRT.TrustLevel* trustLevel)
    {
        if (trustLevel is null)
        {
            return HRESULT.E_INVALIDARG;
        }

        *trustLevel = global::Windows.Win32.System.WinRT.TrustLevel.BaseTrust;
        return HRESULT.S_OK;
    }
}
