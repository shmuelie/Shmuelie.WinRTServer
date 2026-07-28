using System;
using System.Runtime.Versioning;
using static Windows.Win32.PInvoke;

namespace Shmuelie.WinRTServer;

/// <summary>
/// A scope that impersonates the current COM caller and reverts to the server identity when disposed.
/// </summary>
/// <seealso cref="ServerSecurity.ImpersonateClient"/>
[SupportedOSPlatform("windows6.0.6000")]
public sealed class ClientImpersonation : IDisposable
{
    private bool reverted;

    internal ClientImpersonation()
    {
    }

    /// <summary>
    /// Reverts to the server's own identity.
    /// </summary>
    public void Dispose()
    {
        if (reverted)
        {
            return;
        }

        reverted = true;
        _ = CoRevertToSelf();
    }
}
