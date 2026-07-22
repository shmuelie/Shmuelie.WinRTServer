using System;

namespace Shmuelie.WinRTServer;

/// <summary>
/// Process-global runtime settings applied to a server, corresponding to the <c>COMGLB_RO_SETTINGS</c>
/// property of <c>IGlobalOptions</c>.
/// </summary>
/// <remarks>These map to the <c>GLOBALOPT_RO_FLAGS</c> values documented for <c>IGlobalOptions</c>.</remarks>
[Flags]
public enum ServerRuntimeOptions
{
    /// <summary>
    /// No runtime options.
    /// </summary>
    None = 0,

    /// <summary>
    /// Remove touch messages from the message queue in an STA modal loop.
    /// </summary>
    StaModalLoopRemoveTouchMessages = 0x1,

    /// <summary>
    /// Remove input messages from the shared queue in an STA modal loop.
    /// </summary>
    StaModalLoopSharedQueueRemoveInputMessages = 0x2,

    /// <summary>
    /// Do not remove input messages from the shared queue in an STA modal loop.
    /// </summary>
    StaModalLoopSharedQueueDoNotRemoveInputMessages = 0x4,

    /// <summary>
    /// Enable fast rundown of stub objects when the server process exits.
    /// </summary>
    FastRundown = 0x8,

    /// <summary>
    /// Reorder pointer messages in the shared queue in an STA modal loop.
    /// </summary>
    StaModalLoopSharedQueueReorderPointerMessages = 0x80,
}

/// <summary>
/// Configuration for a <see cref="ComServer"/> or <see cref="WinRtServer"/>.
/// </summary>
public sealed class ServerOptions
{
    /// <summary>
    /// Gets or sets the process-global runtime options applied via <c>IGlobalOptions</c>.
    /// </summary>
    /// <remarks>Defaults to <see cref="ServerRuntimeOptions.FastRundown"/>, matching the historical behavior.</remarks>
    public ServerRuntimeOptions RuntimeOptions
    {
        get;
        set;
    } = ServerRuntimeOptions.FastRundown;
}
