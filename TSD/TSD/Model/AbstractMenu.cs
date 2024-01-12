using System.Collections.ObjectModel;
using TSD.Model.User;

namespace TSD.Model
{
    /// <summary>
    /// Модель меню
    /// </summary>
    public class AbstractMenu : SharedModel
    {
        #region Приватные поля
        private ObservableCollection<AbstractColumns> _columnsValue = new ObservableCollection<AbstractColumns>();
        #endregion

        #region Свойства
        /// <summary>
        ///Ключ меню
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Наименование меню
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Действие меню
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Условие, если разрешено добавлять позиции в документ
        /// </summary>
        public bool AllowAddRows { get; set; }

        /// <summary>
        /// Условие, если документ доступны для чтения
        /// </summary>
        public bool IsBlockProcessedRows { get; set; }

        /// <summary>
        /// Условие, если доступна пакетная загрузка
        /// </summary>
        public bool IsBatchLoad { get; set; }

        /// <summary>
        /// Условие, если это документ, а не справочник 
        /// </summary>
        public bool IsDoc { get; set; }

        /// <summary>
        /// Текущий документ
        /// </summary>
        public CurrentDocument CurrentDocument { get; set; } = new CurrentDocument();

        /// <summary>
        /// Набор полей
        /// </summary>
        public ObservableCollection<AbstractField> Fields { get; set; } = new ObservableCollection<AbstractField>();

        /// <summary>
        /// Набор действий
        /// </summary>
        public ObservableCollection<AbstractAction> Actions { get; set; } = new ObservableCollection<AbstractAction>();

        /// <summary>
        /// Набор колонок
        /// </summary>
        public ObservableCollection<AbstractColumn> Columns { get; set; } = new ObservableCollection<AbstractColumn>();

        /// <summary>
        /// Набор значений в таблице
        /// </summary>
        public ObservableCollection<AbstractColumns> ColumnsValue
        {
            get => _columnsValue;
            set => SetProperty(ref _columnsValue, value);
        }

        /// <summary>
        /// Активность меню (false - неактивно и true - активно)
        /// </summary>
        public bool IsEnabled { get; set; } = true;
        #endregion
    }
}