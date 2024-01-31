using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Xml.Linq;
using TSD.Model;
using TSD.Model.User;
using TSD.Services;
using TSD.Services.DataBase;
using TSD.Services.Extensions;
using Xamarin.Forms;

namespace TSD.ViewModels
{
    /// <summary>
    /// Страница детальной формы
    /// </summary>
    public class UniversalDetailViewModel : ViewModelBase
    {
        #region Приватные поля
        private string _title = string.Empty;
        private string _titleButton = string.Empty;
        private AbstractColumn _selectedElement = null;
        private AbstractColumns _element;
        private AbstractColumns _tempElement;
        private Editor _entrySelect = null;
        private ObservableCollection<AbstractColumn> _elements = new ObservableCollection<AbstractColumn>();
        #endregion

        #region Конструктор
        public UniversalDetailViewModel()
        {
            MessagingCenter.Subscribe<MessageClass<object>>(this, Resources.MsgCenterTagSaveDocumentLocal, x =>
            {
                if (_element != null)
                {
                    _element.IsOpen = true;

                    WorkDataBase.UpdateRow(_element);
                }
            });

            MessagingCenter.Subscribe<MessageClass<string>>(this, Resources.MsgCenterTagPasteData, x =>
            {
                if (_entrySelect != null)
                    _entrySelect.Text = x.Data.Trim();      
            });

            MessagingCenter.Subscribe<MessageClass<Editor>>(this, "SelectEntry", x =>
            {
                _entrySelect = x.Data;
            });

            MessagingCenter.Subscribe<MessageClass<AbstractColumns>>(this, Resources.MsgCenterTagSendDataDetailView, element =>
            {
                _element = element.Data;

                var columnFirst = _element.ColumnsElementBrowse.FirstOrDefault();

                Title = $"{columnFirst.Name}:{columnFirst.Value}";
                TitleButton = UserAccount.SelectedMenu.IsBlockProcessedRows ? Resources.XamlButtonComplitedProcessed : Resources.OK;
                ComplitedProcessed = new Command(ComplitedProcessedMethod);
                NextElement = new Command(NextElementMethod, () => _element.NextRowId != null);
                PrevElement = new Command(PrevElementMethod, () => _element.PrevRowId != null);
                Elements = _element.ColumnsElement.Where(x => x.BrowseCard).ToObservableCollection();
            });
        }
        #endregion

        #region Команды
        /// <summary>
        /// Команда завершения обработки в детальной форме
        /// </summary>
        public ICommand ComplitedProcessed { get; set; }
        public ICommand NextElement { get; set; }
        public ICommand PrevElement { get; set; }
        #endregion

        #region Свойства
        /// <summary>
        /// Заголовок детальной формы
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        /// <summary>
        /// Заголовок кнопки обработки
        /// </summary>
        public string TitleButton
        {
            get => _titleButton;
            set => SetProperty(ref _titleButton, value);
        }

        public AbstractColumn SelectedElement
        {
            get => _selectedElement;
            set => SetProperty(ref _selectedElement, value);
        }
        #endregion

        #region Свойства для чтения
        /// <summary>
        /// Список колонок выбранного элемента
        /// </summary>
        public ObservableCollection<AbstractColumn> Elements
        {
            get => _elements;
            set => SetProperty(ref _elements, value);
        }

        /// <summary>
        /// Высота ListView
        /// </summary>
        public int HeightColumns => 50 * Elements.Count;
        #endregion

        #region Приватные методы
        /// <summary>
        /// Метод завершения обработки
        /// </summary>
        private void ComplitedProcessedMethod()
        {
            if (UserAccount.SelectedMenu.IsBlockProcessedRows) _element.IsBlock = true;

            MessagingCenter.Send(new MessageClass<object>(), Resources.MsgCenterTagCloseDetail);
        }

        /// <summary>
        /// Метод "Следующий элемент"
        /// </summary>
        private async void NextElementMethod()
        {

            _tempElement = _element;

            _element = UserAccount.SelectedMenu.ColumnsValue.FirstOrDefault(x => x.Id == _element.NextRowId);

            while (_element.IsBlock)
            {
                if (_element.NextRowId == null)
                {
                    _element = _tempElement;
                    break;
                }

                _element = UserAccount.SelectedMenu.ColumnsValue.FirstOrDefault(x => x.Id == _element.NextRowId);
            }

            if (_element.IsOpen)
            {
                if (await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageSeeRepeatPosition, Resources.Yes, Resources.No))
                {
                    _tempElement.IsOpen = true;

                    WorkDataBase.UpdateRow(_tempElement);
                }
                else
                {
                    _element = _tempElement;
                }
            }
            else
            {
                _tempElement.IsOpen = true;
                WorkDataBase.UpdateRow(_tempElement);
            }

            Elements = _element.ColumnsElement.Where(x => x.BrowseCard).ToObservableCollection();

            var columnFirst = _element.ColumnsElementBrowse.FirstOrDefault();

            Title = $"{columnFirst.Name}:{columnFirst.Value}";

            ((Command)NextElement).ChangeCanExecute();
            ((Command)PrevElement).ChangeCanExecute();
        }

        /// <summary>
        /// Метод "Предыдущий элемент"
        /// </summary>
        private async void PrevElementMethod()
        {
            _tempElement = _element;

            _element = UserAccount.SelectedMenu.ColumnsValue.FirstOrDefault(x => x.Id == _element.PrevRowId);

            while (_element.IsBlock)
            {
                if (_element.PrevRowId == null)
                {
                    _element = _tempElement;
                    break;
                }

                _element = UserAccount.SelectedMenu.ColumnsValue.FirstOrDefault(x => x.Id == _element.PrevRowId);
            }

            if (_element.IsOpen)
            {
                if (await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageSeeRepeatPosition, Resources.Yes, Resources.No))
                {
                    _tempElement.IsOpen = true;

                    WorkDataBase.UpdateRow(_tempElement);
                }
                else
                {
                    _element = _tempElement;
                }
            }
            else
            {
                _tempElement.IsOpen = true;
                WorkDataBase.UpdateRow(_tempElement);
            }

            Elements = _element.ColumnsElement.Where(x => x.BrowseCard).ToObservableCollection();

            var columnFirst = _element.ColumnsElementBrowse.FirstOrDefault();

            Title = $"{columnFirst.Name}:{columnFirst.Value}";

            ((Command)NextElement).ChangeCanExecute();
            ((Command)PrevElement).ChangeCanExecute();
        }
        #endregion
    }
}
