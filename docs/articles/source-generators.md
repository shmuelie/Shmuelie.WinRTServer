# Declarative registration & source generators

Writing `RegisterClass<...>` calls by hand is fine for a handful of types, but
`Shmuelie.WinRTServer` ships a source generator that can emit the registration
for you from a simple attribute.

## The `[ServerClass]` attribute

The [`ServerClassAttribute`](xref:Shmuelie.WinRTServer.ServerClassAttribute) and
the generator ship together in the **`Shmuelie.WinRTServer.SourceGenerator`**
package. Add that package (or the `Shmuelie.WinRTServer` meta-package) alongside
`Shmuelie.WinRTServer.Core` — the generated code references Core types. Apply the
attribute to an implementation class and list the COM interfaces it should be
registered for:

```csharp
using System.Runtime.InteropServices;
using Shmuelie.WinRTServer;

[Guid("474527DE-81CD-466E-ADCF-6E3809CD5033")]
[ServerClass(typeof(IRemoteThing))]
public sealed partial class RemoteThing : IRemoteThing
{
    // ...
}
```

- The interfaces you pass drive **COM** class-factory registration.
- **WinRT** activation registration is always generated using the class name as
  the activatable class id.
- Set [`Lifetime`](xref:Shmuelie.WinRTServer.ServerClassAttribute) to
  `ServerObjectLifetime.Singleton` or `Transient` to control how the class is
  registered with a DI container (see below).

## Generated registration

The generator emits a `GeneratedServerRegistration` class with extension methods
that register every annotated type at once:

```csharp
using Shmuelie.WinRTServer.Generated;

using ComServer server = new();
server.RegisterGeneratedClasses(comWrappers);   // all [ServerClass] types
server.Start();
```

The same extension is generated for
[`WinRtServer`](xref:Shmuelie.WinRTServer.WinRtServer). Because the code is
generated at compile time (no reflection), it is trimming- and AOT-friendly.

## Generated DI helpers

When your project also references the
**`Shmuelie.WinRTServer.DependencyInjection`** package and
`Microsoft.Extensions.DependencyInjection`, the generator emits a
`GeneratedServerServices` class as well:

```csharp
using Shmuelie.WinRTServer.Generated;

ServiceProvider provider = new ServiceCollection()
    .AddServerObjects()          // registers each [ServerClass] with its Lifetime
    .BuildServiceProvider();

using ComServer server = new();
server.RegisterGeneratedClasses(provider, comWrappers);  // resolve from DI
server.Start();
```

`AddServerObjects` adds each annotated type using the lifetime from its
attribute, and the provider-based `RegisterGeneratedClasses` wires the server up
to resolve instances from that container.

## Relationship to the CsWinRT generator

There are two generators in play, and they do different jobs:

| Generator | Package | Purpose |
| --- | --- | --- |
| **This library's generator** | `Shmuelie.WinRTServer` (analyzer) | Emits *server registration* code (factories + `RegisterGeneratedClasses` + DI helpers) from `[ServerClass]`. |
| **CsWinRT generator** | `Microsoft.Windows.CsWinRT` | Emits the *WinRT projection* (the `ComWrappers` marshalling glue) for the WinRT types you consume/produce. |

They are complementary. CsWinRT makes your WinRT types callable across the ABI;
this library's generator saves you from writing registration boilerplate. If you
use CsWinRT's projection, register with the
`Shmuelie.WinRTServer.CsWinRT` extension methods; otherwise use
`Shmuelie.WinRTServer.StrategyBased`. See
[Getting Started](getting-started.md#choosing-a-comwrappers-flavor).
