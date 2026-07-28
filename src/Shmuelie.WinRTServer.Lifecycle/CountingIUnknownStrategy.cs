using System;
using System.Runtime.InteropServices.Marshalling;

namespace Shmuelie.WinRTServer.Internal;

/// <summary>
/// An <see cref="IIUnknownStrategy"/> that forwards every call to a fully implemented inner strategy while
/// counting the references that pass through it, so an owner can be told when the count returns to zero.
/// </summary>
/// <remarks>This realizes the approach described in the library's lifecycle issue: wrap the "fully implemented"
/// strategy (from <see cref="StrategyBasedComWrappers"/>) and observe reference creation and release.</remarks>
internal sealed unsafe class CountingIUnknownStrategy(IIUnknownStrategy inner, ReferenceCountedServerLifetime owner) : IIUnknownStrategy
{
    public void* CreateInstancePointer(void* unknown)
    {
        void* instance = inner.CreateInstancePointer(unknown);
        owner.OnReferenceAdded();
        return instance;
    }

    public int QueryInterface(void* instancePtr, in Guid iid, out void* ppObj) => inner.QueryInterface(instancePtr, in iid, out ppObj);

    public int Release(void* instancePtr)
    {
        int result = inner.Release(instancePtr);
        owner.OnReferenceReleased();
        return result;
    }
}
