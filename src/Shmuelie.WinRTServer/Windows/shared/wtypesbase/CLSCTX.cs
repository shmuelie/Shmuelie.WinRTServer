namespace Shmuelie.Interop.Windows;

/// <summary>
/// See <see href="https://docs.microsoft.com/windows/win32/api/wtypesbase/ne-wtypesbase-clsctx"/>.
/// </summary>
internal enum CLSCTX
{
    CLSCTX_INPROC_SERVER = 0x1,
    CLSCTX_LOCAL_SERVER = 0x4,
}
