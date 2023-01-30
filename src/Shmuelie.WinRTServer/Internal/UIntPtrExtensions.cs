using System;

namespace Shmuelie.WinRTServer;

#if NETSTANDARD2_0
/// <summary>
/// Implements methods for <see cref="UIntPtr"/> missing in NS2.0.
/// </summary>
internal static class UIntPtrExtensions
{
    /// <summary>
    /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
    /// </summary>
    /// <param name="this"></param>
    /// <param name="value">An unsigned native integer to compare.</param>
    /// <returns>
    /// <para>A value that indicates the relative order of the objects being compared. The return value has these meanings:</para>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Value</term>
    ///         <term>Meaning</term>
    ///     </listheader>
    ///     <item>
    ///         <term>Less than zero</term>
    ///         <term>This instance precedes <paramref name="value"/> in the sort order.</term>
    ///     </item>
    ///     <item>
    ///         <term>Zero</term>
    ///         <term>This instance occurs in the same position in the sort order as <paramref name="value"/>.</term>
    ///     </item>
    ///     <item>
    ///         <term>Greater than zero</term>
    ///         <term>This instance follows <paramref name="value"/> in the sort order.</term>
    ///     </item>
    /// </list>
    /// </returns>
    public static int CompareTo(this nuint @this, nuint value)
    {
        return ((UIntPtr)@this).ToUInt64().CompareTo(((UIntPtr)value).ToUInt64());
    }

    /// <summary>
    /// Converts the numeric value of this instance to its equivalent string representation, using the specified format.
    /// </summary>
    /// <param name="this"></param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <returns>The string representation of the value of this instance as specified by <paramref name="format"/>.</returns>
    /// <exception cref="FormatException"><paramref name="format"/> is invalid or not supported.</exception>
    public static string ToString(this nuint @this, string? format)
    {
        return ((UIntPtr)@this).ToUInt64().ToString(format);
    }

    /// <summary>
    /// Formats the value of the current instance using the specified format.
    /// </summary>
    /// <param name="this"></param>
    /// <param name="format">The format to use. -or- A <see langword="null"/> reference to use the default format defined for the type of the <see cref="IFormattable"/> implementation.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <returns>The value of the current instance in the specified format.</returns>
    /// <exception cref="FormatException"><paramref name="format"/> is invalid or not supported.</exception>
    public static string ToString(this nuint @this, string? format, IFormatProvider? provider)
    {
        return ((UIntPtr)@this).ToUInt64().ToString(format, provider);
    }
}
#endif