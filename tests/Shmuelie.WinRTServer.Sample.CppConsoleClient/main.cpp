#include "pch.h"
#include <winrt/Shmuelie.WinRTServer.Sample.Interfaces.h>
#include <winrt/Shmuelie.WinRTServer.Sample.h>

using namespace winrt;
using namespace Windows::Foundation;
using namespace Shmuelie::WinRTServer::Sample;
using namespace Shmuelie::WinRTServer::Sample::Interfaces;

IAsyncOperation<int> AsyncMain()
{
    RemoteThing remoteThing;

    int32_t rem = remoteThing.Rem(5, 4);
    printf("Rem: %i\n", rem);

    printf("Delay Start...");
    co_await remoteThing.DelayAsync(3000);
    printf("Stop\n");

    co_return 0;
}

int main()
{
    init_apartment();

    return AsyncMain().get();
}
