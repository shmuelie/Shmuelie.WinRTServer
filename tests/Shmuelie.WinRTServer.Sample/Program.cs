using System;
using System.Threading.Tasks;

namespace Shmuelie.WinRTServer.Sample;

public static class Program
{
    public static async Task Main()
    {
        await using (ComServer server = new ComServer())
        {
            server.RegisterClass<ExampleClass, IExampleClass>();
            server.Start();
        }
    }
}
