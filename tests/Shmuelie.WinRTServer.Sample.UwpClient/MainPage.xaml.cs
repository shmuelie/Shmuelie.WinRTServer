using System;
using System.Runtime.InteropServices;
using Shmuelie.WinRTServer.Sample.Interfaces;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#nullable enable

namespace Shmuelie.WinRTServer.Sample.UwpClient;

public sealed partial class MainPage : Page
{
    private readonly RemoteThing _remoteThing;

    public MainPage()
    {
        this.InitializeComponent();
        _remoteThing = CreateRemoteThing();
        _remoteThing.LoopCompleted += _remoteThing_LoopCompleted;
        DateTimeUtcBtn.Content = _remoteThing.NowUtc.ToString();
    }

    private void _remoteThing_LoopCompleted(IRemoteThing sender, object args)
    {
        _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => LoopProg.Value = 0);
    }

    private static unsafe RemoteThing CreateRemoteThing()
    {
        Guid classId = Guid.Parse("4F59AF92-A98D-4A20-8C8D-1C076647A6AE");
        Guid iid = new Guid(0x2474f7c0, 0x9db1, 0x4f4f, 0xb6, 0x14, 0xdc, 0x5a, 0x89, 0x5, 0xb, 0x64);
        uint hresult = CoCreateRemoteThingInstance(&classId, null, 0x4U, &iid, out RemoteThing remoteThing);
        Marshal.ThrowExceptionForHR((int)hresult);
        return remoteThing;
    }

    [DllImport("ole32", EntryPoint = "CoCreateInstance", ExactSpelling = true)]
    private unsafe static extern uint CoCreateRemoteThingInstance(Guid* rclsid, void* pUnkOuter, uint dwClsContext, Guid* riid, out RemoteThing ppv);

    private void RemBtn_Click(object sender, RoutedEventArgs e)
    {
        RemResponse.Text = _remoteThing.Rem(5, 4).ToString();
    }

    private async void DelayBtn_Click(object sender, RoutedEventArgs e)
    {
        DelayBtn.IsEnabled = false;
        DelayProg.IsIndeterminate = true;
        await _remoteThing.DelayAsync(3000);
        DelayProg.IsIndeterminate = false;
        DelayBtn.IsEnabled = true;
    }

    private async void LoopBtn_Click(object sender, RoutedEventArgs e)
    {
        LoopBtn.IsEnabled = false;
        await _remoteThing.LoopAsync(100).AsTask(new Progress<LoopProgress>(p =>
        {
            _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => LoopProg.Value = p.Count);
        }));
        LoopBtn.IsEnabled = true;
    }

    private async void ListBtn_Click(object sender, RoutedEventArgs e)
    {
        ListBtn.IsEnabled = false;
        var l = await _remoteThing.GenerateListAsync(new ListOptions() { Count = 10, DelayTicks = 1000 }).AsTask(new Progress<ListProgress>(p =>
        {
            _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => ListProg.Value = p.Count);
        }));
        ListResp.Text = string.Join(";", l);
        ListBtn.IsEnabled = true;
    }

    private void DateTimeUtcBtn_Click(object sender, RoutedEventArgs e)
    {
        DateTimeUtcBtn.Content = _remoteThing.NowUtc.ToString();
    }
}