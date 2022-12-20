namespace Shmuelie.Interop.Windows;

/// <summary>
/// Values that are used in activation calls to indicate the execution contexts in which an object is to be run.
/// </summary>
/// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/wtypesbase/ne-wtypesbase-clsctx">CLSCTX enumeration (wtypesbase.h)</seealso>
internal enum CLSCTX
{
    /// <summary>
    /// The code that creates and manages objects of this class is a DLL that runs in the same process as the caller of the function specifying the class context.
    /// </summary>
    CLSCTX_INPROC_SERVER = 0x1,

    /// <summary>
    /// The EXE code that creates and manages objects of this class runs on same machine but is loaded in a separate process space.
    /// </summary>
    CLSCTX_LOCAL_SERVER = 0x4,
}
