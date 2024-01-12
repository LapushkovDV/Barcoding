namespace TSD.Services.DataBase.Interfaces
{
    /// <summary>
    /// Интерфейс для внедрения зависимостей путей в Android и IOS
    /// </summary>
    public interface IPath
    {
        /// <summary>
        /// Объявленная функция взятия пути локальной БД
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>Путь, где находится локальная БД</returns>
        string GetDatabasePath(string filename);
    }
}
