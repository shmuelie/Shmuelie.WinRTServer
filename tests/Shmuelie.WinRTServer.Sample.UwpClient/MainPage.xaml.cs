using System;
using Shmuelie.WinRTServer.Sample.Interfaces;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#nullable enable

namespace Shmuelie.WinRTServer.Sample.UwpClient;

public sealed partial class MainPage : Page
{
    private readonly RemoteActivation _remoteActivation;

    public MainPage()
    {
        this.InitializeComponent();
        _remoteActivation = new RemoteActivation();
    }

    private void RemBtn_Click(object sender, RoutedEventArgs e)
    {
        RemResponse.Text = _remoteActivation.Rem(5, 4).ToString();
    }

    private async void DelayBtn_Click(object sender, RoutedEventArgs e)
    {
        DelayBtn.IsEnabled = false;
        DelayProg.IsIndeterminate = true;
        await _remoteActivation.DelayAsync(3000);
        DelayProg.IsIndeterminate = false;
        DelayBtn.IsEnabled = true;
    }

    private async void LoopBtn_Click(object sender, RoutedEventArgs e)
    {
        LoopBtn.IsEnabled = false;
        await _remoteActivation.LoopAsync(100).AsTask(new Progress<LoopProgress>(p =>
        {
            _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => LoopProg.Value = p.Count);
        }));
        LoopBtn.IsEnabled = true;
    }

    private async void ListBtn_Click(object sender, RoutedEventArgs e)
    {
        ListBtn.IsEnabled = false;
        var l = await _remoteActivation.GenerateListAsync(new ListOptions() { Count = 10, DelayTicks = 1000 }).AsTask(new Progress<ListProgress>(p =>
        {
            _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => ListProg.Value = p.Count);
        }));
        ListResp.Text = string.Join(";", l);
        ListBtn.IsEnabled = true;
    }
}
