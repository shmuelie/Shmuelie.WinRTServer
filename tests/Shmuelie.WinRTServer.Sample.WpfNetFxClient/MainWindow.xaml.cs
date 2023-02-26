using System.Windows;

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
            DataContext = MainPageViewModelProxy.Create(Dispatcher);
        }
    }
}
