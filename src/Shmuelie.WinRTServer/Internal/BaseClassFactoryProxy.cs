using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;
using static Windows.Win32.PInvoke;

namespace Shmuelie.WinRTServer;

/// <summary>
/// CCW for <see cref="BaseClassFactory" />.
/// </summary>
internal unsafe struct BaseClassFactoryProxy
{
    private static readonly void** Vtbl = InitVtbl();

    private static void** InitVtbl()
    {
        void** lpVtbl = (void**)Marshal.AllocHGlobal(sizeof(void*) * 5);

        lpVtbl[0] = (void*)Marshal.GetFunctionPointerForDelegate(Impl.QueryInterfaceWrapper);
        lpVtbl[1] = (void*)Marshal.GetFunctionPointerForDelegate(Impl.AddRefWrapper);
        lpVtbl[2] = (void*)Marshal.GetFunctionPointerForDelegate(Impl.ReleaseWrapper);
        lpVtbl[3] = (void*)Marshal.GetFunctionPointerForDelegate(Impl.CreateInstanceWrapper);
        lpVtbl[4] = (void*)Marshal.GetFunctionPointerForDelegate(Impl.LockServerWrapper);

        return lpVtbl;
    }

    /// <summary>
    /// The vtable pointer for the current instance.
    /// </summary>
    private void** _lpVtbl;

    private GCHandle _factory;

    /// <summary>
    /// The current reference count for the object (from <c>IUnknown</c>).
    /// </summary>
    private uint _referenceCount;

    public static BaseClassFactoryProxy* Create(BaseClassFactory factory)
    {
        BaseClassFactoryProxy* @this = (BaseClassFactoryProxy*)Marshal.AllocHGlobal(sizeof(BaseClassFactoryProxy));
        @this->_lpVtbl = Vtbl;
        @this->_factory = GCHandle.Alloc(factory);
        @this->_referenceCount = 1;

        return @this;
    }

    /// <summary>
    /// Devirtualized API for <c>IUnknown.Release()</c>.
    /// </summary>
    /// <returns>The updated reference count for the current instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release()
    {
        return Impl.Release((BaseClassFactoryProxy*)Unsafe.AsPointer(ref this));
    }

    private static class Impl
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate HRESULT QueryInterfaceDelegate(BaseClassFactoryProxy* @this, Guid* riid, void** ppvObject);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate uint AddRefDelegate(BaseClassFactoryProxy* @this);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate uint ReleaseDelegate(BaseClassFactoryProxy* @this);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate HRESULT CreateInstanceDelegate(BaseClassFactoryProxy* @this, IUnknown* pUnkOuter, Guid* riid, void** ppvObject);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate HRESULT LockServerDelegate(BaseClassFactoryProxy* @this, int fLock);

        /// <summary>
        /// The cached <see cref="QueryInterfaceDelegate"/> for <c>IUnknown.QueryInterface(REFIID, void**)</c>.
        /// </summary>
        public static readonly QueryInterfaceDelegate QueryInterfaceWrapper = QueryInterface;

        /// <summary>
        /// The cached <see cref="AddRefDelegate"/> for <c>IUnknown.AddRef()</c>.
        /// </summary>
        public static readonly AddRefDelegate AddRefWrapper = AddRef;

        /// <summary>
        /// The cached <see cref="ReleaseDelegate"/> for <c>IUnknown.Release()</c>.
        /// </summary>
        public static readonly ReleaseDelegate ReleaseWrapper = Release;

        public static readonly CreateInstanceDelegate CreateInstanceWrapper = CreateInstance;

        public static readonly LockServerDelegate LockServerWrapper = LockServer;

        /// <summary>
        /// Implements <see href="https://docs.microsoft.com/en-us/windows/win32/api/unknwn/nf-unknwn-iunknown-queryinterface(refiid_void)"><c>IUnknown.QueryInterface(REFIID, void**)</c></see>.
        /// </summary>
        private static HRESULT QueryInterface(BaseClassFactoryProxy* @this, Guid* riid, void** ppvObject)
        {
            if (riid->Equals(IUnknown.IID_Guid) ||
                riid->Equals(IClassFactory.IID_Guid))
            {
                _ = Interlocked.Increment(ref Unsafe.As<uint, int>(ref @this->_referenceCount));

                *ppvObject = @this;

                return HRESULT.S_OK;
            }

            return HRESULT.E_NOINTERFACE;
        }

        /// <summary>
        /// Implements <see href="https://docs.microsoft.com/en-us/windows/win32/api/unknwn/nf-unknwn-iunknown-addref"><c>IUnknown.AddRef()</c></see>.
        /// </summary>
        private static uint AddRef(BaseClassFactoryProxy* @this)
        {
            return (uint)Interlocked.Increment(ref Unsafe.As<uint, int>(ref @this->_referenceCount));
        }

        /// <summary>
        /// Implements <see href="https://docs.microsoft.com/en-us/windows/win32/api/unknwn/nf-unknwn-iunknown-release"><c>IUnknown.Release()</c></see>.
        /// </summary>
        public static uint Release(BaseClassFactoryProxy* @this)
        {
            uint referenceCount = (uint)Interlocked.Decrement(ref Unsafe.As<uint, int>(ref @this->_referenceCount));

            if (referenceCount == 0)
            {
                @this->_factory.Free();

                Marshal.FreeHGlobal((IntPtr)@this);
            }

            return referenceCount;
        }

        public static HRESULT CreateInstance(BaseClassFactoryProxy* @this, IUnknown* pUnkOuter, Guid* riid, void** ppvObject)
        {
            try
            {
                if (pUnkOuter is not null)
                {
                    return HRESULT.CLASS_E_NOAGGREGATION;
                }

                BaseClassFactory? factory = Unsafe.As<BaseClassFactory>(@this->_factory.Target);

                if (factory is null)
                {
                    return HRESULT.E_HANDLE;
                }

                if (!riid->Equals(IUnknown.IID_Guid) && !riid->Equals(factory.Iid))
                {
                    return HRESULT.E_NOINTERFACE;
                }

                var instance = factory.CreateInstance();

                if (riid->Equals(IUnknown.IID_Guid))
                {
                    *ppvObject = (void*)Marshal.GetIUnknownForObject(instance);
                }
                else
                {
                    Type? t = Marshal.GetTypeFromCLSID(factory.Clsid);

                    if (t is null)
                    {
                        return HRESULT.E_UNEXPECTED;
                    }

                    *ppvObject = (void*)Marshal.GetComInterfaceForObject(instance, t);
                }

                factory.OnInstanceCreated(instance);
            }
            catch (Exception e)
            {
                return (HRESULT)Marshal.GetHRForException(e);
            }
            return HRESULT.S_OK;
        }

        public static HRESULT LockServer(BaseClassFactoryProxy* @this, int fLock)
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
}

