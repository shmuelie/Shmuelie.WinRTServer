using System;
using System.Runtime.InteropServices;

namespace Shmuelie.WinRTServer.Sample.Server;

[Guid("EC6A0FF9-BBF4-48EA-9BFA-DFAF84D4FAF8")]
public sealed partial class Times : ITimes
{
    public DateTimeOffset UtcNow { get; set; }
    public DateTimeOffset LocalNow { get; set; }
    public string? NameAndDescription { get; set; }
}
