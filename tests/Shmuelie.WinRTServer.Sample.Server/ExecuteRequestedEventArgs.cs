using System;

namespace Shmuelie.WinRTServer.Sample;

internal sealed class ExecuteRequestedEventArgs : EventArgs
{
    public ExecuteRequestedEventArgs(object? parameter)
    {
        Parameter = parameter;
    }

    public object? Parameter
    {
        get;
    }
}
