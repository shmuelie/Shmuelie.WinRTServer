using System;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Shmuelie.WinRTServer.Sample.Interfaces;
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
        Guid iid = typeof(IMainPageViewModel).GUID;
        uint hresult =
#if NET6_0_WINDOWS10_0_22000_0
            CoCreateInstance(&classId, null, 0x4U, &iid, out IntPtr ppv)
#else
            CoCreateInstance(&classId, null, 0x4U, &iid, out MainPageViewModel mainPageViewModel)
#endif
            ;
        Marshal.ThrowExceptionForHR((int)hresult);
#if NET6_0_WINDOWS10_0_22000_0
        return MainPageViewModel.FromAbi(ppv);
#else
        return mainPageViewModel;
#endif
    }

    [DllImport("ole32")]
    private unsafe static extern uint CoCreateInstance(Guid* rclsid, void* pUnkOuter, uint dwClsContext, Guid* riid, out
#if NET6_0_WINDOWS10_0_22000_0
        IntPtr
#else
        MainPageViewModel
#endif
        ppv);
}
