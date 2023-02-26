using System;

namespace Shmuelie.WinRTServer.Sample;

internal sealed class CanExecuteRequestedEventArgs : EventArgs
{
    public CanExecuteRequestedEventArgs(bool canExecute, object? parameter)
    {
        CanExecute = canExecute;
        Parameter = parameter;
    }

    public bool CanExecute
    {
        get;
        set;
    }

    public object? Parameter
    {
        get;
    }
}
