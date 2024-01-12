using Newtonsoft.Json;

namespace TSD.Model
{
    /// <summary>
    /// Модель полей
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class AbstractField : SharedModel
    {
        #region Приватные поля 
        private bool _browseCard = true;
        private bool _nullable = false;
        #endregion
        #region Свойства
        /// <summary>
        /// Номер для сортировки
        /// </summary>
        [JsonProperty("ROW")]
        public int Row { get; set; }

        /// <summary>
        /// Наименование поля
        /// </summary>
        [JsonProperty("NAME")]
        public string Name { get; set; }

        /// <summary>
        /// Системное имя поля
        /// </summary>
        [JsonProperty("SYSNAME")]
        public string SysName { get; set; }

        /// <summary>
        /// Размер поля
        /// </summary>
        [JsonProperty("SIZE")]
        public int Size { get; set; }

        /// <summary>
        /// Условие, если поле является идентификатором
        /// </summary>
        [JsonProperty("ISIDENTIFIER")]
        public bool IsIdentifier { get; set; }

        /// <summary>
        /// Значение поля
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Значение, если нужно показать поле в панели
        /// </summary>
        [JsonProperty("BROWSE_CARD")]
        public bool BrowseCard
        {
            get => _browseCard; 
            set => SetProperty(ref _browseCard, value);
        }

        /// <summary>
        /// Значение, которое позволяет обнулять колонку
        /// </summary>
        [JsonProperty("NULLABLE")]
        public bool Nullable
        {
            get => _nullable;
            set => SetProperty(ref _nullable, value);
        }
        #endregion
    }
}
