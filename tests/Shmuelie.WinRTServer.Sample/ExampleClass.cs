using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Shmuelie.WinRTServer.Sample;

[Guid("34E53910-86A5-4C3A-A3BA-466ADC776BAC")]
public sealed class ExampleClass : IExampleClass
{
    public string Status => "OK";

    public async Task UpdateAsync()
    {
        await Task.Delay(1000);
    }

    public async Task<bool> TrySaveAsync()
    {
        await Task.Delay(1000);
        return false;
    }

    IAsyncAction IExampleClass.UpdateAsync()
    {
        return UpdateAsync().AsAsyncAction();
    }

    IAsyncOperation<bool> IExampleClass.TrySaveAsync()
    {
        return TrySaveAsync().AsAsyncOperation();
    }
}
