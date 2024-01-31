using Android;
using Android.Content;
using Android.OS;
using Android.Telephony;
using TSD.Droid.Services;
using TSD.Services.Interfaces;
using static Android.Provider.Settings;

[assembly: Xamarin.Forms.Dependency(typeof(InformationDevice))]
namespace TSD.Droid.Services
{
    /// <summary>
    /// Сервис для работы с устройством
    /// </summary>
    public class InformationDevice : IInformationDevice
    {
        /// <summary>
        /// Вывод кода устройства
        /// </summary>
        /// <returns>Код устройства</returns>
        public string GetDeviceCode()
        {
            if (MainActivity.Instance.CheckSelfPermission(Manifest.Permission.ReadPhoneState) == Android.Content.PM.Permission.Granted)
            {    
                var context = Android.App.Application.Context;
                var id = Secure.GetString(context.ContentResolver, Secure.AndroidId);

                return id;
            }
            else
            {
                return string.Empty;
            }
        }

    }
}