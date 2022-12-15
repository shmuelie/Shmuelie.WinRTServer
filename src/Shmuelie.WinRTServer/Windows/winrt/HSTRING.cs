using System;
#if NETSTANDARD2_0
using static Shmuelie.WinRTServer.UIntPtrExtensions;
#endif

namespace Shmuelie.Interop.Windows;

/// <summary>
/// A handle to a Windows Runtime string.
/// </summary>
internal readonly unsafe partial struct HSTRING : IComparable, IComparable<HSTRING>, IEquatable<HSTRING>, IFormattable
{
    public readonly void* Value;

    public HSTRING(void* value)
    {
        Value = value;
    }

    public static HSTRING INVALID_VALUE => new HSTRING((void*)(-1));

    public static HSTRING NULL => new HSTRING(null);

    public static bool operator ==(HSTRING left, HSTRING right) => left.Equals(right);

    public static bool operator !=(HSTRING left, HSTRING right) => !left.Equals(right);

    /// <inheritdoc/>
    public int CompareTo(object? obj)
    {
        if (obj is HSTRING other)
        {
            return CompareTo(other);
        }

        return (obj is null) ? 1 : throw new ArgumentException("obj is not an instance of HSTRING.");
    }

    /// <inheritdoc/>
    public int CompareTo(HSTRING other)
    {
        int result;
        WinString.WindowsCompareStringOrdinal(this, other, &result);
        return result;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => (obj is HSTRING other) && Equals(other);

    /// <inheritdoc/>
    public bool Equals(HSTRING other) => CompareTo(other) == 0;

    /// <inheritdoc/>
    public override int GetHashCode() => ((nuint)(Value)).GetHashCode();

    /// <inheritdoc/>
    public override string ToString()
    {
        if (Value is null)
        {
            return string.Empty;
        }

        uint characterCount;
        ushort* characters = WinString.WindowsGetStringRawBuffer(this, &characterCount);

        if (characters is null || characterCount == 0)
        {
            return string.Empty;
        }

        return new string((char*)characters, 0, (int)characterCount);
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (format is null)
        {
            return ToString();
        }

        if (format.Equals("x", StringComparison.OrdinalIgnoreCase))
        {
            return ((nuint)(Value)).ToString(format, formatProvider);
        }

        throw new FormatException("Unsupported format");
    }
}
