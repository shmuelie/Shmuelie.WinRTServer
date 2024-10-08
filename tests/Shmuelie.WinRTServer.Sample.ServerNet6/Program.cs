﻿using System;
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
            await using (ComServer server = new ComServer())
            {
                server.RegisterClass<RemoteThing, IRemoteThing>();
                server.RegisterClass<Times, ITimes>();
                server.RegisterClass<Input, IInput>();
                server.Start();
                await server.WaitForFirstObjectAsync();
            }
        }
        else if (args.Contains("-WINRT"))
        {
            await using (WinRtServer server = new WinRtServer())
            {
                server.RegisterClass<RemoteThing>();
                server.RegisterClass<Times>();
                server.RegisterClass<Input>();
                server.Start();
                await server.WaitForFirstObjectAsync();
            }
        }
    }
}
