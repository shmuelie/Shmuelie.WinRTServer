# Using interfaces defined elsewhere

The [Getting Started](getting-started.md) walkthrough assumes you author the
contract interfaces yourself. But you can just as easily implement interfaces
**defined by someone else** — for example a WinRT interface from the Windows SDK
or the Windows App SDK — and expose your implementation out of process.

## The key idea

`Shmuelie.WinRTServer` never requires that *you* declared the interface. A class
factory only needs:

- a **CLSID** (for `ComServer`) or an **activatable class id** (for
  `WinRtServer`), and
- the **IID(s)** the object can be requested as.

Those IIDs can belong to any WinRT/COM interface your implementation satisfies,
regardless of who defined it.

## Implementing a pre-defined WinRT interface

Suppose you want to expose an object that implements an interface `IExisting`
that already ships in a referenced WinMD. Implement it as usual, give the
implementation a CLSID, and register the existing interface:

```csharp
using System.Runtime.InteropServices;
using Shmuelie.WinRTServer;
using Some.External.Namespace;   // where IExisting is defined

[Guid("00000000-0000-0000-0000-000000000001")]
public sealed partial class MyThing : IExisting
{
    // implement the members IExisting declares
}
```

```csharp
server.RegisterClass<MyThing, IExisting>();
```

`typeof(IExisting).GUID` supplies the IID, so the factory advertises the
externally-defined interface. Clients that already know `IExisting` can consume
your object without you shipping a contract WinMD for that interface at all.

## When you still need a metadata project

- If clients activate your class **by name** (WinRT activation) or need proxy
  metadata for marshaling, you still provide a metadata WinMD whose
  `runtimeclass` lists the external interface as its default interface — see
  [Getting Started](getting-started.md).
- If the external interface's metadata is already available to the client (it
  ships in the SDK/WinAppSDK the client references), you only need metadata for
  *your* runtime class, not for the interface.

## Custom IID selection

If you need finer control than `typeof(T).GUID` — for instance to expose a
class under an interface whose managed projection doesn't surface the GUID you
want — implement a [custom factory](custom-factories.md) and return the exact
IIDs from [`Iids`](xref:Shmuelie.WinRTServer.BaseClassFactory.Iids).
