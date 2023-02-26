using System;
using Shmuelie.WinRTServer.Sample.Interfaces;

namespace Shmuelie.WinRTServer.Sample;

internal sealed class MainPageViewModelFactory
{
    private readonly Lazy<MainPageViewModel> instance = new();

    private MainPageViewModel Get() => instance.Value;

    public BaseClassFactory ClassFactory { get; }

    public BaseActivationFactory ActivationFactory { get; }

    public MainPageViewModelFactory()
    {
        ClassFactory = new DelegateClassFactory<MainPageViewModel, IMainPageViewModel>(Get);
        ActivationFactory = new DelegateActivationFactory<MainPageViewModel>(Get);
    }
}

