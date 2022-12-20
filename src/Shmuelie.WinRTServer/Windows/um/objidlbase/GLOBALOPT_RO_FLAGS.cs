using System;

namespace Shmuelie.Interop.Windows;

[Flags]
internal enum GLOBALOPT_RO_FLAGS
{
    /// <summary>
    /// Indicates that stubs in the current process are subjected to fast stub rundown behavior, which means that stubs are run down on termination of the client process, instead of waiting for normal cleanup timeouts to expire.
    /// </summary>
    COMGLB_FAST_RUNDOWN = 0x8,
}
