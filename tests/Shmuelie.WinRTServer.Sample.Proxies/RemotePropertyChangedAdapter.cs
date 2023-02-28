using System.ComponentModel;
using Shmuelie.WinRTServer.Sample.Interfaces;

namespace Shmuelie.WinRTServer.Sample.Proxies;

public abstract class RemotePropertyChangedAdapter : INotifyPropertyChanged
{
    private readonly
#if UAP10_0
        Windows.UI.Core.CoreDispatcher
#else
        System.Windows.Threading.Dispatcher
#endif
        ? dispatcher;

    public RemotePropertyChangedAdapter(IRemotePropertyChanged remotePropertyChanged,
#if UAP10_0
        Windows.UI.Core.CoreDispatcher
#else
        System.Windows.Threading.Dispatcher
#endif
        dispatcher)
    {
        remotePropertyChanged.PropertyChanged += RemotePropertyChanged_PropertyChanged;
        this.dispatcher = dispatcher;
    }

    private void RemotePropertyChanged_PropertyChanged(object? sender, IRemotePropertyChangedEventArgs e)
    {
        _ =
#if UAP10_0
            dispatcher?.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
#else
            dispatcher?.InvokeAsync(
#endif
            () => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e.PropertyName)));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
