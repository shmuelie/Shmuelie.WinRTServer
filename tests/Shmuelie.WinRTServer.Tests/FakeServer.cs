using System;
using Shmuelie.WinRTServer;

namespace Shmuelie.WinRTServer.Tests;

/// <summary>
/// A controllable <see cref="IServer"/> used to drive lifetime helpers in tests.
/// </summary>
internal sealed class FakeServer : IServer
{
    public bool IsRunning { get; set; }

    public event EventHandler<InstanceCreatedEventArgs>? InstanceCreated;

    public void RaiseInstanceCreated(object instance) =>
        InstanceCreated?.Invoke(this, new InstanceCreatedEventArgs(instance));
}
