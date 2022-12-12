using System.Runtime.InteropServices;

namespace Shmuelie.WinRTServer.Windows;

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal unsafe delegate int DllGetActivationFactory(void* activatableClassId, IActivationFactory** factory);
