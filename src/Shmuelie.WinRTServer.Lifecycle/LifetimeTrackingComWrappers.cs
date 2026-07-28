using System.Runtime.InteropServices.Marshalling;

namespace Shmuelie.WinRTServer.Internal;

/// <summary>
/// A <see cref="StrategyBasedComWrappers"/> that wraps the default (fully implemented)
/// <see cref="IIUnknownStrategy"/> in a <see cref="CountingIUnknownStrategy"/> so a
/// <see cref="ReferenceCountedServerLifetime"/> can observe reference operations.
/// </summary>
internal sealed class LifetimeTrackingComWrappers(ReferenceCountedServerLifetime owner) : StrategyBasedComWrappers
{
    protected override IIUnknownStrategy GetOrCreateIUnknownStrategy() => new CountingIUnknownStrategy(base.GetOrCreateIUnknownStrategy(), owner);
}
