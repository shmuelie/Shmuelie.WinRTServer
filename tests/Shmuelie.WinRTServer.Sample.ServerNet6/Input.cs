using System;
using System.Runtime.InteropServices;
using Shmuelie.WinRTServer.Sample.Interfaces;

namespace Shmuelie.WinRTServer.Sample;

[Guid("2F8EDC40-C145-4128-B451-559D45CD9074")]
public sealed class Input : IInput
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}
