using System;
using TSD.Model;

namespace TSD.Services.Interfaces
{
    /// <summary>
    /// Интерфейс для работы с сервисами
    /// </summary>
    public interface IWorkService
    {
        /// <summary>
        /// Запуск сервиса
        /// </summary>
        /// <param name="service">Сервис</param>
        /// <param name="sourceCall">Тип источника вызова (необязательно)</param>
        void StartForegroundServiceCompact(ServiceEnums service = ServiceEnums.Sync, Type sourceCall = default);

        /// <summary>
        /// Остановка сервиса
        /// </summary>
        /// <param name="service"></param>
        void StopForegroundServiceCompact(ServiceEnums service = ServiceEnums.Sync);
    }
}
