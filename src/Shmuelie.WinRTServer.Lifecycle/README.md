# Shmuelie.WinRTServer.Lifecycle

Object-lifetime helpers for [Shmuelie.WinRTServer](https://github.com/shmuelie/Shmuelie.WinRTServer).

The servers don't track object lifetime themselves. Attach one of these helpers to
keep the host process alive until every client has released its objects:

- **`PollingServerLifetime`** — tracks live objects with `WeakReference`s and a GC
  poll (works with any `ComWrappers`).
- **`ReferenceCountedServerLifetime`** — deterministic tracking via an
  `IIUnknownStrategy` composed over `StrategyBasedComWrappers`.

Both implement **`IServerLifetime`** (`WaitForFirstObjectAsync`,
`WaitUntilEmptyAsync`, `IsEmpty`, `Empty`).

```csharp
using ComServer server = new();
using PollingServerLifetime lifetime = new(server);
server.RegisterClass<RemoteThing, IRemoteThing>();
server.Start();
await lifetime.WaitUntilEmptyAsync();
```

Depends on `Shmuelie.WinRTServer.Core`. See the
[documentation](https://shmuelie.github.io/Shmuelie.WinRTServer/articles/lifetime-and-di.html).
