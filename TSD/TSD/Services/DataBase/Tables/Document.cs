using SQLite;

namespace TSD.Services.DataBase.Tables
{
    /// <summary>
    /// Таблица документов
    /// </summary>
    [Table("Document")]
    public class Document : BaseModel
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
        /// Условие, если разрешено добавлять позиции в документ
        /// </summary>
        public bool IsAllowEditDoc { get; set; }

        /// <summary>
        /// GUID - номер статуса
        /// </summary>
        public string GuidStatus { get; set; }

        /// <summary>
        /// Последняя операция, выполняемая над документом
        /// </summary>
        public string LastAction { get; set; }

        /// <summary>
        /// Вторичный ключ к статусу
        /// </summary>
        [Indexed]
        public int StatusId { get; set; }

        /// <summary>
        /// Вторичный ключ к пользователю
        /// </summary>
        [Indexed]
        public int UserId { get; set; }

        /// <summary>
        /// Вторичный ключ к меню
        /// </summary>
        [Indexed]
        public int MenuId { get; set; }
    }
}
