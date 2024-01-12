using System.Collections.ObjectModel;
using System.Linq;
using TSD.Services;
using TSD.Services.Extensions;
using Xamarin.Forms;

namespace TSD.Model
{
    /// <summary>
    /// Модель колонок (множество)
    /// </summary>
    public class AbstractColumns : SharedModel
    {
        #region Приватные поля
        private ObservableCollection<AbstractColumn> _columnsElement = new ObservableCollection<AbstractColumn>();
        private bool _isBlock = false;
        private bool _isOpen = false;
        private int? _prevRowElement = null;
        private int? _nextRowElement = null;
        #endregion

        #region Конструктор
        public AbstractColumns()
        {
            MessagingCenter.Subscribe<MessageClass<object>>(this, Resources.MsgCenterTagUpdateSelect, (sender) =>
            {
                RaisePropertyChanged(nameof(IsStatusRow));
            });
        }
        #endregion

        #region Свойства
        /// <summary>
        /// Массив колонок
        /// </summary>
        public ObservableCollection<AbstractColumn> ColumnsElement
        {
            get => _columnsElement;
            set => SetProperty(ref _columnsElement, value);
        }

        /// <summary>
        /// Условие если строка заблокирована для входа
        /// </summary>
        public bool IsBlock
        {
            get => _isBlock;
            set => SetProperty(ref _isBlock, value);
        }

        /// <summary>
        /// Условие, если в строку уже заходили
        /// </summary>
        public bool IsOpen
        {
            get => _isOpen;
            set => SetProperty(ref _isOpen, value);
        }

        /// <summary>
        /// Предыдущий элемент
        /// </summary>
        public int? PrevRowId
        {
            get => _prevRowElement;
            set => SetProperty(ref _prevRowElement, value);
        }

        /// <summary>
        /// Следующий элемент
        /// </summary>
        public int? NextRowId
        {
            get => _nextRowElement;
            set => SetProperty(ref _nextRowElement, value);
        }
        #endregion

        #region Свойства для чтения
        /// <summary>
        /// Массив колонок для табличной формы
        /// </summary>
        public ObservableCollection<AbstractColumn> ColumnsElementBrowse => ColumnsElement.Count != 0
            ? ColumnsElement.Where(x => x.Browse == 1).ToObservableCollection()
            : new ObservableCollection<AbstractColumn>();

        /// <summary>
        /// Длина строки
        /// </summary>
        public double SizeRow => ColumnsElement.Count != 0
            ? ColumnsElement.Where(x => x.Browse == 1).Sum(x => x.SizeColumn)
            : 0.0;

        /// <summary>
        /// Высота элементов
        /// </summary>
        public int HeightElements => ColumnsElement.Count != 0 ? (20 * ColumnsElement.Count) + 10 : 20;

        /// <summary>
        /// Статус строки
        /// </summary>
        public int IsStatusRow => ColumnsElement.Any(x => x.IsSync) ? 1
            : (ColumnsElement.Any(x => x.IsModify) && !IsBlock ? 2
            : (ColumnsElement.All(x => x.IsNew) && !IsBlock ? 3 
            : (IsOpen && !IsBlock ? 4 : 0)));
        #endregion

        #region Приватные методы
        /// <summary>
        /// Метод обновления свойств
        /// </summary>
        public void UpdateProperties()
        {
            RaisePropertyChanged(nameof(ColumnsElement));
            RaisePropertyChanged(nameof(ColumnsElementBrowse));
        }
        #endregion
    }
}
