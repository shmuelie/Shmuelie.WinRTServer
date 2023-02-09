using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Storage.Streams;

namespace Shmuelie.WinRTServer.Sample.Interfaces;

[Guid(0x2474f7c0, 0x9db1, 0x4f4f, 0xb6, 0x14, 0xdc, 0x5a, 0x89, 0x5, 0xb, 0x64)]

public interface IRemoteThing
{
    int Rem(int a, int b);

    IAsyncAction DelayAsync(int ticks);

    IAsyncActionWithProgress<LoopProgress> LoopAsync(int total);

    IAsyncOperationWithProgress<IReadOnlyList<int>, ListProgress> GenerateListAsync(ListOptions options);

    event TypedEventHandler<IRemoteThing, object> LoopCompleted;

    DateTimeOffset NowUtc { get; }

    IInputStream OpenFile(string path);

    ITimes GetTimes(IInput input);
}
