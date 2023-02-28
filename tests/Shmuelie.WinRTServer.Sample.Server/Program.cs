using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shmuelie.WinRTServer.Sample.Server;

public static class Program
{
    public async static Task Main(string[] args)
    {
        MainPageViewModelFactory factory = new();

        if (args.Contains("-COM"))
        {
            await using (ComServer server = new ComServer())
            {
                server.RegisterClassFactory(factory.ClassFactory);
                server.Start();
                await Task.Delay(-1);
            }
        }
        else if (args.Contains("-WINRT"))
        {
            await using (WinRtServer server = new WinRtServer())
            {
                server.RegisterActivationFactory(factory.ActivationFactory);
                server.Start();
                await Task.Delay(-1);
            }
        }
    }
}
