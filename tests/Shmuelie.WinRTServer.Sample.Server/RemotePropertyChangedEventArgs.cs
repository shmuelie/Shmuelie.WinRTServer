using System;
using System.Runtime.InteropServices;
using Shmuelie.WinRTServer.Sample.Interfaces;

namespace Shmuelie.WinRTServer.Sample;

[Guid("C4B16766-C0A3-4FBC-B049-B46ED7198012")]
internal class RemotePropertyChangedEventArgs : EventArgs, IRemotePropertyChangedEventArgs
{
    public RemotePropertyChangedEventArgs(string propertyName)
    {
        PropertyName = propertyName;
    }

    public string PropertyName { get; }
}
