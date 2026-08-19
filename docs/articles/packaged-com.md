# Packaged COM

COM servers are traditionally registered by writing to the registry (the
`HKCR\CLSID\{…}\LocalServer32` keys). An MSIX-packaged app cannot write there;
instead it declares its servers **inside the package manifest** using the
[`com`][comext] and [`windows.activatableClass`][winrtext] extensions. Windows
reads those declarations from a per-package "packaged COM" catalog, so the
registration is isolated to your app and removed cleanly on uninstall.

## Why use it

- No machine-wide registry pollution; registration is per-package.
- Clean install/uninstall and servicing.
- Required if you ship your server through the Microsoft Store or as MSIX.

The repository's `.wapproj` packaging project demonstrates this end to end.

## Declaring a COM server

In `Package.appxmanifest`, under the executable's `<Application>` (or an
`<Extension>` with `Category="windows.comServer"`), declare an
`<ExeServer>` and the `<Class>` entries for each CLSID your
[`ComServer`](xref:Shmuelie.WinRTServer.ComServer) registers:

```xml
<Extensions>
  <com:Extension Category="windows.comServer">
    <com:ComServer>
      <com:ExeServer Executable="Server\MyServer.exe" DisplayName="My Server">
        <com:Class Id="474527DE-81CD-466E-ADCF-6E3809CD5033" />
      </com:ExeServer>
    </com:ComServer>
  </com:Extension>
</Extensions>
```

The `Id` is the CLSID your implementation carries via
`System.Runtime.InteropServices.GuidAttribute`, and it must match what the
client activates.

## Declaring a WinRT activatable class

For [`WinRtServer`](xref:Shmuelie.WinRTServer.WinRtServer) activation, declare
the activatable class and its out-of-process server host:

```xml
<Extensions>
  <com:Extension Category="windows.comServer">
    <com:ComServer>
      <com:ExeServer Executable="Server\MyServer.exe" DisplayName="My Server">
        <com:Class Id="…" />
      </com:ExeServer>
    </com:ComServer>
  </com:Extension>
  <uap3:Extension Category="windows.activatableClass.outOfProcessServer">
    <uap3:OutOfProcessServer ServerName="MyServer">
      <uap3:Class ClassId="Contoso.Remoting.RemoteThing" />
    </uap3:OutOfProcessServer>
  </uap3:Extension>
</Extensions>
```

The `ClassId` is the activatable class id — the full type name your activation
factory returns.

## Notes

- The server executable is packaged *inside* your MSIX; the `Executable` path is
  relative to the package root.
- Because the registration is package-local, only apps that can see the package
  (your own, or others you explicitly share with) can activate the server.
- For the project plumbing (`.wapproj`, `nuget restore`, the C++/WinRT metadata
  project), see [Getting Started](getting-started.md).

[comext]: https://learn.microsoft.com/uwp/schemas/appxpackage/uapmanifestschema/element-com-comserver
[winrtext]: https://learn.microsoft.com/uwp/schemas/appxpackage/uapmanifestschema/element-uap3-outofprocessserver
