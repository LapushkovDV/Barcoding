using SQLite;

namespace TSD.Services.DataBase.Tables
{
    /// <summary>
    /// Таблица строк
    /// </summary>
    [Table("Row")]
    public class Row : BaseModel
    {
        /// <summary>
        /// Вторичный ключ к документу
        /// </summary>
        [Indexed]
        public int DocumentId { get; set; }

        /// <summary>
        /// Условия, если строка заблокирована
        /// </summary>
        public bool IsBlockRow { get; set; }

        /// <summary>
        /// Условие, если строка открывалась уже
        /// </summary>
        public bool IsOpenRow { get; set; }
    }
}
