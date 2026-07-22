![Out-of-Process WinRT/COM Server](https://raw.githubusercontent.com/shmuelie/Shmuelie.WinRTServer/main/Shmuelie.WinRTServer.Title.png)
==================================================================

[![.NET](https://github.com/Shmuelie/Shmuelie.WinRTServer/workflows/.NET/badge.svg)][1] [![NuGet](https://img.shields.io/nuget/dt/Shmuelie.WinRTServer.svg)][2] [![NuGet](https://img.shields.io/nuget/vpre/Shmuelie.WinRTServer.svg)][3]

# What is it?

The Component Object Model (COM) API that underlines .NET and the Windows
Runtime supports the concept of Out Of Process (OOP) Servers. This allows for
using objects that are in a different process (or even a different machine) as
though they were in the local process. This library adds APIs to make the
process of creating the "server" in .NET much easier.

> **Note**: COM and Windows Runtime are Windows only.

# Why?

- **Cross Language** — because communication is over COM/WinRT, any language that
  can use COM can use this library (the samples include a C++/WinRT console
  client talking to a C# server).
- **Complex Types** — return objects with methods, events, and properties, not
  just serialized data. If you can do it with a local object, you can do it with
  a remote one — including streaming a file across the boundary.
- **Built-in Support for Types** — collections, maps, streams, and more work out
  of the box.
- **Security** — COM's security model is available. See [Security in COM][7].

# Install

```
dotnet add package Shmuelie.WinRTServer
```

# Documentation

Full documentation, guides, and API reference live on the **[documentation
site][docs]**:

- [Getting Started][docs-start] — build a contract, metadata, server, and client
  end to end.
- [How COM OOP server creation works][docs-oop] — the concepts behind the API.
- [Object lifetime & dependency injection][docs-lifetime] — keeping the process
  alive with `IServerLifetime`, and resolving objects from a container.
- [Declarative registration & source generators][docs-gen] — `[ServerClass]` and
  the built-in generator.
- [Creating custom factories][docs-factories],
  [using interfaces defined elsewhere][docs-external],
  [WinRT activation limits][docs-winrt],
  [packaged COM][docs-packaged],
  [hosting models][docs-hosting], and
  [server security][docs-security].
- [API Reference][docs-api].

# Quick look

```csharp
using Shmuelie.WinRTServer;
using Shmuelie.WinRTServer.CsWinRT; // or .StrategyBased

using ComServer server = new();
using PollingServerLifetime lifetime = new(server);

server.RegisterClass<RemoteThing, IRemoteThing>();
server.Start();

await lifetime.WaitUntilEmptyAsync();
```

# Samples

Runnable samples live under the `tests` folder:

- .NET 10 Server
- UWP .NET Client App
- C++/WinRT Console Client App
- WPF .NET Framework Client App
- WPF .NET 10 Client App

[1]: https://github.com/Shmuelie/Shmuelie.WinRTServer/actions
[2]: https://www.nuget.org/stats/packages/Shmuelie.WinRTServer?groupby=Version
[3]: https://www.nuget.org/packages/Shmuelie.WinRTServer/
[7]: https://learn.microsoft.com/en-us/windows/win32/com/security-in-com
[docs]: https://shmuelie.github.io/Shmuelie.WinRTServer/
[docs-start]: https://shmuelie.github.io/Shmuelie.WinRTServer/articles/getting-started.html
[docs-oop]: https://shmuelie.github.io/Shmuelie.WinRTServer/articles/how-com-oop-works.html
[docs-lifetime]: https://shmuelie.github.io/Shmuelie.WinRTServer/articles/lifetime-and-di.html
[docs-gen]: https://shmuelie.github.io/Shmuelie.WinRTServer/articles/source-generators.html
[docs-factories]: https://shmuelie.github.io/Shmuelie.WinRTServer/articles/custom-factories.html
[docs-external]: https://shmuelie.github.io/Shmuelie.WinRTServer/articles/external-interfaces.html
[docs-winrt]: https://shmuelie.github.io/Shmuelie.WinRTServer/articles/winrt-activation-limits.html
[docs-packaged]: https://shmuelie.github.io/Shmuelie.WinRTServer/articles/packaged-com.html
[docs-hosting]: https://shmuelie.github.io/Shmuelie.WinRTServer/articles/hosting-models.html
[docs-security]: https://shmuelie.github.io/Shmuelie.WinRTServer/articles/security.html
[docs-api]: https://shmuelie.github.io/Shmuelie.WinRTServer/api/Shmuelie.WinRTServer.html
