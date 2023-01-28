using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Shmuelie.WinRTServer.Sample.Interfaces;
using Windows.Foundation;

namespace Shmuelie.WinRTServer.Sample;

[Guid("4F59AF92-A98D-4A20-8C8D-1C076647A6AE")]
public sealed class RemoteThing : IRemoteThing
{
    public int Rem(int a, int b)
    {
        return Math.DivRem(a, b, out var _);
    }

    public async Task DelayAsync(int ticks, CancellationToken cancellationToken = default)
    {
        await Task.Delay(ticks, cancellationToken).ConfigureAwait(false);
    }

    [DebuggerNonUserCode]
    IAsyncAction IRemoteThing.DelayAsync(int ticks)
    {
        return AsyncInfo.Run(c => DelayAsync(ticks, c));
    }

    public async Task<IReadOnlyList<int>> GenerateListAsync(ListOptions options, IProgress<ListProgress> progress = default, CancellationToken cancellationToken = default)
    {
        List<int> list = new();
        for (int i = 0; i < options.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(options.DelayTicks, cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            list.Add(i * 2);
            progress?.Report(new ListProgress() { Count = i, Total = options.Count, Last = i * 2});
        }
        return list;
    }

    [DebuggerNonUserCode]
    IAsyncOperationWithProgress<IReadOnlyList<int>, ListProgress> IRemoteThing.GenerateListAsync(ListOptions options)
    {
        return AsyncInfo.Run<IReadOnlyList<int>, ListProgress>((c, p) => GenerateListAsync(options, p, c));
    }

    public async Task LoopAsync(int total, IProgress<LoopProgress> progress = default, CancellationToken cancellationToken = default)
    {
        for (int i = 0; i < total; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(500, cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report(new LoopProgress() { Count = i, Total = total });
        }
    }

    [DebuggerNonUserCode]
    IAsyncActionWithProgress<LoopProgress> IRemoteThing.LoopAsync(int total)
    {
        return AsyncInfo.Run<LoopProgress>((c, p) => LoopAsync(total, p, c));
    }
}
