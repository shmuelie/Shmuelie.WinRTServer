using Windows.UI.Xaml.Controls;

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