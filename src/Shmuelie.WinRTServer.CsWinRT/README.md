# Shmuelie.WinRTServer.CsWinRT

`RegisterClass<…>` convenience extensions for
[Shmuelie.WinRTServer](https://github.com/shmuelie/Shmuelie.WinRTServer) that use
CsWinRT's **`DefaultComWrappers`**.

Use this package when your project references `Microsoft.Windows.CsWinRT` and
projects its WinRT types through CsWinRT.

```csharp
using Shmuelie.WinRTServer.CsWinRT;

using ComServer server = new();
server.RegisterClass<RemoteThing, IRemoteThing>();
server.Start();
```

The methods live in the `Shmuelie.WinRTServer.CsWinRT` namespace. For the runtime
source-generated equivalent, use `Shmuelie.WinRTServer.StrategyBased` instead.

Depends on `Shmuelie.WinRTServer.Core` and `Microsoft.Windows.CsWinRT`. See the
[documentation](https://shmuelie.github.io/Shmuelie.WinRTServer/articles/getting-started.html).
