using System.Runtime.InteropServices;

namespace Shmuelie.Interop.Windows;

/// <summary>
/// APIs from the <c>winstring</c> header.
/// </summary>
internal static unsafe class WinString
{
    /// <summary>
    /// Creates a new <see cref="HSTRING"/> based on the specified source string.
    /// </summary>
    /// <param name="sourceString">A null-terminated string to use as the source for the new <see cref="HSTRING"/>. To create a new, empty, or <see langword="null"/> string, pass <see langword="null"/> for <paramref name="sourceString"/> and 0 for <paramref name="length"/>.</param>
    /// <param name="length">The length of <paramref name="sourceString"/>, in Unicode characters. Must be 0 if <paramref name="sourceString"/> is <see langword="null"/>.</param>
    /// <param name="string">A pointer to the newly created <see cref="HSTRING"/>, or <see langword="null"/> if an error occurs. Any existing content in <paramref name="string"/> is overwritten. The <see cref="HSTRING"/> is a standard handle type.</param>
    /// <returns>
    /// <para>This function can return one of these values.</para>
    /// <list type="table">
    ///     <listheader>
    ///         <description>Return code</description>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <description><c>S_OK</c></description>
    ///         <description>The <see cref="HSTRING"/> was created successfully.</description>
    ///     </item>
    ///     <item>
    ///         <description><c>E_INVALIDARG</c></description>
    ///         <description><paramref name="string"/> is <see langword="null"/>.</description>
    ///     </item>
    ///     <item>
    ///         <description><c>E_OUTOFMEMORY</c></description>
    ///         <description>Failed to allocate the new <see cref="HSTRING"/>.</description>
    ///     </item>
    ///     <item>
    ///         <description><c>E_POINTER</c></description>
    ///         <description><paramref name="sourceString"/> is <see langword="null"/> and <paramref name="length"/> is non-zero.</description>
    ///     </item>
    /// </list>
    /// </returns>
    /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/winstring/nf-winstring-windowscreatestring">WindowsCreateString function (winstring.h)</seealso>
    [DllImport("combase", ExactSpelling = true)]
    public static extern int WindowsCreateString(ushort* sourceString, uint length, HSTRING* @string);

    /// <summary>
    /// Decrements the reference count of a string buffer.
    /// </summary>
    /// <param name="string">The string to be deleted. If <paramref name="string"/> is a fast-pass string created by <c>WindowsCreateStringReference</c>, or if <paramref name="string"/> is <see langword="null"/> or empty, no action is taken and <c>S_OK</c> is returned.</param>
    /// <returns>This function always returns <c>S_OK</c>.</returns>
    /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/winstring/nf-winstring-windowsdeletestring">WindowsDeleteString function (winstring.h)</seealso>
    [DllImport("combase", ExactSpelling = true)]
    public static extern int WindowsDeleteString(HSTRING @string);

    /// <summary>
    /// Retrieves the backing buffer for the specified string.
    /// </summary>
    /// <param name="string">An optional string for which the backing buffer is to be retrieved. Can be <see langword="null"/>.</param>
    /// <param name="length">
    /// An optional pointer to a <see cref="uint"/>. If <see langword="null"/>
    /// is passed for <paramref name="length"/>, then it is ignored. If
    /// <paramref name="length"/> is a valid pointer to a <see cref="uint"/>,
    /// and string is a valid <see cref="HSTRING"/>, then on successful
    /// completion the function sets the value pointed to by <paramref
    /// name="length"/> to the number of Unicode characters in the backing
    /// buffer for <paramref name="string"/> (including embedded null
    /// characters, but excluding the terminating null). If <paramref
    /// name="length"/> is a valid pointer to a <see cref="uint"/>, and
    /// <paramref name="string"/> is <see langword="null"/>, then the value
    /// pointed to by <paramref name="length"/> is set to 0.
    /// </param>
    /// <returns>A pointer to the buffer that provides the backing store for <paramref name="string"/>, or the empty string if <paramref name="string"/> is <see langword="null"/> or the empty string.</returns>
    /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/winstring/nf-winstring-windowsgetstringrawbuffer">WindowsGetStringRawBuffer function (winstring.h)</seealso>
    [DllImport("combase", ExactSpelling = true)]
    public static extern ushort* WindowsGetStringRawBuffer(HSTRING @string, uint* length);

    /// <summary>
    /// Compares two specified <see cref="HSTRING"/> objects and returns an integer that indicates their relative position in a sort order.
    /// </summary>
    /// <param name="string1">The first string to compare.</param>
    /// <param name="string2">The second string to compare.</param>
    /// <param name="result">A value that indicates the lexical relationship between <paramref name="string1"/> and <paramref name="string2"/>.</param>
    /// <returns>
    /// <para>This function can return one of these values.</para>
    /// <list type="table">
    ///     <listheader>
    ///         <description>Return code</description>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <description><c>S_OK</c></description>
    ///         <description>The comparison was successful.</description>
    ///     </item>
    ///     <item>
    ///         <description><c>E_INVALIDARG</c></description>
    ///         <description><paramref name="result"/> is <see langword="null"/>.</description>
    ///     </item>
    /// </list>
    /// </returns>
    /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/winstring/nf-winstring-windowscomparestringordinal">WindowsCompareStringOrdinal function (winstring.h)</seealso>
    [DllImport("combase", ExactSpelling = true)]
    public static extern int WindowsCompareStringOrdinal(HSTRING string1, HSTRING string2, int* result);
}
