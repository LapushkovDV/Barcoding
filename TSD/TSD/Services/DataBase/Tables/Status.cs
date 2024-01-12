using SQLite;

namespace TSD.Services.DataBase.Tables
{
    /// <summary>
    /// Таблица статусов
    /// </summary>
    [Table("Status")]
    public class Status : BaseModel
    {
        /// <summary>
        /// Наименование статуса
        /// </summary>
        public string Name { get; set; }
    }
}
