using SQLite;

namespace TSD.Services.DataBase.Tables
{
    /// <summary>
    /// Абстрактный базовый класс для моделей
    /// </summary>
    public abstract class BaseModel
    {
        /// <summary>
        /// Первичный ключ
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
    }
}
