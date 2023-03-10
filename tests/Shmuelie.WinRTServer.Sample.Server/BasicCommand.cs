using System;
using System.Runtime.InteropServices;
using Shmuelie.WinRTServer.Sample.Interfaces;

namespace Shmuelie.WinRTServer.Sample;

[ClassInterface(ClassInterfaceType.None)]
internal sealed class BasicCommand : IRemoteCommand
{
    private bool canExecute;

    public BasicCommand() : this(false)
    {
    }

    public BasicCommand(bool canExecute)
    {
        this.canExecute = canExecute;
    }

    public event EventHandler<object>? CanExecuteChanged;

    public bool CanExecute(object parameter)
    {
        CanExecuteRequestedEventArgs args = new(canExecute, parameter);
        CanExecuteRequested?.Invoke(this, args);
        canExecute = args.CanExecute;
        return canExecute;
    }

    public void Execute(object parameter)
    {
        ExecuteRequested?.Invoke(this, new ExecuteRequestedEventArgs(parameter));
    }

    public event EventHandler<CanExecuteRequestedEventArgs>? CanExecuteRequested;

    public event EventHandler<ExecuteRequestedEventArgs>? ExecuteRequested;

    public void NotifyCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
