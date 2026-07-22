# Getting Started

This walkthrough builds a complete out-of-process server and a client that uses
it. The moving parts mirror the reference sample under `samples/` in the
repository.

A working solution has up to four kinds of project (the reference sample lives
under `samples/` in the repository):

1. **Contract** — a C# project whose output is a WinMD containing the *interfaces*
   of your remote objects.
2. **Metadata** — a C++/WinRT project that emits a WinMD of *proxy* runtime
   classes (no C++ code, only IDL). Clients reference this.
3. **Server** — the process that implements the interfaces and registers them.
   This is the only project that references `Shmuelie.WinRTServer`.
4. **Client** — any COM-capable app (Win32/WPF/WinForms, UWP, C++/WinRT, …).

> [!TIP]
> The contract + metadata split exists because of C++/WinRT tooling
> requirements today. See [How COM OOP server creation works](how-com-oop-works.md)
> for the background.

## 1. Define the contract

The contract project is an ordinary C# project that outputs a WinMD. Its
interfaces follow a few rules:

- Assign a GUID with **`Windows.Foundation.Metadata.GuidAttribute`** (not the
  `System.Runtime.InteropServices` one).
- Async methods use WinRT async types (`IAsyncAction`, `IAsyncOperation<T>`, the
  `...WithProgress<>` variants) — never `Task`/`Task<T>`.
- Events use `TypedEventHandler<,>` or `EventHandler<T>`.

```csharp
using Windows.Foundation;
using Windows.Foundation.Metadata;

namespace Contoso.Remoting;

[Guid("2474F7C0-9DB1-4F4F-B614-DC5A89050B64")]
public interface IRemoteThing
{
    int Rem(int a, int b);

    IAsyncAction DelayAsync(int ticks);

    DateTimeOffset NowUtc { get; }

    event TypedEventHandler<IRemoteThing, object> LoopCompleted;
}
```

## 2. Generate the proxy metadata

The metadata project is a C++/WinRT project using [MIDL 3.0][midl]. Each
`runtimeclass` implements a contract interface and needs an empty constructor:

```idl
namespace Contoso.Remoting
{
    runtimeclass RemoteThing : [default] Contoso.Remoting.IRemoteThing
    {
        RemoteThing();
    }
}
```

> [!IMPORTANT]
> Because the solution mixes SDK-style and C++/WinRT, a real `nuget restore` is
> required, and the C++ project needs
> `<RestoreProjectStyle>Packages.config</RestoreProjectStyle>`.

## 3. Implement and register the server

The implementation lives in the server project. Note the two differences from
the contract:

- The implementation's GUID uses **`System.Runtime.InteropServices.GuidAttribute`**.
- Public methods can be normal `Task`-based methods; an explicit interface
  member adapts them to the WinRT async types using
  [`AsyncInfo.Run`](https://learn.microsoft.com/dotnet/api/system.runtime.interopservices.windowsruntime.asyncinfo).

```csharp
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;

[Guid("474527DE-81CD-466E-ADCF-6E3809CD5033")]
public sealed partial class RemoteThing : IRemoteThing
{
    public int Rem(int a, int b) => Math.DivRem(a, b, out _);

    public DateTimeOffset NowUtc => DateTimeOffset.UtcNow;

    public async Task DelayAsync(int ticks, CancellationToken ct = default)
        => await Task.Delay(ticks, ct).ConfigureAwait(false);

    [DebuggerNonUserCode]
    IAsyncAction IRemoteThing.DelayAsync(int ticks) => AsyncInfo.Run(c => DelayAsync(ticks, c));

    public event TypedEventHandler<IRemoteThing, object?>? LoopCompleted;
}
```

Register and run the server. The server itself does **not** keep the process
alive; attach an [`IServerLifetime`](xref:Shmuelie.WinRTServer.IServerLifetime)
helper to wait until all created objects have been released:

```csharp
using Shmuelie.WinRTServer;
using Shmuelie.WinRTServer.CsWinRT; // or .StrategyBased

using ComServer server = new();
using PollingServerLifetime lifetime = new(server);

server.RegisterClass<RemoteThing, IRemoteThing>();
server.Start();

await lifetime.WaitUntilEmptyAsync();
```

Use [`WinRtServer`](xref:Shmuelie.WinRTServer.WinRtServer) instead if your
clients can use WinRT activation. See
[Object lifetime & dependency injection](lifetime-and-di.md) for the full story
and [Declarative registration](source-generators.md) to skip the manual
`RegisterClass` calls entirely.

## 4. Consume it from a client

A full-trust client that uses WinRT activation creates the object with plain
`new`:

```csharp
using Contoso.Remoting;

var thing = new RemoteThing();      // activates the OOP server
int r = thing.Rem(5, 4);
await thing.DelayAsync(3000);
```

A UWP client cannot use WinRT activation and must use COM activation instead —
see [WinRT activation limits](winrt-activation-limits.md).

## Choosing a ComWrappers flavor

The `RegisterClass<...>` extension methods live in two namespaces that differ
only by the `ComWrappers` they use:

- **`Shmuelie.WinRTServer.CsWinRT`** — for projects referencing
  `Microsoft.Windows.CsWinRT` (uses CsWinRT's `DefaultComWrappers`).
- **`Shmuelie.WinRTServer.StrategyBased`** — for source-generated COM interop
  (uses the runtime `StrategyBasedComWrappers`).

## Troubleshooting

- Make sure the client references both WinMDs (contract and metadata).
- Contract interfaces use `Windows.Foundation.Metadata.GuidAttribute`; server
  implementations use `System.Runtime.InteropServices.GuidAttribute`. Swapping
  these is the most common mistake.

[midl]: https://learn.microsoft.com/uwp/midl-3/
