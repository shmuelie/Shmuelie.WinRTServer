using System;
using System.ComponentModel;
using System.Windows.Threading;
using Shmuelie.WinRTServer.Sample.Interfaces;

namespace Shmuelie.WinRTServer.Sample.WpfNetFxClient;

[Serializable]
internal abstract class RemotePropertyChangedAdapter : INotifyPropertyChanged
{
    [NonSerialized]
    private readonly Dispatcher dispatcher;

    public RemotePropertyChangedAdapter(IRemotePropertyChanged remotePropertyChanged, Dispatcher dispatcher)
    {
        remotePropertyChanged.PropertyChanged += RemotePropertyChanged_PropertyChanged;
        this.dispatcher = dispatcher;
    }

    private void RemotePropertyChanged_PropertyChanged(object sender, IRemotePropertyChangedEventArgs e)
    {
        _ = dispatcher.InvokeAsync(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e.PropertyName)));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
