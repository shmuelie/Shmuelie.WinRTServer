![Out-of-Process WinRT/COM Server](https://raw.githubusercontent.com/shmuelie/Shmuelie.WinRTServer/main/Shmuelie.WinRTServer.Title.png)
==================================================================

[![.NET](https://github.com/Shmuelie/Shmuelie.WinRTServer/workflows/.NET/badge.svg)][1] [![NuGet](https://img.shields.io/nuget/dt/Shmuelie.WinRTServer.svg)][2] [![NuGet](https://img.shields.io/nuget/vpre/Shmuelie.WinRTServer.svg)][3]

# What is it?

The Component Object Model (COM) API that underlines .NET and the Windows
Runtime supports the concept of Out Of Process (OOP) Servers. This allows for
using objects that are in a different process (or even a different machine) as
though they were in the local process. This library adds APIs to make the
process of creating the "server" in .NET much easier.

> **Note**: COM and Windows Runtime are Windows only.

# Why?

## Cross Language

Because this uses COM/WinRT for the communication any language that can use COM
can use this library. As an example of this, the sample includes a simple C++
console application that talks to the .NET/C# server.

## Complex Types

Most RPC/ICP systems are just sending messages between the two processes. At
most they can serialize an object graph. COM allows for more complicated
objects, where the returned types can have methods, events, and properties. If
you could do it with a local object, you can do it with a remote object. The
samples include using most of these abilities, including having the remote
process load a file as a stream and having the local process use the stream
without having to send the whole file across.

## Built in Support for Types

Many of the types you are used to using are supported out of the box like
collection types, map types, streams, etc. This allows you to not have to worry
about how the IPC works.

## Security

COM provides various ways to secure usage and creation of objects. For more
details, see [Security in COM][7].

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
3. Event delegates must be either `TypedEventHandler<TSender, TResult>` or
   `EventHandler<TResult>` instead of `EventHandler` and `EventHandler<T>`.
4. Types in method parameters, type parameters, and return types must be:

   - A blittable type.
   - An interface that has a [.NET/WinRT Mapping][4].
   - A WinRT type.
   - Another interface in the project.

5. Methods, properties, and events are all supported.

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
should register them with an instance of `COMServer` for COM activation and
`WinRtServer` for WinRT activation. The implementations must have a GUID using
the `System.Runtime.InteropServices.GuidAttribute` attribute.

Because the interfaces must use the WinRT asynchronous types instead of the .NET
ones, the implementation will likely need to use `AsyncInfo` to help adapt
between the two systems.

## Client Project

A client can be both full trust applications (Win32, WPF, WinForms, etc) or a
UWP app.

A UWP client cannot use WinRT activation and must use COM style activation. The
UWP sample app shows how to do this. To understand the details behind it, see
[this blog post][6].

A full trust client can use WinRT activation, which allows you to create the
remote instances simply by `new SomeType()`, like you would for any other type.
The sample WPF application shows this in action (using with WinForms would be
similar).

# Package Versions and Releases

Pre-release and stable packages use the same package ID,
`Shmuelie.WinRTServer`. Pre-release versions follow SemVer 2.0, for example
`2.3.0-preview.1`, `2.3.0-beta.2`, and `2.3.0-rc.1`. These versions are
illustrative; check the package feed for available versions.

## Build a Package Locally

Use a Visual Studio developer PowerShell with MSBuild 17.8 or later, the .NET 8
SDK, and the Windows SDK installed. Packages are written to `artifacts`.

```powershell
# Pack an explicitly versioned preview.
msbuild .\src\Shmuelie.WinRTServer\Shmuelie.WinRTServer.csproj -restore '-t:Build;Pack' -p:Configuration=Release '-p:Version=2.3.0-preview.1' '-bl:artifacts\local-{}.binlog'

# Equivalent prefix/suffix inputs.
msbuild .\src\Shmuelie.WinRTServer\Shmuelie.WinRTServer.csproj -restore '-t:Build;Pack' -p:Configuration=Release '-p:VersionPrefix=2.3.0' '-p:VersionSuffix=preview.1' '-bl:artifacts\local-{}.binlog'

# An explicit Version without a suffix creates a stable local package.
msbuild .\src\Shmuelie.WinRTServer\Shmuelie.WinRTServer.csproj -restore '-t:Build;Pack' -p:Configuration=Release '-p:Version=2.3.0' '-bl:artifacts\local-{}.binlog'
```

Without explicit inputs, local packages use the base version in
`Directory.Build.props` with an `alpha` suffix. Providing only `VersionPrefix`
retains that suffix; providing only `VersionSuffix` uses the repository's base
version. Use `Version` to request a stable package, not an empty suffix.
Conflicting inputs fail rather than silently selecting a different version.
Do not override `PackageVersion`, `AssemblyVersion`, `FileVersion`, or
`InformationalVersion` independently.

Pre-release identifiers may contain ASCII letters, digits, and hyphens, separated
by dots. Numeric identifiers cannot have leading zeroes. Version components must
be between 0 and 65534 so the assembly and file versions can use
`major.minor.patch.0`. Optional SemVer build metadata is preserved in package
metadata, but is not part of the NuGet package filename or its unique identity.

## Publish from CI

CI derives versions from its build context and rejects conflicting version
overrides:

| Context | Package identity |
| --- | --- |
| Ordinary push | `<base>-alpha.<run-id>` |
| Pull request | `<base>-pr.<run-id>` |
| Push to branch `rel/2.3.0-rc.1` | `2.3.0-rc.1` |
| Push to branch `rel/2.3.0` | `2.3.0` |

The commit SHA is included as build metadata. Re-running a workflow retains the
same version; a new nightly run receives a new run ID. PRs cannot become stable
releases even if their branch is named `rel/2.3.0`. Tags do not select release
versions. Invalid release-branch versions fail instead of falling back to a
nightly version.

To release a preview, push the intended commit to a branch such as
`rel/2.3.0-preview.1`. After validation, release candidates and the stable release
use new versions, for example `rel/2.3.0-rc.1` and then `rel/2.3.0`.
Never replace an already published version: increment the pre-release identifier
or the stable version when publishing changed contents. Commit metadata alone
does not create a new package identity.

The workflow checks version rules, package/assembly contents, and stable versus
pre-release consumption, as well as the existing Debug/Release solution builds.
Only push builds publish, and they publish the verified artifact without
rebuilding it. Both stable and pre-release packages currently go to
[GitHub Packages](https://github.com/shmuelie/Shmuelie.WinRTServer/packages).
NuGet.org publishing is tracked separately in
[#42](https://github.com/shmuelie/Shmuelie.WinRTServer/issues/42).

## Consume Pre-release Packages

Configure the GitHub NuGet feed using your own credentials. For example, on
Windows, use a personal access token with `read:packages` in an environment
variable; NuGet encrypts the saved password for the current user. Do not commit
credentials to the repository.

```powershell
dotnet nuget add source https://nuget.pkg.github.com/shmuelie/index.json --name shmuelie-github --username YOUR_GITHUB_USERNAME --password $env:GITHUB_PACKAGES_TOKEN

# Install a specific preview.
dotnet add .\MyApp.csproj package Shmuelie.WinRTServer --version 2.3.0-rc.1 --source https://nuget.pkg.github.com/shmuelie/index.json

# Or opt into the latest available pre-release.
dotnet add .\MyApp.csproj package Shmuelie.WinRTServer --prerelease --source https://nuget.pkg.github.com/shmuelie/index.json
```

In Visual Studio's NuGet UI, select the configured feed and enable **Include
prerelease**. Without this option (or the CLI's `--prerelease`), normal latest-version
selection remains stable-only; existing pinned package references are unchanged.

## Validate Versioning

From the repository root in PowerShell 7 with MSBuild on `PATH`:

```powershell
# Fast version-resolution and invalid-input checks.
.\tests\Versioning\Test-Versioning.ps1

# Also build packages, inspect their metadata/assemblies, and restore consumers
# from an isolated local feed to check pre-release opt-in.
.\tests\Versioning\Test-Versioning.ps1 -Packages
```

The package checks restore dependencies from NuGet.org; an alternative public
source can be supplied with `-RestoreSource`. Tests never publish packages.
Per-run binary logs and artifacts are retained under `artifacts\versioning`.

# Sample

To help understand usage and show what can be done samples can be found under
the tests folder. The sample has:

- .NET 8 Server
- UWP .NET Client App
- C++/WinRT Console Client App
- WPF .NET Framework Client App
- WPF .NET 8 Client App

Sample builds require Windows SDK **10.0.26100.0**, Visual Studio's C++ tools,
and UWP development support (including the .NET Native toolchain for Release).
The build SDK is pinned in `tests\Directory.Build.props`; it supplies native
headers/tools, UWP references, and C#/WinRT projection metadata. This does not
change the library's `net8.0-windows10.0.22000.0` target framework or the samples'
minimum Windows versions.

Run `.\tests\Test-WindowsSdk.ps1` in PowerShell 7 to check the SDK's metadata,
headers, libraries, and tools before building. CI runs this check before restore
and retains both Debug and Release solution builds as publishing gates. If the
SDK is missing, install it through Visual Studio Installer instead of overriding
`TargetPlatformVersion` globally or reinstalling the obsolete 22000 SDK.

Build the solution so its dependency ordering generates the interface and proxy
WinMDs before the clients consume them. The clients reference the explicit output
filenames so a clean checkout does not require a preliminary build.
Managed x86 clients consume the native Metadata project's `Win32` outputs;
their own platform remains x86. Full solution CI and packaged-server validation
use x64: the existing .NET 8 Native AOT server configuration does not support
`win-x86`, and its full x86 packaging path is outside this pre-release change.

# Troubleshooting

If you are having issues, check on these things:

- Make sure the client app includes the WinMDs (Interface and Metadata)
- Make sure that the interface project uses the
   `Windows.Foundation.Metadata.GuidAttribute` attribute, not the
   `System.Runtime.InteropServices.GuidAttribute` attribute.
- Make sure that the server project uses the
  `System.Runtime.InteropServices.GuidAttribute` attribute, not the
  `Windows.Foundation.Metadata.GuidAttribute` attribute.

[1]: https://github.com/Shmuelie/Shmuelie.WinRTServer/actions
[2]: https://www.nuget.org/stats/packages/Shmuelie.WinRTServer?groupby=Version
[3]: https://www.nuget.org/packages/Shmuelie.WinRTServer/
[4]: https://learn.microsoft.com/en-us/windows/apps/develop/platform/csharp-winrt/net-mappings-of-winrt-types
[5]: https://learn.microsoft.com/en-us/uwp/midl-3/
[6]: https://devblogs.microsoft.com/ifdef-windows/the-journey-of-moving-from-cpp-winrt-to-csharp-in-the-microsoft-store/
[7]: https://learn.microsoft.com/en-us/windows/win32/com/security-in-com
