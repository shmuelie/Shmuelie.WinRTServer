# Hosting models: Windows Service vs. stand-alone executable

A `Shmuelie.WinRTServer` server is just a process that registers factories and
stays alive while clients hold objects. *How* that process is launched and kept
running is up to you. Two common models are a **COM-launched stand-alone
executable** and a **Windows Service**.

## Stand-alone executable (COM-launched)

This is the default the samples use. The server is a normal `.exe`; COM starts
it on demand when a client activates one of its classes, and you let it exit once
all objects are released.

```csharp
using ComServer server = new();
using PollingServerLifetime lifetime = new(server);

server.RegisterClass<RemoteThing, IRemoteThing>();
server.Start();

await lifetime.WaitUntilEmptyAsync();   // exit when the last client is gone
```

- **Launch:** on demand by COM (SCM/`CoCreateInstance`) or manually.
- **Lifetime:** tie it to
  [`WaitUntilEmptyAsync`](xref:Shmuelie.WinRTServer.IServerLifetime.WaitUntilEmptyAsync)
  so the process is short-lived and cheap.
- **Identity:** runs as the activating user (subject to COM security).
- **Best for:** per-user objects, interactive scenarios, Store/MSIX apps.

## Windows Service

Host the same registration inside a long-running service when the server must be
available independently of any client, run under a fixed account, or start with
the machine.

```csharp
using Microsoft.Extensions.Hosting;

await Host.CreateApplicationBuilder(args)
    .ConfigureServices(s => s.AddHostedService<ServerWorker>())
    .Build()
    .RunAsync();

sealed class ServerWorker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using ComServer server = new();
        server.RegisterClass<RemoteThing, IRemoteThing>();
        server.Start();
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
```

- **Launch:** by the Service Control Manager, optionally at boot.
- **Lifetime:** lives for the service's lifetime — you generally do **not** wait
  for empty; the service decides when to stop.
- **Identity:** a fixed service account, not the caller. Configure COM security
  and, when you need the caller's identity, impersonate per call — see
  [Server security](security.md).
- **Best for:** always-on shared servers, machine-wide services, non-interactive
  hosts.

## Choosing

| | Stand-alone exe | Windows Service |
| --- | --- | --- |
| Started | on demand by COM | by the SCM / at boot |
| Runs as | the activating user | a fixed service account |
| Typical lifetime | until objects released | until the service stops |
| Registration | `.wapproj` / registry / packaged COM | same, plus service install |

Whichever you pick, the `Shmuelie.WinRTServer` API is identical — only the
surrounding host and how you gate the process lifetime change.
