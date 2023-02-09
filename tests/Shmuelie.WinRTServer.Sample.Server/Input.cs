using System;
using System.Runtime.InteropServices;
using Shmuelie.WinRTServer.Sample.Interfaces;

namespace Shmuelie.WinRTServer.Sample;

[Guid("4F59AF92-A98D-4A20-8C8D-1C076647A6B0")]
public sealed class Input : IInput
{
    public string Name { get; set; }
    public string Description { get; set; }
}
