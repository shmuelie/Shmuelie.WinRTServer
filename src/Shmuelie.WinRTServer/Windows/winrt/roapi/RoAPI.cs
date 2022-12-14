using System.Runtime.InteropServices;

namespace Shmuelie.Interop.Windows;

internal static unsafe class RoAPI
{
    [DllImport("combase", ExactSpelling = true)]
    public static extern int RoInitialize(RO_INIT_TYPE initType);

    [DllImport("combase", ExactSpelling = true)]
    public static extern int RoRegisterActivationFactories(HSTRING* activatableClassIds, delegate* unmanaged[Stdcall]<HSTRING, IActivationFactory**, int>* activationFactoryCallbacks, uint count, RO_REGISTRATION_COOKIE* cookie);

    [DllImport("combase", ExactSpelling = true)]
    public static extern void RoRevokeActivationFactories(RO_REGISTRATION_COOKIE cookie);

    [DllImport("combase", ExactSpelling = true)]
    public static extern void RoUninitialize();
}
