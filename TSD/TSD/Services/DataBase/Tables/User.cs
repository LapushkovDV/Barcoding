using SQLite;

namespace TSD.Services.DataBase.Tables
{
    /// <summary>
    /// Таблица пользователей
    /// </summary>
    [Table("User")]
    public class User : BaseModel
    {
        /// <summary>
        ///Ключ пользователя с сервера
        /// </summary>
        public int ExternalUserId { get; set; }

        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string Login { get; set; }
    }
}
