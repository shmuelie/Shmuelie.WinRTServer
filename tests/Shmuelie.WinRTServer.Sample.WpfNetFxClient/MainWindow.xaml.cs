using System;
using System.Windows;
using Shmuelie.WinRTServer.Sample.Interfaces;

namespace Shmuelie.WinRTServer.Sample.WpfNetFxClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly RemoteActivation remoteActivation;

        public MainWindow()
        {
            InitializeComponent();
            remoteActivation = new RemoteActivation();
        }

        private void RemBtn_Click(object sender, RoutedEventArgs e)
        {
            RemResponse.Text = remoteActivation.Rem(5, 4).ToString();
        }

        private async void DelayBtn_Click(object sender, RoutedEventArgs e)
        {
            DelayBtn.IsEnabled = false;
            DelayProg.IsIndeterminate = true;
            await remoteActivation.DelayAsync(3000);
            DelayProg.IsIndeterminate = false;
            DelayBtn.IsEnabled = true;
        }

        private async void LoopBtn_Click(object sender, RoutedEventArgs e)
        {
            LoopBtn.IsEnabled = false;
            await remoteActivation.LoopAsync(100).AsTask(new Progress<LoopProgress>(p =>
            {
                _ = Dispatcher.InvokeAsync(() => LoopProg.Value = p.Count);
            }));
            LoopBtn.IsEnabled = true;
        }

        private async void ListBtn_Click(object sender, RoutedEventArgs e)
        {
            ListBtn.IsEnabled = false;
            var l = await remoteActivation.GenerateListAsync(new ListOptions() { Count = 10, DelayTicks = 1000 }).AsTask(new Progress<ListProgress>(p =>
            {
                _ = Dispatcher.InvokeAsync(() => ListProg.Value = p.Count);
            }));
            ListResp.Text = string.Join(";", l);
            ListBtn.IsEnabled = true;
        }
    }
}
