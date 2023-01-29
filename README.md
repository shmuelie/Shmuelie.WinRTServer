![Out-of-Process WinRT/COM Server](Shmuelie.WinRTServer.Title.png)
==================================================================

[![.NET](https://github.com/Shmuelie/Shmuelie.WinRTServer/workflows/.NET/badge.svg)][1] [![NuGet](https://img.shields.io/nuget/dt/Shmuelie.WinRTServer.svg)][2] [![NuGet](https://img.shields.io/nuget/vpre/Shmuelie.WinRTServer.svg)][3]

# What is it?

The Component Object Model (COM) API that underlines .NET and the Windows
Runtime supports the concept of Out Of Process (OOP) Servers. This allows for
using objects that are in a different process (or even a different machine) as
though they were in the local process. This library adds APIs to make the
process of creating the "server" in .NET much easier.

> **Note**: COM and Windows Runtime are Windows only.

# Usage

Currently to create an Out-of-Process server requires the C++/WinRT tooling
(though no actual C++ code) and a "contract" project. These two limitations will
be removed in a future version of the library.

## Contract Project

The contract project is a C# project that contains the interfaces of the remote
objects. Output is a WinMD that is referenced by the other projects. The interfaces have some rules:

1. The interface must have a GUID assigned using the
   `Windows.Foundation.Metadata.GuidAttribute` attribute, not the
   `System.Runtime.InteropServices.GuidAttribute` attribute.
2. Asynchronous methods must use the WinRT types (`IAsyncAction`,
   `IAsyncActionWithProgress<TProgress>`, `IAsyncOperation<TResult>`,
   `IAsyncOperationWithProgress<TResult, TProgress>`) instead of `Task` and
   `Task<T>`.
3. Types in method parameters, type parameters, and return types must be:

   - A blittable type.
   - An interface that has a [.NET/WinRT Mapping][4].
   - A WinRT type.
   - Another interface in the project.

4. Methods, properties, and events are all supported.

## Metadata Project

The metadata project is a C++/WinRT project that uses [MIDL 3.0][5] to create
proxy types in a WinMD that can be referenced by the client of the OOP Server.
No actual C++ code is needed, only the IDL.

The IDL is very simple, only needing `runtimeclass`es that implement the
interface from the contract project. Unlike in C#, in MIDL 3.0 the type
automatically has the members from the interface so they do not need to be
listed again. Importantly the `runtimeclass` must have an empty constructor,
otherwise the proxy type cannot be created.

> :exclamation:**Important**: Because of the mix of SDK Style and C++/WinRT,
> `nuget restore` is needed to restore for C++/WinRT. In addition
> `<RestoreProjectStyle>Packages.config</RestoreProjectStyle>` is needed in the
> C++ project file.

## Server Project

The server project is the only project that references `Shmuelie.WinRTServer`.
It will contain implementations of the interfaces from the contract and when run
should register them with an instance of `COMServer`. The implementations must
have a GUID using the `System.Runtime.InteropServices.GuidAttribute` attribute.

Because the interfaces must use the WinRT asynchronous types instead of the .NET
ones, the implementation will likely need to use `AsyncInfo` to help adapt
between the two systems.

## Client Project

An application to use the server project, using the metadata project to generate
the proxy information. If the client is a UWP project, see [this blog post][6]
for how to create the instances.

# Sample

To help understand usage, a sample using a UWP client app is included under the
test directory. Simply run the `Shmuelie.WinRTServer.Sample.Package` project to
see it in action.

> **Note**: If Visual Studio fails to build the Metadata project restarting
> Visual Studio should fix the problem.

# Alternatives

If both applications are full trust applications other RPC/IPC technology might
be easier to use. The main advantages of using COM/WinRT are that any language
that can use COM can be the client and the runtime handles how to
serialize/marshal types.

If the client is a UWP application, the options are more limited. AppServices
from the Community toolkit is easier to use but does not support more complex
scenarios that COM/WinRT OOP does like events, properties, and complex object
graphs.

[1]: https://github.com/Shmuelie/Shmuelie.WinRTServer/actions
[2]: https://www.nuget.org/stats/packages/Shmuelie.WinRTServer?groupby=Version
[3]: https://www.nuget.org/packages/Shmuelie.WinRTServer/
[4]: https://learn.microsoft.com/en-us/windows/apps/develop/platform/csharp-winrt/net-mappings-of-winrt-types
[5]: https://learn.microsoft.com/en-us/uwp/midl-3/
[6]: https://devblogs.microsoft.com/ifdef-windows/the-journey-of-moving-from-cpp-winrt-to-csharp-in-the-microsoft-store/
