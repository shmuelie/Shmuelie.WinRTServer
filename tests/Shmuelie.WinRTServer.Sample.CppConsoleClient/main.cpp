#include "pch.h"
#include <winrt/Shmuelie.WinRTServer.Sample.Interfaces.h>
#include <winrt/Shmuelie.WinRTServer.Sample.h>

#include "Progress.h"

#include <iostream>
#include <algorithm>

using namespace winrt;
using namespace Windows::Foundation;
using namespace Windows::Storage::Streams;
using namespace Shmuelie::WinRTServer::Sample;
using namespace Shmuelie::WinRTServer::Sample::Interfaces;

IAsyncOperation<int> AsyncMain()
{
    RemoteThing remoteThing;

    int32_t rem = remoteThing.Rem(5, 4);
    std::cout << "Rem: " << rem << std::endl;

    std::cout << "Delay";
    IndefiniteSpinner spinner;
    spinner.ShowSpinner();
    co_await remoteThing.DelayAsync(3000);
    spinner.StopSpinner();
    std::cout << "Delayed" << std::endl;

    std::cout << "Looping" << std::endl;
    ProgressBar bar;
    bar.ShowProgress(0, 30);
    auto loopOp{ remoteThing.LoopAsync(30) };
    loopOp.Progress([&bar](auto const&, LoopProgress p)
        {
            bar.ShowProgress(p.Count, p.Total);
        });
    co_await loopOp;
    bar.EndProgress(true);
    std::cout << "Looped" << std::endl;

    std::cout << "Listing" << std::endl;

    ListOptions options;
    options.Count = 10;
    options.DelayTicks = 1000;
    bar.ShowProgress(0, 10);
    auto listOp{ remoteThing.GenerateListAsync(options) };
    listOp.Progress([&bar](auto const&, ListProgress p)
        {
            bar.ShowProgress(p.Count, p.Total);
        });
    const auto& listResult = co_await listOp;
    bar.EndProgress(true);
    std::cout << "List: " << std::endl;
    std::for_each(listResult.begin(), listResult.end(), [](int32_t i)
        {
            std::cout << "\t" << i << std::endl;
        });

    std::cout << "Now: " << clock::to_sys(remoteThing.NowUtc()) << std::endl;

    std::cout << "Open File";

    auto data = remoteThing.OpenFile(L"C:\\Windows\\explorer.exe");
    Buffer buffer{ 10 };
    spinner.ShowSpinner();
    IBuffer read = co_await data.ReadAsync(buffer, buffer.Capacity(), InputStreamOptions::None);
    spinner.StopSpinner();
    std::cout << "Data:" << std::endl;
    for (uint32_t i = 0; i < read.Length(); i++)
    {
        std::cout << "\t" << (uint32_t)*(read.data() + i) << std::endl;
    }

    system("pause");

    co_return 0;
}

int main()
{
    init_apartment();

    return AsyncMain().get();
}
