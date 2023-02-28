using System;
using System.Runtime.InteropServices;
using System.Windows.Input;
#if UAP10_0
using Dispatcher = Windows.UI.Core.CoreDispatcher;
#else
using System.Windows.Threading;
#endif

namespace Shmuelie.WinRTServer.Sample.Proxies;

public sealed class MainPageViewModelProxy : RemotePropertyChangedAdapter
{
    private readonly MainPageViewModel viewModel;

    private MainPageViewModelProxy(Dispatcher dispatcher, MainPageViewModel viewModel) : base(viewModel, dispatcher)
    {
        this.viewModel = viewModel;
        IncrementCommand = new RemoteCommandAdapter(viewModel.IncrementCommand, dispatcher);
        DecrementCommand = new RemoteCommandAdapter(viewModel.DecrementCommand, dispatcher);
        ClearCommand = new RemoteCommandAdapter(viewModel.ClearCommand, dispatcher);
    }

    public ICommand IncrementCommand { get; }

    public ICommand DecrementCommand { get; }

    public ICommand ClearCommand { get; }

    public int Count => viewModel.Count;

    public static MainPageViewModelProxy Create(Dispatcher dispatcher)
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
