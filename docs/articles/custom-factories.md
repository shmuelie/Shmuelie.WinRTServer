# Creating custom factories

The `RegisterClass<...>` extensions cover the common cases by creating
`General*` (reflection) or `Delegate*` (`Func<T>`) factories. When you need full
control over how an object is created — pooling, argument inspection, custom
CLSID/IID selection, logging — implement a factory yourself.

## COM class factories

Derive from
[`BaseClassFactory`](xref:Shmuelie.WinRTServer.BaseClassFactory) and provide:

- [`Clsid`](xref:Shmuelie.WinRTServer.BaseClassFactory.Clsid) — the class id
  clients activate.
- [`Iids`](xref:Shmuelie.WinRTServer.BaseClassFactory.Iids) — every interface id
  the created object may be requested as. Returning more than one is how a single
  class supports multiple interfaces.
- `CreateInstance()` — returns a new managed object.

```csharp
using System;
using System.Collections.Generic;
using Shmuelie.WinRTServer;

public sealed class PooledThingFactory : BaseClassFactory
{
    private readonly ObjectPool<RemoteThing> pool;

    public PooledThingFactory(ObjectPool<RemoteThing> pool) => this.pool = pool;

    protected internal override Guid Clsid => typeof(RemoteThing).GUID;

    protected internal override IReadOnlyList<Guid> Iids { get; } =
        [typeof(IRemoteThing).GUID];

    protected internal override object CreateInstance() => pool.Rent();
}
```

Register it like any factory:

```csharp
server.RegisterClassFactory(new PooledThingFactory(pool), comWrappers);
```

`CreateInstance` is called on every activation. The base class raises
[`InstanceCreated`](xref:Shmuelie.WinRTServer.BaseClassFactory.InstanceCreated)
for you.

## WinRT activation factories

Derive from
[`BaseActivationFactory`](xref:Shmuelie.WinRTServer.BaseActivationFactory) and
provide:

- [`ActivatableClassId`](xref:Shmuelie.WinRTServer.BaseActivationFactory.ActivatableClassId) — the type name clients activate with `new`.
- `ActivateInstance()` — returns a new managed object.

```csharp
public sealed class RemoteThingActivationFactory : BaseActivationFactory
{
    public override string ActivatableClassId => "Contoso.Remoting.RemoteThing";

    public override object ActivateInstance() => new RemoteThing();
}
```

```csharp
server.RegisterActivationFactory(new RemoteThingActivationFactory(), comWrappers);
```

## Built-in factory reference

| Factory | Creates instances via |
| --- | --- |
| [`GeneralClassFactory<T, …>`](xref:Shmuelie.WinRTServer.GeneralClassFactory`2) | `new T()` (reflection) |
| [`DelegateClassFactory<T, …>`](xref:Shmuelie.WinRTServer.DelegateClassFactory`2) | a supplied `Func<T>` |
| [`ServiceProviderClassFactory<T, …>`](xref:Shmuelie.WinRTServer.ServiceProviderClassFactory`2) | an `IServiceProvider` |
| [`GeneralActivationFactory<T>`](xref:Shmuelie.WinRTServer.GeneralActivationFactory`1) | `new T()` (reflection) |
| [`DelegateActivationFactory<T>`](xref:Shmuelie.WinRTServer.DelegateActivationFactory`1) | a supplied `Func<T>` |
| [`ServiceProviderActivationFactory<T>`](xref:Shmuelie.WinRTServer.ServiceProviderActivationFactory`1) | an `IServiceProvider` |

The class factories all come in 1-, 2-, and 3-interface generic arities so a
single class can be exposed as multiple interfaces.
