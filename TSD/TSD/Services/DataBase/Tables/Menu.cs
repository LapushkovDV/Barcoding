using SQLite;

namespace TSD.Services.DataBase.Tables
{
    /// <summary>
    /// Таблица пунктов меню
    /// </summary>
    [Table("Menu")]
    public class Menu : BaseModel
    {
        /// <summary>
        /// Наименование меню
        /// </summary>
        public string Action { get; set; }
    }
}
