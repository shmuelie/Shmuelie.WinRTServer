using System.Windows;
using Shmuelie.WinRTServer.Sample.Proxies;

namespace Shmuelie.WinRTServer.Sample.WpfNetFxClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = MainPageViewModelProxy.Create(Dispatcher);
        }

        public MainPageViewModelProxy ViewModel { get; }
    }
}
