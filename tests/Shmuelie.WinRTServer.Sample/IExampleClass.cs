using Windows.Foundation;

namespace Shmuelie.WinRTServer.Sample;

public interface IExampleClass
{
    string Status { get; }

    IAsyncAction UpdateAsync();

    IAsyncOperation<bool> TrySaveAsync();
}
