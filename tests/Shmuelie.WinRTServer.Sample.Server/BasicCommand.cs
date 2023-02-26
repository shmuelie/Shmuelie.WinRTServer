using System;
using System.Windows.Input;

namespace Shmuelie.WinRTServer.Sample;

internal sealed class BasicCommand : ICommand
{
    private bool canExecute;

    public BasicCommand() : this(false)
    {
    }

    public BasicCommand(bool canExecute)
    {
        this.canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

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
