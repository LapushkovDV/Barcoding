using SQLite;

namespace TSD.Services.DataBase.Tables
{
    /// <summary>
    /// Таблица колонок
    /// </summary>
    [Table("Column")]
    public class Column : BaseModel
    {
        /// <summary>
        /// Номер позиции колонки
        /// </summary>
        public int Npp { get; set; }

        /// <summary>
        /// Название колонки
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Системное имя колонки
        /// </summary>
        public string SysName { get; set; }

        /// <summary>
        /// Размер колонки
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Условие, если модификация колонки разрешена
        /// </summary>
        public bool Modif { get; set; }

        /// <summary>
        /// Условие, если были изменения
        /// </summary>
        public bool IsModify { get; set; }

        /// <summary>
        /// Условие, если позиция новая
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// Операция над колонкой
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Значение, если колонка активна
        /// </summary>
        public int WIsActive { get; set; }

        /// <summary>
        /// Тип данных колонки
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// Значение, если нужно показать колонку в таблице
        /// </summary>
        public int Browse { get; set; }

        /// <summary>
        /// Размер колонки, пересчитанный
        /// </summary>
        public double SizeColumn { get; set; }

        /// <summary>
        /// Значение колонки
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Старое значение
        /// </summary>
        public string ValueOld { get; set; }

        /// <summary>
        /// Значение, если нужно показать колонку в спецификации
        /// </summary>
        public bool BrowseCard { get; set; }
        
        /// <summary>
        /// Значение, если нужно обнулить колонку
        /// </summary>
        public bool Nullable { get; set; }

        /// <summary>
        /// Вторичный ключ к строке
        /// </summary>
        [Indexed]
        public int RowId { get; set; }
    }
}
