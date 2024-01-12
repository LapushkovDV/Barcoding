namespace TSD.Services.Interfaces
{
    /// <summary>
    /// Интерфейс для работы с устройством
    /// </summary>
    public interface IInformationDevice
    {
        /// <summary>
        /// Вывод кода устройства
        /// </summary>
        /// <returns>Код устройства</returns>
        string GetDeviceCode();
    }
}