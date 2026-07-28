# Packages

`Shmuelie.WinRTServer` is published as a set of focused packages so you install
only what you need. Every package shares the same icon and metadata and carries a
README describing just its contents.

| Package | What it adds | Depends on |
| --- | --- | --- |
| **Shmuelie.WinRTServer.Core** | The `ComServer` / `WinRtServer` hosts, class/activation factories, `ServerOptions`, and `ServerSecurity`. Everything else builds on this. | — |
| **Shmuelie.WinRTServer.Lifecycle** | `IServerLifetime` helpers (`PollingServerLifetime`, `ReferenceCountedServerLifetime`) to keep the process alive until objects are released. | Core |
| **Shmuelie.WinRTServer.StrategyBased** | `RegisterClass<…>` extensions using the runtime `StrategyBasedComWrappers`. | Core |
| **Shmuelie.WinRTServer.CsWinRT** | `RegisterClass<…>` extensions using CsWinRT's `DefaultComWrappers`. | Core, `Microsoft.Windows.CsWinRT` |
| **Shmuelie.WinRTServer.DependencyInjection** | `ServiceProvider*` factories that resolve server objects from an `IServiceProvider`. | Core, `Microsoft.Extensions.DependencyInjection.Abstractions` |
| **Shmuelie.WinRTServer.SourceGenerator** | The `[ServerClass]` attribute + Roslyn generator for declarative registration. | (install Core too) |
| **Shmuelie.WinRTServer** | Meta-package that references all of the above. | all of the above |

## Choosing packages

- **Just want it to work?** Reference the **`Shmuelie.WinRTServer`** meta-package.
- **Minimizing dependencies?** Start with **Core**, then add:
  - **Lifecycle** to keep the host process running (nearly always needed).
  - **StrategyBased** *or* **CsWinRT** for the `RegisterClass<…>` convenience — pick
    the one matching how your project projects WinRT types
    (see [Getting Started](getting-started.md#choosing-a-comwrappers-flavor)).
  - **DependencyInjection** to resolve objects from a container.
  - **SourceGenerator** for `[ServerClass]` declarative registration.

> [!NOTE]
> The `Shmuelie.WinRTServer.SourceGenerator` package ships the `[ServerClass]`
> attribute and the analyzer, but the code it generates references
> **`Shmuelie.WinRTServer.Core`** types — install Core (or the meta-package)
> alongside it. Its DI output additionally requires the
> **`Shmuelie.WinRTServer.DependencyInjection`** package.

## Assembly names

The **Core** package ships `Shmuelie.WinRTServer.dll`, and every package places
its public types in the `Shmuelie.WinRTServer` (or `Shmuelie.WinRTServer.CsWinRT`
/ `Shmuelie.WinRTServer.StrategyBased`) namespace. Only the *package ids* differ —
type names are stable regardless of how the library is split.
