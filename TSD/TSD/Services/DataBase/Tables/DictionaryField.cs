using SQLite;

namespace TSD.Services.DataBase.Tables
{   /// <summary>
    /// Таблица полей справочника
    /// </summary>
    [Table("DictionaryField")]
    public class DictionaryField : BaseModel
    {

        /// <summary>
        /// Номер позиции поля для сортировки
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Наименование поля
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Системное имя поля
        /// </summary>
        public string SysName { get; set; }

        /// <summary>
        /// Размер поля
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Условие, если поле является идентификатором
        /// </summary>
        public bool IsIdentifier { get; set; }

        /// <summary>
        /// Значение поля
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Значение, если нужно показать поле в панели
        /// </summary>
        public bool BrowseCard { get; set; }

        /// <summary>
        /// Значение, если нужно обнулить поле
        /// </summary>
        public bool Nullable { get; set; }

        /// <summary>
        /// Вторичный ключ к справочнику
        /// </summary>
        [Indexed]
        public int DictionaryId { get; set; }
    }
}
