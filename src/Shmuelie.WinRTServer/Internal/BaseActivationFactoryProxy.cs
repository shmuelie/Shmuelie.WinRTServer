using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Shmuelie.Interop.Windows;
using static Shmuelie.Interop.Windows.Windows;

namespace Shmuelie.WinRTServer;

internal unsafe struct BaseActivationFactoryProxy
{
    private static readonly void** Vtbl = InitVtbl();

    private static void** InitVtbl()
    {
        void** lpVtbl = (void**)Marshal.AllocHGlobal(sizeof(void*) * 7);

        lpVtbl[0] = (void*)Marshal.GetFunctionPointerForDelegate(Impl.QueryInterfaceWrapper);
        lpVtbl[1] = (void*)Marshal.GetFunctionPointerForDelegate(Impl.AddRefWrapper);
        lpVtbl[2] = (void*)Marshal.GetFunctionPointerForDelegate(Impl.ReleaseWrapper);
        lpVtbl[3] = (void*)Marshal.GetFunctionPointerForDelegate(Impl.GetIidsWrapper);
        lpVtbl[4] = (void*)Marshal.GetFunctionPointerForDelegate(Impl.GetRuntimeClassNameWrapper);
        lpVtbl[5] = (void*)Marshal.GetFunctionPointerForDelegate(Impl.GetTrustLevelWrapper);
        lpVtbl[6] = (void*)Marshal.GetFunctionPointerForDelegate(Impl.ActivateInstanceWrapper);

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

    public static BaseActivationFactoryProxy* Create(BaseActivationFactory factory)
    {
        BaseActivationFactoryProxy* @this = (BaseActivationFactoryProxy*)Marshal.AllocHGlobal(sizeof(BaseActivationFactoryProxy));
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
        return Impl.Release((BaseActivationFactoryProxy*)Unsafe.AsPointer(ref this));
    }

    private static class Impl
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int QueryInterfaceDelegate(BaseActivationFactoryProxy* @this, Guid* riid, void** ppvObject);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate uint AddRefDelegate(BaseActivationFactoryProxy* @this);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate uint ReleaseDelegate(BaseActivationFactoryProxy* @this);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetIidsDelegate(BaseActivationFactoryProxy* @this, uint* iidCount, Guid** iids);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetRuntimeClassNameDelegate(BaseActivationFactoryProxy* @this, HSTRING* className);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetTrustLevelDelegate(BaseActivationFactoryProxy* @this, TrustLevel* trustLevel);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int ActivateInstanceDelegate(BaseActivationFactoryProxy* @this, IInspectable** instance);

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

        public static readonly GetIidsDelegate GetIidsWrapper = GetIids;

        public static readonly GetRuntimeClassNameDelegate GetRuntimeClassNameWrapper = GetRuntimeClassName;

        public static readonly GetTrustLevelDelegate GetTrustLevelWrapper = GetTrustLevel;

        public static readonly ActivateInstanceDelegate ActivateInstanceWrapper = ActivateInstance;

        /// <summary>
        /// Implements <see href="https://docs.microsoft.com/en-us/windows/win32/api/unknwn/nf-unknwn-iunknown-queryinterface(refiid_void)"><c>IUnknown.QueryInterface(REFIID, void**)</c></see>.
        /// </summary>
        private static int QueryInterface(BaseActivationFactoryProxy* @this, Guid* riid, void** ppvObject)
        {
            if (riid->Equals(__uuidof<IUnknown>()) ||
                riid->Equals(__uuidof<IInspectable>()) ||
                riid->Equals(__uuidof<IActivationFactory>()))
            {
                _ = Interlocked.Increment(ref Unsafe.As<uint, int>(ref @this->_referenceCount));

                *ppvObject = @this;

                return S.S_OK;
            }

            return E.E_NOINTERFACE;
        }

        /// <summary>
        /// Implements <see href="https://docs.microsoft.com/en-us/windows/win32/api/unknwn/nf-unknwn-iunknown-addref"><c>IUnknown.AddRef()</c></see>.
        /// </summary>
        private static uint AddRef(BaseActivationFactoryProxy* @this)
        {
            return (uint)Interlocked.Increment(ref Unsafe.As<uint, int>(ref @this->_referenceCount));
        }

        /// <summary>
        /// Implements <see href="https://docs.microsoft.com/en-us/windows/win32/api/unknwn/nf-unknwn-iunknown-release"><c>IUnknown.Release()</c></see>.
        /// </summary>
        public static uint Release(BaseActivationFactoryProxy* @this)
        {
            uint referenceCount = (uint)Interlocked.Decrement(ref Unsafe.As<uint, int>(ref @this->_referenceCount));

            if (referenceCount == 0)
            {
                @this->_factory.Free();

                Marshal.FreeHGlobal((IntPtr)@this);
            }

            return referenceCount;
        }

        public static int GetIids(BaseActivationFactoryProxy* @this, uint* iidCount, Guid** iids)
        {
            if (iidCount is null || iids is null)
            {
                return E.E_INVALIDARG;
            }

            *iidCount = 1;
            *iids = (Guid*)Marshal.AllocHGlobal(sizeof(Guid));
            *iids[0] = __uuidof<IActivationFactory>();
            return S.S_OK;
        }

        public static int GetRuntimeClassName(BaseActivationFactoryProxy* @this, HSTRING* className)
        {
            try
            {
                if (className is null)
                {
                    return E.E_INVALIDARG;
                }

                BaseActivationFactory? factory = Unsafe.As<BaseActivationFactory>(@this->_factory.Target);

                if (factory is null)
                {
                    return E.E_HANDLE;
                }

                string? fullName = factory.GetType().FullName;

                if (fullName is null)
                {
                    return E.E_UNEXPECTED;
                }

                fixed (char* fullNamePtr = fullName)
                {
                    return WinString.WindowsCreateString((ushort*)fullNamePtr, (uint)fullName.Length, className);
                }

            }
            catch (Exception e)
            {
                return Marshal.GetHRForException(e);
            }
        }

        public static int GetTrustLevel(BaseActivationFactoryProxy* @this, TrustLevel* trustLevel)
        {
            if (trustLevel is null)
            {
                return E.E_INVALIDARG;
            }

            *trustLevel = TrustLevel.BaseTrust;
            return S.S_OK;
        }

        public static int ActivateInstance(BaseActivationFactoryProxy* @this, IInspectable** instance)
        {
            try
            {
                if (instance is null)
                {
                    return E.E_INVALIDARG;
                }

                BaseActivationFactory? factory = Unsafe.As<BaseActivationFactory>(@this->_factory.Target);

                if (factory is null)
                {
                    return E.E_HANDLE;
                }

                object managedInstance = factory.ActivateInstance();

                using ComPtr<IUnknown> unkwnPtr = default;
                unkwnPtr.Attach((IUnknown*)Marshal.GetIUnknownForObject(managedInstance));
                int result = unkwnPtr.CopyTo(instance);
                if (result != S.S_OK)
                {
                    return result;
                }
                return S.S_OK;

            }
            catch (Exception e)
            {
                return Marshal.GetHRForException(e);
            }
        }
    }
}
