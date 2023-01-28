namespace Shmuelie.Interop.Windows;

/// <summary>
/// See <see href="https://docs.microsoft.com/windows/win32/seccrypto/common-hresult-values"/>.
/// </summary>
internal static partial class E
{
    /// <summary>
    /// 	No such interface supported
    /// </summary>
    public const int E_NOINTERFACE = unchecked((int)(0x80004002));

    /// <summary>
    /// Handle that is not valid
    /// </summary>
    public const int E_HANDLE = unchecked((int)(0x80070006));

    /// <summary>
    /// Unexpected failure
    /// </summary>
    public const int E_UNEXPECTED = unchecked((int)(0x8000FFFF));
}