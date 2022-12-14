using System.Runtime.InteropServices;

namespace Shmuelie.Interop.Windows;

internal static unsafe class WinString
{
    [DllImport("combase", ExactSpelling = true)]
    public static extern int WindowsCreateString(ushort* sourceString, uint length, void** @string);

    [DllImport("combase", ExactSpelling = true)]
    public static extern int WindowsDeleteString(void* @string);

    [DllImport("combase", ExactSpelling = true)]
    public static extern int WindowsDuplicateString(void* @string, void** newString);

    [DllImport("combase", ExactSpelling = true)]
    public static extern uint WindowsGetStringLen(void* @string);

    [DllImport("combase", ExactSpelling = true)]
    public static extern ushort* WindowsGetStringRawBuffer(void* @string, uint* length);
}
