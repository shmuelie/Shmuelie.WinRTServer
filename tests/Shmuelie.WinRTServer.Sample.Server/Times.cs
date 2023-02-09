using System;
using System.Runtime.InteropServices;
using Shmuelie.WinRTServer.Sample.Interfaces;

namespace Shmuelie.WinRTServer.Sample;

[Guid("4F59AF92-A98D-4A20-8C8D-1C076647A6AF")]
public sealed class Times : ITimes
{
    public DateTimeOffset UtcNow { get; set; }
    public DateTimeOffset LocalNow { get; set; }
    public string NameAndDescription { get; set; }
}
