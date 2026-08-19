# How COM OOP server creation works

This article explains what actually happens when a client creates an object
that lives in your server process, so the API surface of
`Shmuelie.WinRTServer` makes sense.

## Activation, briefly

"Activation" is how COM turns a **class identifier** into a live object. For an
out-of-process (OOP) server the identifier resolves to a *different* process;
COM launches or connects to that process, asks it for the object, and hands the
client a proxy. Method calls on the proxy are marshaled across the process
boundary.

There are two identifier systems this library supports:

| System | Identifier | Client creates with | Server API |
| --- | --- | --- | --- |
| **COM** | a CLSID (`Guid`) | `CoCreateInstance` / language equivalent | [`ComServer`](xref:Shmuelie.WinRTServer.ComServer) |
| **WinRT** | an activatable class id (the type name) | `new SomeType()` | [`WinRtServer`](xref:Shmuelie.WinRTServer.WinRtServer) |

## The class factory

A server does not register objects; it registers **factories**. When a client
asks COM for a CLSID, COM calls the matching factory's `CreateInstance` (COM) or
`ActivateInstance` (WinRT), which returns a fresh object.

`Shmuelie.WinRTServer` models this with two base types:

- [`BaseClassFactory`](xref:Shmuelie.WinRTServer.BaseClassFactory) — COM class
  factories. It exposes a CLSID and one or more interface IIDs
  ([`Iids`](xref:Shmuelie.WinRTServer.BaseClassFactory.Iids)), so a single class
  can be requested as several interfaces.
- [`BaseActivationFactory`](xref:Shmuelie.WinRTServer.BaseActivationFactory) —
  WinRT activation factories, keyed by activatable class id.

You rarely implement these directly — the `RegisterClass<...>` extension methods
create `General*` (reflection) or `Delegate*` (`Func<T>`) factories for you. See
[Creating custom factories](custom-factories.md) when you need more control.

## Registration lifecycle

Both servers follow the same shape:

```csharp
using ComServer server = new();
server.RegisterClass<RemoteThing, IRemoteThing>();
server.Start();   // CoRegisterClassObject / RoRegisterActivationFactories
// ... process stays alive while clients hold objects ...
// Dispose revokes the registrations.
```

- `ComServer` calls `CoRegisterClassObject` with `REGCLS_MULTIPLEUSE`,
  `REGCLS_SUSPENDED`, and `REGCLS_AGILE`, then `CoResumeClassObjects` on
  `Start`. The agile flag lets COM dispatch incoming calls from the MTA without
  marshaling, which avoids unexpected reentrancy when registration happens on an
  STA thread.
- `WinRtServer` calls `RoRegisterActivationFactories`.

## Lifetime is your responsibility

The servers **do not** track how many objects are alive. They raise
[`InstanceCreated`](xref:Shmuelie.WinRTServer.IServer.InstanceCreated) and
otherwise stay out of the way. To keep the process running until the last client
releases its objects, attach an external
[`IServerLifetime`](xref:Shmuelie.WinRTServer.IServerLifetime) helper — see
[Object lifetime & dependency injection](lifetime-and-di.md).

This separation is deliberate: it lets you pick a tracking strategy
(GC-polling vs. deterministic reference counting) or implement your own, without
the server forcing a policy on you.

## Why the extra projects?

Clients need *metadata* describing the proxy types. Today that metadata is
produced by a small C++/WinRT project from IDL. The contract interfaces live in
a separate C# WinMD so both the server and the metadata project can share them.
See [Getting Started](getting-started.md) for the concrete project layout.
