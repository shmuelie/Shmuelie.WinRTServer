using System.ComponentModel;
using Shmuelie.WinRTServer.Sample.Interfaces;
using Windows.UI.Core;

#nullable enable

namespace Shmuelie.WinRTServer.Sample.UwpClient;

internal abstract class RemotePropertyChangedAdapter : INotifyPropertyChanged
{
    private readonly CoreDispatcher dispatcher;

    public RemotePropertyChangedAdapter(IRemotePropertyChanged remotePropertyChanged, CoreDispatcher dispatcher)
    {
        remotePropertyChanged.PropertyChanged += RemotePropertyChanged_PropertyChanged;
        this.dispatcher = dispatcher;
    }

    private void RemotePropertyChanged_PropertyChanged(object sender, IRemotePropertyChangedEventArgs e)
    {
        _ = dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e.PropertyName)));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
