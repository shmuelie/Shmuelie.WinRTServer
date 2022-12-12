namespace Shmuelie.Interop.Windows;

internal enum TrustLevel
{
    BaseTrust = 0,
    PartialTrust = (BaseTrust + 1),
    FullTrust = (PartialTrust + 1),
}
