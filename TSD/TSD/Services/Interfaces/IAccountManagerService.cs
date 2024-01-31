namespace TSD.Services.Interfaces
{
    /// <summary>
    /// Интерфейс для работы с Account-мененджером
    /// </summary>
    public interface IAccountManagerService
    {
        /// <summary>
        /// Запись значения в Account-мененджер
        /// </summary>
        /// <param name="key">Ключ значения</param>
        /// <param name="value">Значение</param>
        void SetValue(string key, string value);

        /// <summary>
        /// Чтение значения с Account-мененджера
        /// </summary>
        /// <param name="key">Ключ значения</param>
        /// <returns>Значение по ключу</returns>
        string GetValue(string key);

        /// <summary>
        /// Условие существования ключа в Account-мененджерe
        /// </summary>
        /// <param name="key"> Ключ значения</param>
        /// <returns></returns>
        bool IsValue(string key);

        /// <summary>
        ///Удаление значения с Account-мененджера
        /// </summary>
        /// <param name="key">Ключ значения</param>
        void DeleteValue(string key);
    }
}
