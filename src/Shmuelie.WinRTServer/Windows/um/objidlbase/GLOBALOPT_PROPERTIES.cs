namespace Shmuelie.Interop.Windows;

/// <summary>
/// Identifies process-global options that you can set or query by using the <see cref="IGlobalOptions" /> interface.
/// </summary>
/// <seealso cref="IGlobalOptions"/>
internal enum GLOBALOPT_PROPERTIES
{
    /// <summary>
    /// Defines COM exception-handling behavior.
    /// </summary>
    COMGLB_EXCEPTION_HANDLING = 1,

    /// <summary>
    /// Sets the AppID for the process.
    /// </summary>
    COMGLB_APPID = 2,

    /// <summary>
    /// Sets the thread-pool behavior of the RPC runtime in the process.
    /// </summary>
    COMGLB_RPC_THREADPOOL_SETTING = 3,

    /// <summary>
    /// Used for miscellaneous settings.
    /// </summary>
    COMGLB_RO_SETTINGS = 4,

    /// <summary>
    /// Defines the policy that's applied in the <c>CoUnmarshalInterface</c> function.
    /// </summary>
    COMGLB_UNMARSHALING_POLICY = 5,

    COMGLB_PROPERTIES_RESERVED1 = 6,

    COMGLB_PROPERTIES_RESERVED2 = 7,

    COMGLB_PROPERTIES_RESERVED3 = 8,
}
