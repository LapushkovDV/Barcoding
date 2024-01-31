using System.Threading.Tasks;

namespace TSD.Services.Interfaces
{
    /// <summary>
    /// Интерфейс для работы с сервисом штрихкодов
    /// </summary>
    public interface IBarcodeService
    {
        /// <summary>
        /// Условие, имеется-ли у устройства штрихкодер
        /// </summary>
        /// <returns></returns>
        Task<bool> IsBarcodeScannerAsync();
    }
}
