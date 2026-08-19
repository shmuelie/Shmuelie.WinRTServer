# Object lifetime & dependency injection

The servers in this library intentionally do **not** track the lifetime of the
objects they create. A freshly started server will happily exit the moment your
`Main` returns. To keep the host process alive until every client has released
its objects, attach an external lifetime helper.

## Why lifetime is external

Both [`ComServer`](xref:Shmuelie.WinRTServer.ComServer) and
[`WinRtServer`](xref:Shmuelie.WinRTServer.WinRtServer) implement
[`IServer`](xref:Shmuelie.WinRTServer.IServer), which exposes an
[`InstanceCreated`](xref:Shmuelie.WinRTServer.IServer.InstanceCreated) event and
nothing else about object lifetime. Tracking is opt-in through
[`IServerLifetime`](xref:Shmuelie.WinRTServer.IServerLifetime), so you can pick
the strategy that fits — or write your own — instead of the server dictating one.

Every helper exposes:

- [`WaitForFirstObjectAsync`](xref:Shmuelie.WinRTServer.IServerLifetime.WaitForFirstObjectAsync) — completes when the first object is created.
- [`WaitUntilEmptyAsync`](xref:Shmuelie.WinRTServer.IServerLifetime.WaitUntilEmptyAsync) — completes once at least one object existed and all have been released.
- [`IsEmpty`](xref:Shmuelie.WinRTServer.IServerLifetime.IsEmpty) and an [`Empty`](xref:Shmuelie.WinRTServer.IServerLifetime.Empty) event.

The typical host waits for empty, then disposes:

```csharp
using ComServer server = new();
using PollingServerLifetime lifetime = new(server);

server.RegisterClass<RemoteThing, IRemoteThing>();
server.Start();

await lifetime.WaitUntilEmptyAsync();
```

## Choosing a helper

### PollingServerLifetime

[`PollingServerLifetime`](xref:Shmuelie.WinRTServer.PollingServerLifetime) holds
a [`WeakReference`](xref:System.WeakReference) to each created object and, on a
timer (60 s by default), forces a garbage collection and prunes dead references.
When the last one is gone it signals *empty*.

- **Pros:** works with any `ComWrappers`; no special registration.
- **Cons:** "empty" is detected on a delay because it depends on the GC.

```csharp
using PollingServerLifetime lifetime = new(server, pollIntervalMilliseconds: 30_000);
```

### ReferenceCountedServerLifetime

[`ReferenceCountedServerLifetime`](xref:Shmuelie.WinRTServer.ReferenceCountedServerLifetime)
tracks outstanding references **deterministically** by composing a counting
`IIUnknownStrategy` over the runtime `StrategyBasedComWrappers`. There is no GC
or timer involved.

Because tracking happens inside the `ComWrappers`, you must register your classes
with the helper's [`ComWrappers`](xref:Shmuelie.WinRTServer.ReferenceCountedServerLifetime.ComWrappers):

```csharp
using ComServer server = new();
using ReferenceCountedServerLifetime lifetime = new(server);

server.RegisterClassFactory(
    new GeneralClassFactory<RemoteThing, IRemoteThing>(),
    lifetime.ComWrappers);

server.Start();
await lifetime.WaitUntilEmptyAsync();
```

| | `PollingServerLifetime` | `ReferenceCountedServerLifetime` |
| --- | --- | --- |
| Detection | GC + timer (delayed) | Deterministic (`AddRef`/`Release`) |
| ComWrappers | any | must use its `ComWrappers` |
| Overhead | periodic full GC | per-reference counting |

## Dependency injection

The **`Shmuelie.WinRTServer.DependencyInjection`** package integrates with
`Microsoft.Extensions.DependencyInjection`. The core library takes no hard
dependency on a container. Two building blocks exist:

### ServiceProvider factories

[`ServiceProviderClassFactory<T, …>`](xref:Shmuelie.WinRTServer.ServiceProviderClassFactory`2)
and
[`ServiceProviderActivationFactory<T>`](xref:Shmuelie.WinRTServer.ServiceProviderActivationFactory`1)
(from the `Shmuelie.WinRTServer.DependencyInjection` package) resolve each
activation from an [`IServiceProvider`](xref:System.IServiceProvider):

```csharp
ServiceProvider provider = new ServiceCollection()
    .AddTransient<RemoteThing>()
    .BuildServiceProvider();

server.RegisterClassFactory(
    new ServiceProviderClassFactory<RemoteThing, IRemoteThing>(provider),
    comWrappers);
```

### Generated helpers

If you annotate your classes with `[ServerClass]` and reference both the
`Shmuelie.WinRTServer.DependencyInjection` and
`Microsoft.Extensions.DependencyInjection` packages, the source generator emits
`AddServerObjects` and provider-based `RegisterGeneratedClasses` overloads for
you. See
[Declarative registration & source generators](source-generators.md).
