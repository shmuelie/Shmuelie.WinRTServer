namespace Shmuelie.Interop.Windows;

/// <summary>
/// Represents the trust level of an activatable class.
/// </summary>
/// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/inspectable/ne-inspectable-trustlevel">TrustLevel enumeration (inspectable.h)</seealso>
internal enum TrustLevel
{
    /// <summary>
    /// The component has access to resources that are not protected.
    /// </summary>
    BaseTrust = 0,
}
