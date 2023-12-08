using System;
using System.Runtime.InteropServices;
using Shmuelie.WinRTServer.Sample.Interfaces;

namespace Shmuelie.WinRTServer.Sample;

[Guid("EC6A0FF9-BBF4-48EA-9BFA-DFAF84D4FAF8")]
public sealed class Times : ITimes
{
    public DateTimeOffset UtcNow { get; set; }
    public DateTimeOffset LocalNow { get; set; }
    public string? NameAndDescription { get; set; }
}
