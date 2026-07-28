namespace Shmuelie.WinRTServer;

/// <summary>
/// The authentication level for COM calls, mirroring the RPC <c>RPC_C_AUTHN_LEVEL</c> constants.
/// </summary>
public enum ServerAuthenticationLevel
{
    /// <summary>Use the default authentication level negotiated by COM.</summary>
    Default = 0,

    /// <summary>No authentication.</summary>
    None = 1,

    /// <summary>Authenticate only when the connection is established.</summary>
    Connect = 2,

    /// <summary>Authenticate at the beginning of each remote procedure call.</summary>
    Call = 3,

    /// <summary>Authenticate every packet.</summary>
    Packet = 4,

    /// <summary>Authenticate every packet and verify none were modified in transit.</summary>
    PacketIntegrity = 5,

    /// <summary>Authenticate every packet and encrypt the arguments.</summary>
    PacketPrivacy = 6,
}

/// <summary>
/// The impersonation level for COM calls, mirroring the RPC <c>RPC_C_IMP_LEVEL</c> constants.
/// </summary>
public enum ServerImpersonationLevel
{
    /// <summary>Use the default impersonation level negotiated by COM.</summary>
    Default = 0,

    /// <summary>The client is anonymous; the server cannot obtain identification information.</summary>
    Anonymous = 1,

    /// <summary>The server can obtain the client's identity but cannot impersonate it.</summary>
    Identify = 2,

    /// <summary>The server can impersonate the client on the local system.</summary>
    Impersonate = 3,

    /// <summary>The server can impersonate the client on remote systems.</summary>
    Delegate = 4,
}
