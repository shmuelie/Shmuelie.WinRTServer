using System;
#if NETSTANDARD2_0
using static Shmuelie.WinRTServer.UIntPtrExtensions;
#endif

namespace Shmuelie.Interop.Windows;

/// <summary>
/// Represents activation factories that are registered by calling the RoRegisterActivationFactories function.
/// </summary>
/// <seealso href="https://learn.microsoft.com/en-us/windows/win32/winrt/ro-registration-cookie">RO_REGISTRATION_COOKIE</seealso>
/// <remarks>
/// Initializes a new instance of the <see cref="RO_REGISTRATION_COOKIE"/> struct.
/// </remarks>
/// <param name="value"></param>
internal readonly unsafe partial struct RO_REGISTRATION_COOKIE(void* value) : IComparable, IComparable<RO_REGISTRATION_COOKIE>, IEquatable<RO_REGISTRATION_COOKIE>, IFormattable
{
    private readonly void* Value = value;

    /// <summary>
    /// Gets a null <see cref="RO_REGISTRATION_COOKIE"/>.
    /// </summary>
    public static RO_REGISTRATION_COOKIE NULL => new RO_REGISTRATION_COOKIE(null);

    public static bool operator ==(RO_REGISTRATION_COOKIE left, RO_REGISTRATION_COOKIE right) => left.Value == right.Value;

    public static bool operator !=(RO_REGISTRATION_COOKIE left, RO_REGISTRATION_COOKIE right) => left.Value != right.Value;

    public static bool operator <(RO_REGISTRATION_COOKIE left, RO_REGISTRATION_COOKIE right) => left.Value < right.Value;

    public static bool operator <=(RO_REGISTRATION_COOKIE left, RO_REGISTRATION_COOKIE right) => left.Value <= right.Value;

    public static bool operator >(RO_REGISTRATION_COOKIE left, RO_REGISTRATION_COOKIE right) => left.Value > right.Value;

    public static bool operator >=(RO_REGISTRATION_COOKIE left, RO_REGISTRATION_COOKIE right) => left.Value >= right.Value;

    public static explicit operator RO_REGISTRATION_COOKIE(void* value) => new RO_REGISTRATION_COOKIE(value);

    public static implicit operator void*(RO_REGISTRATION_COOKIE value) => value.Value;

    public static explicit operator RO_REGISTRATION_COOKIE(byte value) => new RO_REGISTRATION_COOKIE(unchecked((void*)(value)));

    public static explicit operator byte(RO_REGISTRATION_COOKIE value) => (byte)(value.Value);

    public static explicit operator RO_REGISTRATION_COOKIE(short value) => new RO_REGISTRATION_COOKIE(unchecked((void*)(value)));

    public static explicit operator short(RO_REGISTRATION_COOKIE value) => (short)(value.Value);

    public static explicit operator RO_REGISTRATION_COOKIE(int value) => new RO_REGISTRATION_COOKIE(unchecked((void*)(value)));

    public static explicit operator int(RO_REGISTRATION_COOKIE value) => (int)(value.Value);

    public static explicit operator RO_REGISTRATION_COOKIE(long value) => new RO_REGISTRATION_COOKIE(unchecked((void*)(value)));

    public static explicit operator long(RO_REGISTRATION_COOKIE value) => (long)(value.Value);

    public static explicit operator RO_REGISTRATION_COOKIE(nint value) => new RO_REGISTRATION_COOKIE(unchecked((void*)(value)));

    public static implicit operator nint(RO_REGISTRATION_COOKIE value) => (nint)(value.Value);

    public static explicit operator RO_REGISTRATION_COOKIE(sbyte value) => new RO_REGISTRATION_COOKIE(unchecked((void*)(value)));

    public static explicit operator sbyte(RO_REGISTRATION_COOKIE value) => (sbyte)(value.Value);

    public static explicit operator RO_REGISTRATION_COOKIE(ushort value) => new RO_REGISTRATION_COOKIE(unchecked((void*)(value)));

    public static explicit operator ushort(RO_REGISTRATION_COOKIE value) => (ushort)(value.Value);

    public static explicit operator RO_REGISTRATION_COOKIE(uint value) => new RO_REGISTRATION_COOKIE(unchecked((void*)(value)));

    public static explicit operator uint(RO_REGISTRATION_COOKIE value) => (uint)(value.Value);

    public static explicit operator RO_REGISTRATION_COOKIE(ulong value) => new RO_REGISTRATION_COOKIE(unchecked((void*)(value)));

    public static explicit operator ulong(RO_REGISTRATION_COOKIE value) => (ulong)(value.Value);

    public static explicit operator RO_REGISTRATION_COOKIE(nuint value) => new RO_REGISTRATION_COOKIE(unchecked((void*)(value)));

    public static implicit operator nuint(RO_REGISTRATION_COOKIE value) => (nuint)(value.Value);

    /// <inheritdoc/>
    public int CompareTo(object? obj)
    {
        if (obj is RO_REGISTRATION_COOKIE other)
        {
            return CompareTo(other);
        }

        return (obj is null) ? 1 : throw new ArgumentException("obj is not an instance of RO_REGISTRATION_COOKIE.");
    }

    /// <inheritdoc/>
    public int CompareTo(RO_REGISTRATION_COOKIE other) => ((nuint)(Value)).CompareTo((nuint)(other.Value));

    /// <inheritdoc/>
    public override bool Equals(object? obj) => (obj is RO_REGISTRATION_COOKIE other) && Equals(other);

    /// <inheritdoc/>
    public bool Equals(RO_REGISTRATION_COOKIE other) => ((nuint)(Value)).Equals((nuint)(other.Value));

    /// <inheritdoc/>
    public override int GetHashCode() => ((nuint)(Value)).GetHashCode();

    /// <inheritdoc/>
    public override string ToString() => ((nuint)(Value)).ToString((sizeof(nint) == 4) ? "X8" : "X16");

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider) => ((nuint)(Value)).ToString(format, formatProvider);
}