using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TSD.Model;
using TSD.Model.User;
using TSD.Services;
using TSD.Services.DataBase;
using TSD.Services.Extensions;
using TSD.Services.Tasks;
using TSD.Services.Tasks.Models;
using Xamarin.Forms;

namespace TSD.ViewModels
{
    /// <summary>
    /// Окно статусов по документам (ViewModel)
    /// </summary>
    public class TaskViewModel : ViewModelBase
    {
        #region Приватные поля
        private bool _isTaskVisible = false;
        private ObservableCollection<DocumentTask> _documentTasks = new ObservableCollection<DocumentTask>();
        private ObservableCollection<HeaderValue> _headerValues = new ObservableCollection<HeaderValue>();
        #endregion

        #region Команды
        /// <summary>
        /// Команда отмены действия
        /// </summary>
        public ICommand CancelCommand { get; set; }
        #endregion

        #region Конструктор
        public TaskViewModel()
        {
            DocumentTasks = WorkDataBase.GetDocTasks().ToObservableCollection();

            MessagingCenter.Subscribe<MessageClass<object>>(this, Resources.MsgCenterTagOpenTask, x =>
            {
                IsTaskVisible = !IsTaskVisible;
            });


            MessagingCenter.Subscribe<MessageClass<string>>(this, Resources.MsgCenterTagNetwork, result =>
            {
                if (result.Data == Resources.OK)
                {
                    DocumentTasks = WorkDataBase.GetDocTasks().ToObservableCollection();
                }
            });

            MessagingCenter.Subscribe<MessageClass<int>>(this, Resources.MsgCenterUpdateTask, result =>
            {
                var value = DocumentTasks.FirstOrDefault(x => x.Id == result.Data);

                if (value != null)
                {
                    value.StatusId = (int)StatusEnum.Complete;

                    RaisePropertyChanged(nameof(DocumentTasks));
                }

            });


            CancelCommand = new Command(SwitchVisible);
        }
        #endregion

        #region Свойства
        /// <summary>
        /// Показывать окно задач
        /// </summary>
        public bool IsTaskVisible
        {
            get => _isTaskVisible;
            set => SetProperty(ref _isTaskVisible, value);

        }

        /// <summary>
        /// Список документов
        /// </summary>
        public ObservableCollection<DocumentTask> DocumentTasks
        {
            get => _documentTasks;
            set => SetProperty(ref _documentTasks, value);
        }


        /// <summary>
        /// Заголовки значений
        /// </summary>
        public ObservableCollection<HeaderValue> HeaderValues
        {
            get => _headerValues;
            set => SetProperty(ref _headerValues, value);
        }
        #endregion

        #region Приватные методы
        /// <summary>
        /// Метод открытия и закрытия окна с задачами
        /// </summary>
        public void SwitchVisible()
        {
            IsTaskVisible = !IsTaskVisible;
        }
        #endregion
    }
}