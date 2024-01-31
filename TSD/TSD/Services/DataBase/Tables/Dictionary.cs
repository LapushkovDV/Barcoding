using SQLite;

namespace TSD.Services.DataBase.Tables
{   /// <summary>
    /// Таблица справочника
    /// </summary>
    [Table("Dictionary")]
    public class Dictionary : BaseModel
    {

        /// <summary>
        /// Номер документа
        /// </summary>
        [Indexed]
        public string IdDoc { get; set; }

        /// <summary>
        /// Системный идентификатор (штрихкод)
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// Вторичный ключ к меню
        /// </summary>
        [Indexed]
        public int MenuId { get; set; }
    }
}
