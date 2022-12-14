using System;

namespace Shmuelie.WinRTServer;

#if NETSTANDARD2_0
internal static class UIntPtrExtensions
{
    public static int CompareTo(this nuint @this, nuint other)
    {
        return ((UIntPtr)@this).ToUInt64().CompareTo(((UIntPtr)other).ToUInt64());
    }

    public static string ToString(this nuint @this, string? format)
    {
        return ((UIntPtr)@this).ToUInt64().ToString(format);
    }

    public static string ToString(this nuint @this, string? format, IFormatProvider? formatProvider)
    {
        return ((UIntPtr)@this).ToUInt64().ToString(format, formatProvider);
    }
}
#endif