﻿using System;
using System.IO;
using System.Linq;
using System.Windows;
using Shmuelie.WinRTServer.Sample.Interfaces;

namespace Shmuelie.WinRTServer.Sample.WpfNetFxClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly RemoteThing remoteThing;

        public MainWindow()
        {
            InitializeComponent();
            remoteThing = new RemoteThing();
            remoteThing.LoopCompleted += RemoteThing_LoopCompleted;
            DateTimeUtcBtn.Content = remoteThing.NowUtc.ToString();
        }

        private void RemoteThing_LoopCompleted(IRemoteThing sender, object args)
        {
            _ = Dispatcher.InvokeAsync(() => LoopProg.Value = 0);
        }

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
                _ = Dispatcher.InvokeAsync(() => LoopProg.Value = p.Count);
            }));
            LoopBtn.IsEnabled = true;
        }

        private async void ListBtn_Click(object sender, RoutedEventArgs e)
        {
            ListBtn.IsEnabled = false;
            var l = await remoteThing.GenerateListAsync(new ListOptions() { Count = 10, DelayTicks = 1000 }).AsTask(new Progress<ListProgress>(p =>
            {
                _ = Dispatcher.InvokeAsync(() => ListProg.Value = p.Count);
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
            var input = new Input()
            {
                Description = "This is a description",
                Name = "This is a name"
            };
            var times = remoteThing.GetTimes(input);
            LocalTimeTxt.Text = times.LocalNow.ToString();
            UtcTimeTxt.Text = times.UtcNow.ToString();
            NameAndDescriptionTxt.Text = times.NameAndDescription;
        }
    }
}
