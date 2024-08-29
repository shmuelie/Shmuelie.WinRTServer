using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Shmuelie.WinRTServer.Sample.Interfaces;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#nullable enable

namespace Shmuelie.WinRTServer.Sample.UwpClient;

public sealed partial class MainPage : Page
{
    private readonly RemoteThing remoteThing;

    public MainPage()
    {
        this.InitializeComponent();
        remoteThing = CreateRemoteThing();
        remoteThing.LoopCompleted += RemoteThing_LoopCompleted;
        DateTimeUtcBtn.Content = remoteThing.NowUtc.ToString();
    }

    private void RemoteThing_LoopCompleted(IRemoteThing sender, object args)
    {
        _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => LoopProg.Value = 0);
    }

    private static unsafe RemoteThing CreateRemoteThing()
    {
        Guid classId = Guid.Parse("474527DE-81CD-466E-ADCF-6E3809CD5033");
        Guid iid = new Guid(0x2474f7c0, 0x9db1, 0x4f4f, 0xb6, 0x14, 0xdc, 0x5a, 0x89, 0x5, 0xb, 0x64);
        uint hresult = CoCreateRemoteThingInstance(&classId, null, 0x4U, &iid, out RemoteThing remoteThing);
        Marshal.ThrowExceptionForHR((int)hresult);
        return remoteThing;
    }

    private static unsafe Input CreateInput()
    {
        Guid classId = Guid.Parse("2F8EDC40-C145-4128-B451-559D45CD9074");
        Guid iid = new Guid(0x2474f7c0, 0x9db1, 0x4f4f, 0xb6, 0x14, 0xdc, 0x5a, 0x89, 0x5, 0xb, 0x65);
        uint hresult = CoCreateInputInstance(&classId, null, 0x4U, &iid, out Input input);
        Marshal.ThrowExceptionForHR((int)hresult);
        return input;
    }

    [DllImport("ole32", EntryPoint = "CoCreateInstance", ExactSpelling = true)]
    private unsafe static extern uint CoCreateRemoteThingInstance(Guid* rclsid, void* pUnkOuter, uint dwClsContext, Guid* riid, out RemoteThing ppv);

    [DllImport("ole32", EntryPoint = "CoCreateInstance", ExactSpelling = true)]
    private unsafe static extern uint CoCreateInputInstance(Guid* rclsid, void* pUnkOuter, uint dwClsContext, Guid* riid, out Input ppv);

    private void RemBtn_Click(object sender, RoutedEventArgs e)
    {
        RemResponse.Text = remoteThing.Rem(5, 4).ToString();
    }

    private async void DelayBtn_Click(object sender, RoutedEventArgs e)
    {
        DelayBtn.IsEnabled = false;
        DelayProg.IsIndeterminate = true;
        await remoteThing.DelayAsync(3000);
        DelayProg.IsIndeterminate = false;
        DelayBtn.IsEnabled = true;
    }

    private async void LoopBtn_Click(object sender, RoutedEventArgs e)
    {
        LoopBtn.IsEnabled = false;
        await remoteThing.LoopAsync(100).AsTask(new Progress<LoopProgress>(p =>
        {
            _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => LoopProg.Value = p.Count);
        }));
        LoopBtn.IsEnabled = true;
    }

    private async void ListBtn_Click(object sender, RoutedEventArgs e)
    {
        ListBtn.IsEnabled = false;
        var l = await remoteThing.GenerateListAsync(new ListOptions() { Count = 10, DelayTicks = 1000 }).AsTask(new Progress<ListProgress>(p =>
        {
            _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => ListProg.Value = p.Count);
        }));
        ListResp.Text = string.Join(";", l);
        ListBtn.IsEnabled = true;
    }

    private void DateTimeUtcBtn_Click(object sender, RoutedEventArgs e)
    {
        DateTimeUtcBtn.Content = remoteThing.NowUtc.ToString();
    }

    private async void OpenFileBtn_Click(object sender, RoutedEventArgs e)
    {
        var data = remoteThing.OpenFile("C:\\Windows\\explorer.exe").AsStreamForRead();
        byte[] buffer = new byte[10];
        await data.ReadAsync(buffer, 0, buffer.Length);
        OpenFileTxt.Text = string.Join("", buffer.Select(b => b.ToString("X2")));
    }

    private void GetTimesBtn_Click(object sender, RoutedEventArgs e)
    {
        var input = CreateInput();
        input.Description = "This is a description";
        input.Name = "This is a name";
        var times = remoteThing.GetTimes(input);
        LocalTimeTxt.Text = times.LocalNow.ToString();
        UtcTimeTxt.Text = times.UtcNow.ToString();
        NameAndDescriptionTxt.Text = times.NameAndDescription;
    }
}