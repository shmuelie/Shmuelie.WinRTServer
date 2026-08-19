# Shmuelie.WinRTServer.StrategyBased

`RegisterClass<…>` convenience extensions for
[Shmuelie.WinRTServer](https://github.com/shmuelie/Shmuelie.WinRTServer) that use the
runtime **`StrategyBasedComWrappers`** (source-generated COM interop).

Use this package when your project relies on the runtime's source-generated COM
interop rather than CsWinRT's projection.

```csharp
using Shmuelie.WinRTServer.StrategyBased;

using ComServer server = new();
server.RegisterClass<RemoteThing, IRemoteThing>();
server.Start();
```

The methods live in the `Shmuelie.WinRTServer.StrategyBased` namespace. For the
CsWinRT equivalent, use `Shmuelie.WinRTServer.CsWinRT` instead.

Depends on `Shmuelie.WinRTServer.Core`. See the
[documentation](https://shmuelie.github.io/Shmuelie.WinRTServer/articles/getting-started.html).
