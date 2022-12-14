using System.Runtime.InteropServices;

namespace Shmuelie.Interop.Windows;

internal static unsafe class RoAPI
{
    [DllImport("combase", ExactSpelling = true)]
    public static extern int RoInitialize(RO_INIT_TYPE initType);

    [DllImport("combase", ExactSpelling = true)]
    public static extern int RoRegisterActivationFactories(void** activatableClassIds, delegate* unmanaged[Stdcall]<void*, IActivationFactory**, int>* activationFactoryCallbacks, uint count, void* cookie);

    [DllImport("combase", ExactSpelling = true)]
    public static extern void RoRevokeActivationFactories(void* cookie);

    [DllImport("combase", ExactSpelling = true)]
    public static extern void RoUninitialize();
}
