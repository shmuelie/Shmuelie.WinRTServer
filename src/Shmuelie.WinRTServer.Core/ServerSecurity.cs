using System;
using System.Runtime.Versioning;
using Windows.Win32.Security;
using Windows.Win32.System.Com;
using static Windows.Win32.PInvoke;

namespace Shmuelie.WinRTServer;

/// <summary>
/// Helpers for configuring COM security for the server process and for understanding the caller of an incoming
/// COM call.
/// </summary>
[SupportedOSPlatform("windows6.0.6000")]
public static class ServerSecurity
{
    /// <summary>
    /// Initializes process-wide COM security.
    /// </summary>
    /// <param name="authenticationLevel">The default authentication level for the process.</param>
    /// <param name="impersonationLevel">The default impersonation level for proxies.</param>
    /// <remarks>
    /// <para>Wraps <c>CoInitializeSecurity</c> with a <see langword="null"/> security descriptor, which registers
    /// the process without an access-control restriction. This must be called once, before any interface is
    /// marshaled.</para>
    /// </remarks>
    /// <exception cref="System.ComponentModel.Win32Exception">The call to <c>CoInitializeSecurity</c> failed.</exception>
    public static unsafe void Initialize(ServerAuthenticationLevel authenticationLevel = ServerAuthenticationLevel.Default, ServerImpersonationLevel impersonationLevel = ServerImpersonationLevel.Identify)
    {
        CoInitializeSecurity(new PSECURITY_DESCRIPTOR(null), -1, null, null, (RPC_C_AUTHN_LEVEL)authenticationLevel, (RPC_C_IMP_LEVEL)impersonationLevel, null, EOLE_AUTHENTICATION_CAPABILITIES.EOAC_NONE, null).ThrowOnFailure();
    }

    /// <summary>
    /// Impersonates the client of the current COM call until the returned scope is disposed.
    /// </summary>
    /// <returns>A scope that reverts to the server's own identity when disposed.</returns>
    /// <remarks>Only valid while servicing an incoming COM call. Wraps <c>CoImpersonateClient</c> /
    /// <c>CoRevertToSelf</c>.</remarks>
    /// <exception cref="System.ComponentModel.Win32Exception">The call to <c>CoImpersonateClient</c> failed.</exception>
    public static ClientImpersonation ImpersonateClient()
    {
        CoImpersonateClient().ThrowOnFailure();
        return new ClientImpersonation();
    }
}
