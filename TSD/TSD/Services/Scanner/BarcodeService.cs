using System;
using TSD.Services.Interfaces;
using Xamarin.Forms;

namespace TSD.Services.Scanner
{
    /// <summary>
    /// Сервис по работе со штрихкодером (HoneyWell)
    /// </summary>
    public static class BarcodeService
    {
        public static void StartBarcode()
        {
            DependencyService.Get<IWorkService>().StartForegroundServiceCompact(Model.ServiceEnums.Scanner);
        }
        public static void StopBarcode()
        {
            DependencyService.Get<IWorkService>().StopForegroundServiceCompact(Model.ServiceEnums.Scanner);
        }
    }
}
