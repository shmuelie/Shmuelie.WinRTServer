using System;
using System.Windows.Input;
using Shmuelie.WinRTServer.Sample.Interfaces;

namespace Shmuelie.WinRTServer.Sample.Proxies;

public sealed class RemoteCommandAdapter : ICommand
{
    private readonly
#if UAP10_0
    Windows.UI.Core.CoreDispatcher
#else
        System.Windows.Threading.Dispatcher
#endif
    ? dispatcher;
    private readonly IRemoteCommand command;

    public RemoteCommandAdapter(IRemoteCommand command,
#if UAP10_0
        Windows.UI.Core.CoreDispatcher
#else
        System.Windows.Threading.Dispatcher
#endif
        dispatcher)
    {
        this.command = command;
        this.dispatcher = dispatcher;
        command.CanExecuteChanged += Command_CanExecuteChanged;
    }

    private void Command_CanExecuteChanged(object? sender, object e)
    {
        _ =
#if UAP10_0
    dispatcher?.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
#else
            dispatcher?.InvokeAsync(
#endif
            () => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object parameter)
    {
        return command.CanExecute(parameter);
    }

    public void Execute(object parameter)
    {
        command.Execute(parameter);
    }
}
