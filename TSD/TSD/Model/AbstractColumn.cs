using Newtonsoft.Json;

namespace TSD.Model
{
    /// <summary>
    /// Модель колонок
    /// </summary>

    [JsonObject(MemberSerialization.OptIn)]
    public class AbstractColumn : SharedModel
    {
        #region Приватные поля
        private int _npp;
        private string _name;
        private string _sysName;
        private int _size;
        private bool _modif;
        private int _wIsActive;
        private string _dataType;
        private bool _isModify = false;
        private string _value;
        private string _action;
        private int _browse = 0;
        private string _valueold;
        private double _sizeColumn = 1.0;
        private bool _isSync = false;
        private bool _browseCard = true;
        private bool _isNew = false;
        private bool _nullable = false;

        #endregion

        #region Свойства
        /// <summary>
        /// Номер позиции
        /// </summary>
        [JsonProperty("NPP")]
        public int Npp
        {
            get => _npp;
            set => SetProperty(ref _npp, value);
        }

        /// <summary>
        /// Название колонки
        /// </summary>
        [JsonProperty("NAME")]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        /// <summary>
        /// Системное название колонки
        /// </summary>
        [JsonProperty("SYSNAME")]
        public string SysName
        {
            get => _sysName;
            set => SetProperty(ref _sysName, value);
        }

        /// <summary>
        /// Размер колонок
        /// </summary>
        [JsonProperty("SIZE")]
        public int Size
        {
            get => _size;
            set => SetProperty(ref _size, value);
        }

        /// <summary>
        /// Условие, если можно модифицировать колонку
        /// </summary>
        [JsonProperty("MODIF")]
        public bool Modif
        {
            get => _modif;
            set => SetProperty(ref _modif, value);
        }

        /// <summary>
        /// Название действия для данной колонки
        /// </summary>
        [JsonProperty("ACTION")]
        public string Action
        {
            get => _action;
            set => SetProperty(ref _action, value);
        }

        /// <summary>
        /// Значение, если колонка активна
        /// </summary>
        [JsonProperty("WISACTIVE")]
        public int WIsActive
        {
            get => _wIsActive;
            set => SetProperty(ref _wIsActive, value);
        }

        /// <summary>
        /// Тип данных колонки
        /// </summary>
        [JsonProperty("DATATYPE")]
        public string DataType
        {
            get => _dataType;
            set => SetProperty(ref _dataType, value);
        }

        /// <summary>
        /// Значение, если нужно показывать колонки в карточной форме (0 - нет и 1 - да)
        /// </summary>
        [JsonProperty("BROWSE")]
        public int Browse
        {
            get => _browse;
            set => SetProperty(ref _browse, value);
        }

        /// <summary>
        /// Значение, если нужно показать колонку в спецификации
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

        /// <summary>
        /// Условие, если колонка была модифицирована
        /// </summary>
        public bool IsModify
        {
            get => _isModify;
            set => SetProperty(ref _isModify, value);
        }

        /// <summary>
        /// Размер колонки
        /// </summary>
        public double SizeColumn
        {
            get => _sizeColumn;
            set => SetProperty(ref _sizeColumn, value);
        }

        /// <summary>
        /// Значение колонки
        /// </summary>
        public string Value
        {
            get => _value;
            set
            {
                if (ValueOld != null && value != ValueOld)
                    IsModify = true;
                else
                    IsModify = false;

                SetProperty(ref _value, value);
            }
        }

        /// <summary>
        /// Старое значение колонки
        /// </summary>
        public string ValueOld
        {
            get => _valueold;
            set => SetProperty(ref _valueold, value);
        }

        /// <summary>
        /// Условие, если было выполнение команды и необходимо перезагрузить документ с сервера
        /// </summary>
        public bool IsSync
        {
            get => _isSync;
            set => SetProperty(ref _isSync, value);
        }

        /// <summary>
        /// Условие, если значение является новым
        /// </summary>
        public bool IsNew
        {
            get => _isNew;
            set => SetProperty(ref _isNew, value);
        }
        #endregion
    }
}
