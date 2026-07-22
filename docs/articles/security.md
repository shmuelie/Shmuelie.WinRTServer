# Server security

COM provides a rich security model for controlling who may launch and call your
server, and for letting the server act on behalf of its caller.
`Shmuelie.WinRTServer` surfaces the two pieces you most often need through
[`ServerSecurity`](xref:Shmuelie.WinRTServer.ServerSecurity).

For the full background, see [Security in COM][comsec].

## Initializing process security

Call [`ServerSecurity.Initialize`](xref:Shmuelie.WinRTServer.ServerSecurity.Initialize*)
once, **before** any interface is marshaled (i.e. before `Start`), to set the
process-wide authentication and impersonation levels:

```csharp
using Shmuelie.WinRTServer;

ServerSecurity.Initialize(
    authenticationLevel: ServerAuthenticationLevel.PacketPrivacy,
    impersonationLevel: ServerImpersonationLevel.Identify);

using ComServer server = new();
server.RegisterClass<RemoteThing, IRemoteThing>();
server.Start();
```

This wraps `CoInitializeSecurity` with a null security descriptor (no
access-control restriction). The
[`ServerAuthenticationLevel`](xref:Shmuelie.WinRTServer.ServerAuthenticationLevel)
and
[`ServerImpersonationLevel`](xref:Shmuelie.WinRTServer.ServerImpersonationLevel)
enumerations mirror the RPC `RPC_C_AUTHN_LEVEL` / `RPC_C_IMP_LEVEL` constants.

> [!TIP]
> Choose the **lowest** impersonation level that still lets the server do its
> job. `Identify` lets you inspect the caller; `Impersonate`/`Delegate` grant
> progressively more power and should be used deliberately.

## Impersonating the caller

While servicing an incoming call, you can run code as the client with
[`ServerSecurity.ImpersonateClient`](xref:Shmuelie.WinRTServer.ServerSecurity.ImpersonateClient).
It returns a scope that reverts to the server's own identity when disposed:

```csharp
public void WriteToUserFile(string path, string contents)
{
    using (ServerSecurity.ImpersonateClient())
    {
        // Runs as the calling client, subject to the client's permissions.
        File.WriteAllText(path, contents);
    }
    // Back to the server's identity here.
}
```

This wraps `CoImpersonateClient` / `CoRevertToSelf`. It is only valid inside the
execution of an incoming COM call. Always scope it with `using` so the revert
happens even if the body throws.

## Hosting considerations

Security interacts with your [hosting model](hosting-models.md):

- A **stand-alone, COM-launched** server usually runs *as the caller* already,
  so impersonation is often unnecessary.
- A **Windows Service** runs under a fixed account, so impersonation is how you
  perform work with the caller's rights.

[comsec]: https://learn.microsoft.com/windows/win32/com/security-in-com
