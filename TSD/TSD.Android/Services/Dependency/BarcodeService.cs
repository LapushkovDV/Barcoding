using Honeywell.AIDC.CrossPlatform;
using System.Threading.Tasks;
using TSD.Droid.Services.Dependency;
using TSD.Services.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(BarcodeService))]
namespace TSD.Droid.Services.Dependency
{
    public class BarcodeService : IBarcodeService
    {
        public async Task<bool> IsBarcodeScannerAsync()
        {
            return (await BarcodeReader.GetConnectedBarcodeReaders()).Count != 0;
        }
    }
}