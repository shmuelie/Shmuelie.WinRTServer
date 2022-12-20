using System;

namespace Shmuelie.Interop.Windows;

[Flags]
internal enum REGCLS
{
    REGCLS_MULTIPLEUSE = 1,
    REGCLS_SUSPENDED = 4,
}
