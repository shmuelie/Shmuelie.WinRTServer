#include "pch.h"
#include <winrt/Shmuelie.WinRTServer.Sample.Interfaces.h>
#include <winrt/Shmuelie.WinRTServer.Sample.h>

#include "Progress.h"

using namespace winrt;
using namespace Windows::Foundation;
using namespace Shmuelie::WinRTServer::Sample;
using namespace Shmuelie::WinRTServer::Sample::Interfaces;

IAsyncOperation<int> AsyncMain()
{
    RemoteThing remoteThing;

    int32_t rem = remoteThing.Rem(5, 4);
    printf("Rem: %i\n", rem);

    printf("Delay");
    IndefiniteSpinner spinner;
    spinner.ShowSpinner();
    co_await remoteThing.DelayAsync(3000);
    spinner.StopSpinner();
    printf("Delayed\n");

    printf("Looping\n");
    ProgressBar bar;
    bar.ShowProgress(0, 30);
    auto loopOp{ remoteThing.LoopAsync(30) };
    loopOp.Progress([&bar](auto const&, LoopProgress p)
        {
            bar.ShowProgress(p.Count, p.Total);
        });
    co_await loopOp;
    bar.EndProgress(true);
    printf("Looped\n");

    system("pause");

    co_return 0;
}

int main()
{
    init_apartment();

    return AsyncMain().get();
}
