using System.Runtime.InteropServices;

namespace Shmuelie.Interop.Windows;

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal unsafe delegate int DllGetActivationFactory(void* activatableClassId, IActivationFactory** factory);
