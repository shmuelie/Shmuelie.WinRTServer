using System;

namespace Shmuelie.Interop.Windows;

[Flags]
internal enum REGCLS
{
    REGCLS_SINGLEUSE = 0,

    REGCLS_MULTIPLEUSE = 1,

    REGCLS_MULTI_SEPARATE = 2,

    REGCLS_SUSPENDED = 4,

    REGCLS_SURROGATE = 8,

    REGCLS_AGILE = 0x10,
}
