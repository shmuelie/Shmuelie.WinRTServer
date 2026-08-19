# Shmuelie.WinRTServer

A single NuGet library (`Shmuelie.WinRTServer`) that makes it easy to write an
Out-of-Process (OOP) COM/WinRT server in .NET, so objects in one process can be
used from another process (or language) as if local. Windows only.

## Build, test, and run

The solution mixes SDK-style, legacy, UWP, C++/WinRT, and `.wapproj` projects,
so **the full solution must be built with MSBuild, not `dotnet build`**, and
only the `x64`/`x86` platforms exist (there is no `Any CPU` solution config).
The solution file is `Shmuelie.WinRTServer.slnx` (XML `.slnx` format).
C++/WinRT projects require a real `nuget restore` first.

- Restore + build the whole solution (mirrors CI):
  `nuget restore` then
  `msbuild /t:restore;build /p:Configuration=Debug /p:Platform=x64`
  (bare `nuget restore` / `msbuild` auto-discover the `.slnx`). A clean
  from-scratch build may need to be run **twice** to converge: the C++/WinRT
  `samples/Shmuelie.WinRTServer.Sample.Metadata` project emits a WinMD the other
  samples consume, and it isn't always ready on the first pass (this is the
  sample-build fragility the README documents — rerun the build, or rebuild in
  Visual Studio). The `src/` library and `tests/` projects are unaffected and
  build with plain `dotnet`.
- Build just the core library (SDK-style, so `dotnet` also works):
  `dotnet build .\src\Shmuelie.WinRTServer.Core\Shmuelie.WinRTServer.Core.csproj -c Release`
- Pack the NuGets: `dotnet pack` each `src/` package project (Core, Lifecycle,
  StrategyBased, CsWinRT, DependencyInjection, SourceGenerator, Meta); output
  goes to `artifacts/` (`PackageOutputPath`).
- Unit tests live under `tests/` (xUnit): `Shmuelie.WinRTServer.Tests`
  (net10.0-windows, exercises the library) and `Shmuelie.WinRTServer.SourceGenerator.Tests`
  (net10.0, Roslyn-driver tests for the generator). Run with
  `dotnet test .\tests\Shmuelie.WinRTServer.Tests\Shmuelie.WinRTServer.Tests.csproj`
  (COM-activation round-trips are intentionally not covered here). The runnable
  *sample* projects (server + UWP/WPF/C++ clients) moved to `samples/` and
  validate behavior manually. `TreatWarningsAsErrors` is on, so a clean build is
  also a correctness gate.

SDK is pinned to 10.0.302 (`global.json`, `rollForward: latestFeature`), and
`MSBuild.Sdk.Extras` 3.0.44 is used for some sample project types. The library
targets `net10.0-windows10.0.26100.0`; the C++ projects use the v145 toolset.

## Architecture

The library is split into one project per NuGet package under `src/`, all with
assembly/namespace root `Shmuelie.WinRTServer` (the package **ids** differ):

- `Shmuelie.WinRTServer.Core` (assembly `Shmuelie.WinRTServer.dll`, package
  `Shmuelie.WinRTServer.Core`) — the servers, factories, options, security,
  interop. No package dependencies.
- `Shmuelie.WinRTServer.Lifecycle` — `IServerLifetime` helpers.
- `Shmuelie.WinRTServer.StrategyBased` / `.CsWinRT` — the `RegisterClass<…>`
  extension namespaces (depend on Core; CsWinRT also on `Microsoft.Windows.CsWinRT`).
- `Shmuelie.WinRTServer.DependencyInjection` — the `ServiceProvider*` factories.
- `Shmuelie.WinRTServer.Annotations` (not packed on its own) + `.SourceGenerator`
  (packable analyzer package that also ships the annotations lib).
