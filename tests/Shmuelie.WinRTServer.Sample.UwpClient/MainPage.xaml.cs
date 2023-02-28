using Windows.UI.Xaml.Controls;
using Shmuelie.WinRTServer.Sample.Proxies;

#nullable enable

namespace Shmuelie.WinRTServer.Sample.UwpClient;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
        ViewModel = MainPageViewModelProxy.Create(Dispatcher);
    }

    internal MainPageViewModelProxy ViewModel { get; }
}