using System;
using Windows.Foundation.Metadata;

namespace Shmuelie.WinRTServer.Sample.Interfaces;

[Guid(0x2474f7c0, 0x9db1, 0x4f5f, 0xb6, 0x14, 0xdd, 0x5a, 0x89, 0x5, 0xb, 0x65)]
public interface IRemoteCommand
{
    event EventHandler<object> CanExecuteChanged;

    bool CanExecute(object parameter);

    void Execute(object parameter);
}
