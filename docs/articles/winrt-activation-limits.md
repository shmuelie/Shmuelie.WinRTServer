# WinRT activation limits

WinRT activation is the convenient path — a full-trust client writes
`new SomeType()` and the runtime activates your out-of-process server. But it is
not available everywhere, which is why this library ships **both**
[`WinRtServer`](xref:Shmuelie.WinRTServer.WinRtServer) and
[`ComServer`](xref:Shmuelie.WinRTServer.ComServer).

## UWP clients cannot use WinRT activation

A UWP (AppContainer) process cannot activate an arbitrary out-of-process WinRT
class the way a full-trust process can. To consume your server from UWP you must
fall back to **classic COM activation**: register the object with
`ComServer` (which uses `CoRegisterClassObject`) and activate it by CLSID on the
client.

The UWP sample in the repository shows the full pattern. For the background on
why this is necessary and how the Microsoft Store team solved the same problem,
see [this blog post][blog].

## Rule of thumb

| Client | Activation | Server class |
| --- | --- | --- |
| Win32 / WPF / WinForms (full trust) | WinRT (`new T()`) **or** COM | either |
| C++/WinRT (full trust) | WinRT **or** COM | either |
| UWP | COM only | [`ComServer`](xref:Shmuelie.WinRTServer.ComServer) |

If you need to support UWP clients at all, register with `ComServer`. You can
run both servers in the same process if you have a mix of clients.

## Practical consequences

- **You still need the metadata WinMD.** Even COM-activating clients need the
  proxy type metadata to marshal calls — see [Getting Started](getting-started.md).
- **CLSIDs matter for COM.** With `ComServer` the client activates a specific
  CLSID, so the implementation's
  `System.Runtime.InteropServices.GuidAttribute` must match what the client asks
  for.
- **Agility.** `ComServer` registers class objects with `REGCLS_AGILE`, so calls
  from UWP clients are dispatched without STA marshaling surprises. See
  [How COM OOP server creation works](how-com-oop-works.md).

[blog]: https://devblogs.microsoft.com/ifdef-windows/the-journey-of-moving-from-cpp-winrt-to-csharp-in-the-microsoft-store/
