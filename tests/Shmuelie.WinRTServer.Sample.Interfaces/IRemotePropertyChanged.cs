using System;
using Windows.Foundation.Metadata;

namespace Shmuelie.WinRTServer.Sample.Interfaces;

[Guid(0x440c69bf, 0x6760, 0x43e5, 0x9c, 0xe8, 0xc1, 0x94, 0xa1, 0x49, 0xfd, 0xc)]
public interface IRemotePropertyChanged
{
    event EventHandler<IRemotePropertyChangedEventArgs> PropertyChanged;
}
