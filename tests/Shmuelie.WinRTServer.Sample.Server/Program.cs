using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Shmuelie.WinRTServer.Sample.Interfaces;

namespace Shmuelie.WinRTServer.Sample.Server;

public static class Program
{
    public async static Task Main()
    {
        if (Debugger.IsAttached)
        {
            Debugger.Break();
        }
        else
        {
            Debugger.Launch();
        }

        await Task.WhenAll(RunComServer(), RunWinRtServer());

        static async Task RunComServer()
        {
            await using (ComServer server = new ComServer())
            {
                server.RegisterClass<RemoteThing, IRemoteThing>();
                server.Start();
                await server.WaitForFirstObjectAsync();
            }
        }

        static async Task RunWinRtServer()
        {
            await using (WinRtServer server = new WinRtServer())
            {
                server.RegisterClass<RemoteActivation>();
                server.Start();
                await server.WaitForFirstObjectAsync();
            }
        }
    }
}
