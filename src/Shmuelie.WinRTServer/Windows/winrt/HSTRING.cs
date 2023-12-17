using Windows.Win32.Foundation;
using Windows.Win32.System.WinRT;
using static Windows.Win32.PInvoke;

namespace Shmuelie.Interop.Windows;

internal static class HSTRING_
{
#if !NETSTANDARD
    [System.Runtime.Versioning.SupportedOSPlatform("windows8.0")]
#endif
    public unsafe static string AsString(this HSTRING @this)
    {
        if (@this.IsNull)
        {
            return string.Empty;
        }

        uint characterCount;
        PCWSTR characters = WindowsGetStringRawBuffer(@this, &characterCount);

        return characters.ToString();
    }
}
