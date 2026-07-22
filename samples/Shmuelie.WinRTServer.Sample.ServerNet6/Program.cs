using System;
using System.Linq;
using System.Threading.Tasks;
using Shmuelie.WinRTServer.Sample.Interfaces;
using Shmuelie.WinRTServer.CsWinRT;

namespace Shmuelie.WinRTServer.Sample.Server;

public static class Program
{
    public async static Task Main(string[] args)
    {
        if (args.Contains("-COM"))
        {
            using ComServer server = new ComServer();
            using PollingServerLifetime lifetime = new PollingServerLifetime(server);
            server.RegisterClass<RemoteThing, IRemoteThing>();
            server.RegisterClass<Times, ITimes>();
            server.RegisterClass<Input, IInput>();
            server.Start();
            await lifetime.WaitForFirstObjectAsync();
            await lifetime.WaitUntilEmptyAsync();
        }
        else if (args.Contains("-WINRT"))
        {
            using WinRtServer server = new WinRtServer();
            using PollingServerLifetime lifetime = new PollingServerLifetime(server);
            server.RegisterClass<RemoteThing>();
            server.RegisterClass<Times>();
            server.RegisterClass<Input>();
            server.Start();
            await lifetime.WaitForFirstObjectAsync();
            await lifetime.WaitUntilEmptyAsync();
        }
    }
}
