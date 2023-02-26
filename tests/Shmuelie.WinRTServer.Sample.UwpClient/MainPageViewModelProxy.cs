using System.Runtime.InteropServices;
using System;
using System.Windows.Input;
using Windows.UI.Core;

#nullable enable

namespace Shmuelie.WinRTServer.Sample.UwpClient;

internal sealed class MainPageViewModelProxy : RemotePropertyChangedAdapter
{
    private readonly MainPageViewModel viewModel;

    private MainPageViewModelProxy(CoreDispatcher dispatcher, MainPageViewModel viewModel) : base(viewModel, dispatcher)
    {
        this.viewModel = viewModel;
    }

    public ICommand IncrementCommand => viewModel.IncrementCommand;

    public ICommand DecrementCommand => viewModel.DecrementCommand;

    public ICommand ClearCommand => viewModel.ClearCommand;

    public int Count => viewModel.Count;

    public static MainPageViewModelProxy Create(CoreDispatcher dispatcher)
    {
        return new MainPageViewModelProxy(dispatcher, CreateMainPageViewModel());
    }

    private static unsafe MainPageViewModel CreateMainPageViewModel()
    {
        Guid classId = Guid.Parse("4F59AF92-A98D-4A20-8C8D-1D076647A6B0");
        Guid iid = new Guid(0x2474f7c0, 0x9db1, 0x4f4f, 0xb6, 0x14, 0xdd, 0x5a, 0x89, 0x5, 0xb, 0x65);
        uint hresult = CoCreateInstance(&classId, null, 0x4U, &iid, out MainPageViewModel mainPageViewModel);
        Marshal.ThrowExceptionForHR((int)hresult);
        return mainPageViewModel;
    }

    [DllImport("ole32")]
    private unsafe static extern uint CoCreateInstance(Guid* rclsid, void* pUnkOuter, uint dwClsContext, Guid* riid, out MainPageViewModel ppv);
}
