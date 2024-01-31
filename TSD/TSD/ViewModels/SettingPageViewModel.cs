using System.Collections.ObjectModel;
using System.Windows.Input;
using DeviceSetting = TSD.Model.Setting.Device;
using TSD.Model.Setting;
using Xamarin.Forms;
using TSD.Services.Cryptography;
using TSD.Services;
using TSD.Services.Rest;

namespace TSD.ViewModels
{
    /// <summary>
    /// Окно настроек ТСД (ViewModel)
    /// </summary>
    public class SettingPageViewModel : ViewModelBase
    {
        #region Приватные поля
        private int _selectTransfer = 1;
        private int _selectDevice = 0;
        private bool _isChangePass = false;
        private string _oldPasswordAdmin = string.Empty;
        private string _newPasswordAdmin = string.Empty;
        private string _confirmNewPasswordAdmin = string.Empty;
        private string _address = string.Empty;
        private bool _isNetworkVisible = false;
        #endregion

        #region Конструкторы
        public SettingPageViewModel()
        {
            SelectDevice = 0;
            SelectTypeTransfer = 1;
            Address = _manager.IsValue(Resources.RestHost) ? _manager.GetValue(Resources.RestHost) : string.Empty;

            if (_manager.IsValue(Resources.RestTypeTransfer))
            {
                var indexTypeTransfer = int.Parse(_manager.GetValue(Resources.RestTypeTransfer));
      
                SelectTypeTransfer = SettingTSD.SelectTypeTransfer = SettingTSD.TypeTransfers.Count < indexTypeTransfer ? 0 : indexTypeTransfer;

                IsNetworkVisible = SettingTSD.TypeTransfers[SelectTypeTransfer].IsNetwork;
            }    

            if (_manager.IsValue(Resources.RestTypeDevice))
            {
                var indexTypeDevice = int.Parse(_manager.GetValue(Resources.RestTypeDevice));


                SelectDevice = SettingTSD.SelectDevice = SettingTSD.Devices.Count < indexTypeDevice ? 0 : indexTypeDevice;
            }

            Cancel = new Command(CancelMethod);
            ChangePassAdmin = new Command(ChangePassAdminMethod);
            Accept = new Command(AcceptMethod);
            SaveSetting = new Command(SaveSettingMethod);
            CancelSetting = new Command(CancelSettingMethod);
        }
        #endregion

        #region Команды
        public ICommand ChangePassAdmin { get; set; }
        public ICommand Accept { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand SaveSetting { get; set; }
        public ICommand CancelSetting { get; set; }
        #endregion

        #region Свойства для чтения
        public ObservableCollection<TypeTransfer> TypeTransfers => SettingTSD.TypeTransfers;
        public ObservableCollection<DeviceSetting> Devices => SettingTSD.Devices;
        #endregion

        #region Свойства
        public int SelectTypeTransfer
        {
            get => _selectTransfer;
            set 
            { 
                SetProperty(ref _selectTransfer, value);
                
                IsNetworkVisible = SettingTSD.TypeTransfers[value].IsNetwork;

                if (!IsNetworkVisible)
                {
                    Address = _manager.GetValue(Resources.RestHost);
                }
            }
        }

        public int SelectDevice
        {
            get => _selectDevice;
            set => SetProperty(ref _selectDevice, value);
        }

        public bool IsChangePass
        {
            get => _isChangePass;
            set => SetProperty(ref _isChangePass, value);
        }

        public string OldPasswordAdmin
        {
            get => _oldPasswordAdmin;
            set => SetProperty(ref _oldPasswordAdmin, value);
        }

        public string NewPasswordAdmin
        {
            get => _newPasswordAdmin;
            set => SetProperty(ref _newPasswordAdmin, value);
        }

        public string ConfirmNewPasswordAdmin
        {
            get => _confirmNewPasswordAdmin; 
            set => SetProperty(ref _confirmNewPasswordAdmin, value);
        }


        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        public bool IsNetworkVisible
        {
            get => _isNetworkVisible;
            set => SetProperty(ref _isNetworkVisible, value);
        }
        #endregion

        #region Методы для команд
        private void CancelMethod()
        {
            IsChangePass = false;
        }

        private async void AcceptMethod() 
        {
            if (NewPasswordAdmin == string.Empty)
            {
                await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageNotNewPass, Resources.OK);

                OldPasswordAdmin = NewPasswordAdmin = ConfirmNewPasswordAdmin = string.Empty;

                return;
            }

            var hashNewPass = Cryptography.ComputeSHA256(NewPasswordAdmin);
            var hashConfirmPass = Cryptography.ComputeSHA256(ConfirmNewPasswordAdmin);

            if (hashNewPass != hashConfirmPass)
            {
                await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageNotConfirmPass, Resources.OK);

                OldPasswordAdmin = NewPasswordAdmin = ConfirmNewPasswordAdmin = string.Empty;

                return;
            }

            if (!_manager.IsValue(Resources.RestAdminPass)) 
            {
                _manager.SetValue(Resources.RestAdminPass, hashNewPass);
            }
            else 
            {
                var hashOldPass = Cryptography.ComputeSHA256(OldPasswordAdmin);
                var hashExcept = _manager.GetValue(Resources.RestAdminPass);

                if (hashOldPass != hashExcept) 
                {
                    await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageNotOldPass, Resources.OK);

                    OldPasswordAdmin = NewPasswordAdmin = ConfirmNewPasswordAdmin = string.Empty;

                    return;
                }

                _manager.SetValue(Resources.RestAdminPass, hashNewPass);

                await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageAcceptPass, Resources.OK);              
            }

            OldPasswordAdmin = NewPasswordAdmin = ConfirmNewPasswordAdmin = string.Empty;

            IsChangePass = false;
        }

        private void ChangePassAdminMethod()
        {
            IsChangePass= true;
        }

        private async void SaveSettingMethod()
        {
            StateText = Resources.StateTextSave;
            IsActivity = true;
            _manager.SetValue(Resources.RestTypeDevice, SelectDevice.ToString());
            _manager.SetValue(Resources.RestTypeTransfer, SelectTypeTransfer.ToString());

            var baseURL = Address + "/api/";
            var isConnectService = await RestService.TestConnect(baseURL);

            if (IsNetworkVisible && !isConnectService)
            {
                await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageTagPartialSaveSetting, Resources.OK);

                Address = _manager.GetValue(Resources.RestHost);
            }
            else
            {
                if (IsNetworkVisible)
                {
                    _manager.SetValue(Resources.RestHost, Address);
                    _manager.SetValue(Resources.RestBaseUrl, baseURL);
                }

                await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageTagFullSaveSetting, Resources.OK);

                MessagingCenter.Send(new MessageClass<object>(), Resources.MsgCenterTagUpdateFormLogin);

                CancelSettingMethod();
            }

            IsActivity = false;
        }

        private async void CancelSettingMethod()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }
        #endregion
    }
}
