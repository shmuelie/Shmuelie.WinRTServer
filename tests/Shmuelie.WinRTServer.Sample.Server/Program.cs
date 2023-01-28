using System;
using System.Threading.Tasks;
using Shmuelie.WinRTServer.Sample.Interfaces;

namespace Shmuelie.WinRTServer.Sample.Server;

public static class Program
{
    public async static Task Main()
    {
        await using (ComServer server = new ComServer())
        {
            server.RegisterClass<RemoteThing, IRemoteThing>();
            server.Start();
            await server.WaitForFirstObjectAsync();
        }
    }
}
