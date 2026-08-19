# Shmuelie.WinRTServer.SourceGenerator

Declarative registration for
[Shmuelie.WinRTServer](https://github.com/shmuelie/Shmuelie.WinRTServer).

This package contains the **`[ServerClass]`** attribute and a Roslyn incremental
**source generator** that emits the COM/WinRT registration for your annotated
implementation classes — no hand-written `RegisterClass<…>` calls needed.

```csharp
using System.Runtime.InteropServices;
using Shmuelie.WinRTServer;

[Guid("…")]
[ServerClass(typeof(IRemoteThing))]
public sealed partial class RemoteThing : IRemoteThing { /* … */ }
```

```csharp
using Shmuelie.WinRTServer.Generated;

using ComServer server = new();
server.RegisterGeneratedClasses(comWrappers);   // every [ServerClass] type
server.Start();
```

When your project also references `Shmuelie.WinRTServer.DependencyInjection` and
`Microsoft.Extensions.DependencyInjection`, the generator additionally emits
`AddServerObjects` and provider-based registration helpers.

> [!NOTE]
> The generated code references types from **`Shmuelie.WinRTServer.Core`**, so
> install Core alongside this package (or use the **`Shmuelie.WinRTServer`**
> meta-package to get everything).

See the [documentation](https://shmuelie.github.io/Shmuelie.WinRTServer/articles/source-generators.html).
