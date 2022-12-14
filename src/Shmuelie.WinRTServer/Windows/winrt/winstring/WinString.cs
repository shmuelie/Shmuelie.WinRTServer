using System.Runtime.InteropServices;

namespace Shmuelie.Interop.Windows;

internal static unsafe class WinString
{
    [DllImport("combase", ExactSpelling = true)]
    public static extern int WindowsCreateString(ushort* sourceString, uint length, HSTRING* @string);

    [DllImport("combase", ExactSpelling = true)]
    public static extern int WindowsDeleteString(HSTRING @string);

    [DllImport("combase", ExactSpelling = true)]
    public static extern int WindowsDuplicateString(HSTRING @string, HSTRING* newString);

    [DllImport("combase", ExactSpelling = true)]
    public static extern uint WindowsGetStringLen(HSTRING @string);

    [DllImport("combase", ExactSpelling = true)]
    public static extern ushort* WindowsGetStringRawBuffer(HSTRING @string, uint* length);
}
