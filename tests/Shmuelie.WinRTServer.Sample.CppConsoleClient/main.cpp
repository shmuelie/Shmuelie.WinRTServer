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
    system("pause");

    co_return 0;
}

int main()
{
    init_apartment();

    return AsyncMain().get();
}