- `Shmuelie.WinRTServer.Meta` — produces the `Shmuelie.WinRTServer` meta-package
  depending on all six. Core grants `InternalsVisibleTo` to StrategyBased,
  CsWinRT, and DependencyInjection (their extensions/factories touch
  `BaseClassFactory`'s `protected internal` members).

The two server classes (`Core`), both `IDisposable`, non-thread-safe, follow the
same lifecycle (`Register... → Start()`, then dispose when done):

- `ComServer` — classic COM activation (`CoRegisterClassObject`, registered
  with `REGCLS_AGILE`). Register by `(implementation, interface)` pair keyed on
  a CLSID `Guid`. Works for UWP clients, which cannot use WinRT activation.
- `WinRtServer` — WinRT activation (`RoRegisterActivationFactories`). Register
  by activatable class ID (the type name). Lets full-trust clients create remote
  objects with plain `new SomeType()`.

The servers do **not** track object lifetime themselves; they just raise
`InstanceCreated`. Object-lifetime tracking is opt-in and external (Lifecycle
package), via the `IServerLifetime` helpers: `PollingServerLifetime`
(GC/`WeakReference` polling) or `ReferenceCountedServerLifetime` (deterministic,
via an `IIUnknownStrategy` composed over `StrategyBasedComWrappers`). Both expose
`WaitForFirstObjectAsync` and `WaitUntilEmptyAsync`. A server-property surface
(`ServerOptions` / `ServerRuntimeOptions`) configures `IGlobalOptions`, and
`ServerSecurity` wraps `CoInitializeSecurity` / caller impersonation.

Registration is done through `BaseClassFactory` / `BaseActivationFactory`
subclasses. `General*` factories create instances reflectively; `Delegate*`
factories use a supplied `Func<T>`; `ServiceProvider*` factories (DI package)
resolve from an `IServiceProvider`. Class factories support multiple interfaces
per class (`BaseClassFactory.Iids`), with 2-/3-interface `RegisterClass<T,...>`
overloads.

Declarative registration is available via the annotations + source-generator
projects: `Shmuelie.WinRTServer.Annotations` (the `[ServerClass]` attribute) and
`Shmuelie.WinRTServer.SourceGenerator` (a Roslyn incremental generator, shipped
together in the `Shmuelie.WinRTServer.SourceGenerator` package) which emits
COM/WinRT registration and optional `Microsoft.Extensions.DependencyInjection`
helpers (the DI output is gated on the DependencyInjection package being
referenced).

Native interop lives in `Internal/Windows/` and is generated by **CsWin32**
(`Microsoft.Windows.CsWin32`) from `NativeMethods.txt` (API list) and
`NativeMethods.json` (options) — add a Win32 API by adding its name to
`NativeMethods.txt`, don't hand-write P/Invoke. The library is
`IsAotCompatible`, uses `DisableRuntimeMarshalling`, and relies on
`ComWrappers` source-generated marshalling (`[GeneratedComInterface]` /
`StrategyBasedComWrappers`), not `ComImport`.

## ComWrappers flavors (important for consumers)

`RegisterClass<...>` extension methods exist in **two namespaces**, differing
only by which `ComWrappers` they pass in — pick based on how the consumer
projects WinRT types:

- `Shmuelie.WinRTServer.CsWinRT` — uses CsWinRT's `DefaultComWrappers`
  (for projects that reference `Microsoft.Windows.CsWinRT`).
- `Shmuelie.WinRTServer.StrategyBased` — uses the runtime
  `StrategyBasedComWrappers` (source-generated COM interop).

## Contract/implementation conventions

The three-project consumer model (contract WinMD → C++/WinRT metadata WinMD →
server) is documented in the docs site (`docs/articles/getting-started.md`); the
`samples/` projects are the reference implementation. Key rules that are easy to get
wrong:

- **Contract interfaces** use `Windows.Foundation.Metadata.GuidAttribute`;
  **server implementations** use `System.Runtime.InteropServices.GuidAttribute`.
  Mixing these up is the most common failure.
- Interface async methods must use WinRT async types (`IAsyncAction`,
  `IAsyncOperation<T>`, `...WithProgress<...>`), not `Task`/`Task<T>`. Events
  must be `TypedEventHandler<,>` or `EventHandler<T>`.
- Implementations typically expose a normal `Task`-based public method plus an
  explicit-interface member (marked `[DebuggerNonUserCode]`) that adapts it to
  the WinRT type via `System.Runtime.InteropServices.WindowsRuntime.AsyncInfo.Run`.
  See `samples/Shmuelie.WinRTServer.Sample.ServerNet6/RemoteThing.cs`.

## Versioning / release

Do **not** hardcode versions in the csproj. The package/assembly version comes
from `ShmuelieWinRTServerPackageVersion` in the root `Directory.Build.props`;
bump it there before a release. Pushing a branch named `rel/<x.y.z>` (optionally
`-<suffix>`) triggers a real release build with that version; all other CI
builds get an `alpha`/`pr` suffix plus the run id.

## Code style

Root `Directory.Build.props` sets `Nullable enable`, `LangVersion 13`,
`AllowUnsafeBlocks`, `TreatWarningsAsErrors`, and `AnalysisLevel latest-all`.
Public APIs are expected to have XML doc comments (`GenerateDocumentationFile`
is on). `.editorconfig` disables only a couple of analyzer rules (e.g. CA1708).
