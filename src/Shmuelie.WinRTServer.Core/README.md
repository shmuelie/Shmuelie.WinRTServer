# Shmuelie.WinRTServer.Core

The core of [Shmuelie.WinRTServer](https://github.com/shmuelie/Shmuelie.WinRTServer) —
an Out-of-Process WinRT/COM server for .NET (Windows only).

This package contains the essentials:

- **`ComServer`** and **`WinRtServer`** — the out-of-process server hosts (COM and
  WinRT activation).
- **Factories** — `BaseClassFactory` / `BaseActivationFactory` plus the
  `General*` (reflection) and `Delegate*` (`Func<T>`) implementations, including
  multi-interface class-factory arities.
- **`ServerOptions` / `ServerRuntimeOptions`** — `IGlobalOptions` configuration.
- **`ServerSecurity`** — `CoInitializeSecurity` and caller impersonation helpers.
- The `ComWrappers`-based COM interop.

## Companion packages

Install alongside Core as needed:

- `Shmuelie.WinRTServer.Lifecycle` — keep the process alive until objects are released.
- `Shmuelie.WinRTServer.StrategyBased` / `Shmuelie.WinRTServer.CsWinRT` — `RegisterClass<…>` convenience for your `ComWrappers` flavor.
- `Shmuelie.WinRTServer.DependencyInjection` — resolve server objects from a container.
- `Shmuelie.WinRTServer.SourceGenerator` — declarative `[ServerClass]` registration.
- `Shmuelie.WinRTServer` — meta-package that pulls in everything.

See the [documentation](https://shmuelie.github.io/Shmuelie.WinRTServer/) for guides and API reference.
