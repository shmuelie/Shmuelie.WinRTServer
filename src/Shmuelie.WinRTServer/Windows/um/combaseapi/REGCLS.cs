using System;

namespace Shmuelie.Interop.Windows;

/// <summary>
/// Controls the type of connections to a class object.
/// </summary>
/// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/combaseapi/ne-combaseapi-regcls">REGCLS enumeration (combaseapi.h)</seealso>
[Flags]
internal enum REGCLS
{
    /// <summary>
    /// Multiple applications can connect to the class object through calls to
    /// <c>CoGetClassObject</c>. If both the <see cref="REGCLS_MULTIPLEUSE"/>
    /// and <see cref="CLSCTX.CLSCTX_LOCAL_SERVER"/> are set in a call to <see
    /// cref="ComBaseAPI.CoRegisterClassObject(Guid*, IUnknown*, uint, uint,
    /// uint*)"/>, the class object is also automatically registered as an
    /// in-process server, whether <c>CLSCTX_INPROC_SERVER</c> is explicitly
    /// set.
    /// </summary>
    REGCLS_MULTIPLEUSE = 1,
    /// <summary>
    /// Suspends registration and activation requests for the specified CLSID
    /// until there is a call to <see cref="ComBaseAPI.CoResumeClassObjects"/>.
    /// This is used typically to register the CLSIDs for servers that can
    /// register multiple class objects to reduce the overall registration time,
    /// and thus the server application startup time, by making a single call to
    /// the SCM, no matter how many CLSIDs are registered for the server.
    /// </summary>
    REGCLS_SUSPENDED = 4,
}
