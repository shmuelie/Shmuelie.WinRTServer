using System;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Shmuelie.WinRTServer.Sample.Interfaces;

namespace Shmuelie.WinRTServer.Sample;

[ClassInterface(ClassInterfaceType.None)]
[Guid("4F59AF92-A98D-4A20-8C8D-1D076647A6B0")]
internal class MainPageViewModel : IMainPageViewModel
{
    private readonly BasicCommand incrementCommand;
    private readonly BasicCommand decrementCommand;
    private readonly BasicCommand clearCommand;

    public event EventHandler<IRemotePropertyChangedEventArgs>? PropertyChanged;

    public MainPageViewModel()
    {
        incrementCommand = new BasicCommand();
        incrementCommand.ExecuteRequested += IncrementCommand_ExecuteRequested;
        incrementCommand.CanExecuteRequested += IncrementCommand_CanExecuteRequested;

        decrementCommand = new BasicCommand();
        decrementCommand.ExecuteRequested += DecrementCommand_ExecuteRequested;
        decrementCommand.CanExecuteRequested += DecrementCommand_CanExecuteRequested;

        clearCommand = new BasicCommand();
        clearCommand.ExecuteRequested += ClearCommand_ExecuteRequested;
        clearCommand.CanExecuteRequested += ClearCommand_CanExecuteRequested;
    }

    private void ClearCommand_CanExecuteRequested(object sender, CanExecuteRequestedEventArgs e)
    {
        e.CanExecute = Count != 0;
    }

    private void ClearCommand_ExecuteRequested(object sender, ExecuteRequestedEventArgs args)
    {
        Count = 0;
        OnCountChanged();
    }

    private void DecrementCommand_CanExecuteRequested(object sender, CanExecuteRequestedEventArgs args)
    {
        args.CanExecute = Count > int.MinValue;
    }

    private void IncrementCommand_CanExecuteRequested(object sender, CanExecuteRequestedEventArgs args)
    {
        args.CanExecute = Count < int.MaxValue;
    }

    private void DecrementCommand_ExecuteRequested(object sender, ExecuteRequestedEventArgs args)
    {
        Count--;
        OnCountChanged();
    }

    private void IncrementCommand_ExecuteRequested(object sender, ExecuteRequestedEventArgs args)
    {
        Count++;
        OnCountChanged();
    }

    public IRemoteCommand IncrementCommand => incrementCommand;

    public IRemoteCommand DecrementCommand => decrementCommand;

    public IRemoteCommand ClearCommand => clearCommand;

    public int Count { get; private set; }

    private void OnCountChanged()
    {
        PropertyChanged?.Invoke(this, new RemotePropertyChangedEventArgs(nameof(Count)));
        incrementCommand.NotifyCanExecuteChanged();
        decrementCommand.NotifyCanExecuteChanged();
        clearCommand.NotifyCanExecuteChanged();
    }
}
