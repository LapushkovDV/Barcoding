using TSD.Services.Interfaces;
using Xamarin.Common.Mvvm.Base;
using Xamarin.Forms;

namespace TSD.ViewModels
{
    /// <summary>
    /// Базовая ViewModel
    /// </summary>
    public abstract class ViewModelBase : BindableBase
    {
        private bool _isActivity = false;
        private string _stateText = Resources.StateTextDefault;
        private bool _isUsbWork = false;
        private string _typeTransfer = string.Empty;

        /// <summary>
        /// Аккаунт-мененджер для доступа к мобильному хранилищу приложения
        /// </summary>
        internal readonly IAccountManagerService _manager = DependencyService.Get<IAccountManagerService>();

        /// <summary>
        /// Свойство поля видимости загрузочного окна
        /// </summary>
        public bool IsActivity
        {
            get => _isActivity;
            set => SetProperty(ref _isActivity, value);
        }

        /// <summary>
        /// Свойство поля строки состояния загрузочного окна
        /// </summary>
        public string StateText
        {
            get => _stateText;
            set => SetProperty(ref _stateText, value);
        }

        /// <summary>
        /// Свойство поля видимости способа авторизации для режима USB
        /// </summary>
        public bool IsUsbWork
        {
            get => _isUsbWork;
            set => SetProperty(ref _isUsbWork, value);
        }

        /// <summary>
        /// Свойство отображения режима работы ТСД
        /// </summary>
        public string TypeTransfer
        {
            get => _typeTransfer;
            set => SetProperty(ref _typeTransfer, value);
        }
    }
}
