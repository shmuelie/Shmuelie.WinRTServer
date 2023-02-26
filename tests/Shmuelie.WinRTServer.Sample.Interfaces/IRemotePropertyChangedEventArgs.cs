using System;
using Windows.Foundation.Metadata;

namespace Shmuelie.WinRTServer.Sample.Interfaces;

[Guid(0xec5ac7d6, 0xfa4a, 0x40c4, 0xb2, 0x6, 0x97, 0x39, 0x7, 0xaf, 0x3a, 0x1c)]
public interface IRemotePropertyChangedEventArgs
{
    string PropertyName { get; }
}
