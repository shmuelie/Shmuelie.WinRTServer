# Shmuelie.WinRTServer

**Out-of-Process WinRT/COM Server for .NET.**

The Component Object Model (COM) that underlies .NET and the Windows Runtime
supports Out-of-Process (OOP) servers, letting you use objects that live in a
different process (or even a different machine) as though they were local.
`Shmuelie.WinRTServer` provides the APIs to make writing that "server" side in
.NET easy.

> [!NOTE]
> COM and the Windows Runtime are Windows only.

## Why use it?

- **Cross-language** — because communication is over COM/WinRT, any COM-capable
  language can consume your server (the samples include a C++/WinRT client
  talking to a C# server).
- **Complex types** — return objects with methods, events, and properties, not
  just serialized data. If you can do it with a local object, you can do it with
  a remote one (including streaming a file across the boundary).
- **Built-in type support** — collections, maps, streams, and more work out of
  the box.
- **Security** — COM's security model is available; see
  [Server security](articles/security.md).

## Get started

- New here? Start with the [Getting Started walkthrough](articles/getting-started.md).
- Want the big picture? Read [How COM OOP server creation works](articles/how-com-oop-works.md).
- Looking for a type? Browse the [API Reference](xref:Shmuelie.WinRTServer).

## Feature guides

| Guide | What it covers |
| --- | --- |
| [Object lifetime & DI](articles/lifetime-and-di.md) | Keeping the process alive with `IServerLifetime`; resolving objects from a container |
| [Declarative registration](articles/source-generators.md) | `[ServerClass]` + the built-in source generator, vs. the CsWinRT generator |
| [Custom factories](articles/custom-factories.md) | Writing your own `BaseClassFactory` / `BaseActivationFactory` |
| [Using interfaces defined elsewhere](articles/external-interfaces.md) | Implementing contracts you didn't author (e.g. WinAppSDK) |
| [WinRT activation limits](articles/winrt-activation-limits.md) | Why UWP clients need COM activation |
| [Packaged COM](articles/packaged-com.md) | Registering the server in an MSIX package |
| [Windows Service vs. stand-alone exe](articles/hosting-models.md) | Choosing a hosting model |
| [Server security](articles/security.md) | `CoInitializeSecurity` and caller impersonation |

## Install

```
dotnet add package Shmuelie.WinRTServer
```

`Shmuelie.WinRTServer` is a **meta-package** that pulls in the whole stack. To
take only what you need, reference the individual packages instead:

- **`Shmuelie.WinRTServer.Core`** — the servers, factories, options, and security.
- **`Shmuelie.WinRTServer.Lifecycle`** — `IServerLifetime` helpers.
- **`Shmuelie.WinRTServer.StrategyBased`** / **`.CsWinRT`** — `RegisterClass<…>`
  extensions for your `ComWrappers` flavor.
- **`Shmuelie.WinRTServer.DependencyInjection`** — `IServiceProvider`-based factories.
- **`Shmuelie.WinRTServer.SourceGenerator`** — the `[ServerClass]` attribute + generator.

Every add-on depends on Core; the source-generator package ships the
`[ServerClass]` attribute alongside the analyzer.
