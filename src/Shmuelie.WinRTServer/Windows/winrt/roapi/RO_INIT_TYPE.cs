namespace Shmuelie.Interop.Windows;

/// <summary>
/// Determines the concurrency model used for incoming calls to the objects created by this thread.
/// </summary>
/// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/roapi/ne-roapi-ro_init_type">RO_INIT_TYPE enumeration (roapi.h)</seealso>
internal enum RO_INIT_TYPE
{
    /// <summary>
    /// Initializes the thread for multi-threaded concurrency. The current thread is initialized in the MTA.
    /// </summary>
    RO_INIT_MULTITHREADED = 1
}
