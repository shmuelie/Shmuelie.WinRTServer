# Shmuelie.WinRTServer.DependencyInjection

Dependency-injection helpers for
[Shmuelie.WinRTServer](https://github.com/shmuelie/Shmuelie.WinRTServer).

Provides factories that resolve each activation from an `IServiceProvider`:

- **`ServiceProviderClassFactory<T, …>`** (1-, 2-, and 3-interface arities) — for COM
  (`ComServer`) registration.
- **`ServiceProviderActivationFactory<T>`** — for WinRT (`WinRtServer`) registration.

```csharp
ServiceProvider provider = new ServiceCollection()
    .AddTransient<RemoteThing>()
    .BuildServiceProvider();

server.RegisterClassFactory(
    new ServiceProviderClassFactory<RemoteThing, IRemoteThing>(provider),
    comWrappers);
```

When combined with `Shmuelie.WinRTServer.SourceGenerator`, the generator emits
`AddServerObjects` and provider-based registration helpers automatically.

Depends on `Shmuelie.WinRTServer.Core` and
`Microsoft.Extensions.DependencyInjection.Abstractions`. See the
[documentation](https://shmuelie.github.io/Shmuelie.WinRTServer/articles/lifetime-and-di.html).
