using System;
using System.Threading.Tasks;
using TSD.Services.Interfaces;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TSD.Services
{
    /// <summary>
    /// Сервис по работе с мобильным устройством
    /// </summary>
    public static class ServiceDevice
    {
        #region Приватные поля
        /// <summary>
        /// Доступ к аккаунт-мененджеру
        /// </summary>
        private static readonly IAccountManagerService _manager = DependencyService.Get<IAccountManagerService>();
        #endregion

        #region Публичные методы
        /// <summary>
        /// Проверка на доступность к сети на устройстве
        /// </summary>
        public static bool IsNetwork => Connectivity.NetworkAccess == NetworkAccess.Internet || Connectivity.NetworkAccess == NetworkAccess.Local || Connectivity.NetworkAccess == NetworkAccess.ConstrainedInternet;

        /// <summary>
        /// Проверка на доступность и не просроченность токена
        /// </summary>
        public static bool IsVerifyToken => _manager.IsValue(Resources.RestKeyExpire) && (DateTime.Parse(_manager.GetValue(Resources.RestKeyExpire)) > DateTime.Now);

        /// <summary>
        /// Проверка на доступность к информации устройства
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> IsVerifyPermissions()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Phone>();

            if (status == PermissionStatus.Denied)
            {
                status = await Permissions.RequestAsync<Permissions.Phone>();

                return status == PermissionStatus.Granted;
            }

            return true;
        }

        /// <summary>
        /// Проверка разрешения доступности информации об устройстве
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> IsCheckPermissions()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Phone>();

            if (status == PermissionStatus.Denied)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Базовый URL
        /// </summary>
        public static string BaseURL => _manager.IsValue(Resources.RestBaseUrl) ? _manager.GetValue(Resources.RestBaseUrl) : string.Empty;
        #endregion
    }
}
