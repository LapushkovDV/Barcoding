using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using TSD.Model;
using TSD.Model.Setting;
using TSD.Model.User;
using TSD.Services;
using TSD.Services.Cryptography;
using TSD.Services.DataBase;
using TSD.Services.FileServices;
using TSD.Services.Interfaces;
using TSD.Services.Rest;
using TSD.Services.Scanner;
using TSD.Views.FlyoutPages;
using Xamarin.Forms;
using Device = Xamarin.Forms.Device;

namespace TSD.ViewModels
{
    /// <summary>
    /// Страница входа (ViewModel)
    /// </summary>
    public class LoginPageViewModel : ViewModelBase
    {
        #region Приватные поля
        private string _login = string.Empty;
        private string _password = string.Empty;
        private string _passwordAdmin = string.Empty;
        private string _address = string.Empty;
        private string _imei = string.Empty;
        private bool _isVisibleUser = false;
        private bool _isOpenSetting = false;
        private bool _isVisiblePermission = false;
        private bool _isConnectService = false;
        private bool _isAutoSign = false;
        private bool _checkLogin = false;
        #endregion

        #region Команды
        /// <summary>
        /// Команда авторизации
        /// </summary>
        public ICommand Auth { get; set; }

        /// <summary>
        /// Команда входа
        /// </summary>
        public ICommand SignIn { get; set; }

        /// <summary>
        /// Команда выхода
        /// </summary>
        public ICommand SignOut { get; set; }

        /// <summary>
        /// Команда доступа к разрешению использования информации о телефоне
        /// </summary>
        public ICommand AcceptPermission { get; set; }

        /// <summary>
        /// Команда открытия настроек подключения
        /// </summary>
        public ICommand OpenSettingConnect { get; set; }

        /// <summary>
        /// Комманда закрытия окна ввода пароля администратора
        /// </summary>
        public ICommand CancelCommand { get; set; }

        /// <summary>
        /// Подтверждение открытия панели настроек
        /// </summary>
        public ICommand AcceptOpenSetting { get; set; }
        #endregion

        #region Свойства
        /// <summary>
        /// Свойство поля логин
        /// </summary>
        public string Login
        {
            get => _login;
            set
            {
                if (value == null || value == string.Empty)
                {
                    CheckLogin = false;
                }
                else
                {
                    CheckLogin = true;
                }

                SetProperty(ref _login, value);
            }
        }

        /// <summary>
        /// Свойства поля пароля
        /// </summary>
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        /// <summary>
        /// Свойства поля пароля администатора (для настроек)
        /// </summary>
        public string PasswordAdmin
        {
            get => _passwordAdmin;
            set => SetProperty(ref _passwordAdmin, value);
        }

        /// <summary>
        /// Свойство поля видимости полей авторизованного пользователя
        /// </summary>
        public bool IsVisibleUser
        {
            get => _isVisibleUser;
            set => SetProperty(ref _isVisibleUser, value);
        }

        /// <summary>
        /// Свойство поля чекбокса для автовхода после входа в приложение
        /// </summary>
        public bool IsAutoSign
        {
            get => _manager.IsValue(Resources.SharedAutoLogin) && bool.Parse(_manager.GetValue(Resources.SharedAutoLogin).ToString());
            set
            {
                _manager.SetValue(Resources.SharedAutoLogin, value.ToString());
                SetProperty(ref _isAutoSign, value);
            }
        }

        /// <summary>
        /// Свойство поля идентификатора IMEI
        /// </summary>
        public string Imei
        {
            get => _imei;
            set => SetProperty(ref _imei, value);
        }

        /// <summary>
        /// Свойство поля видимости разрешения доступа к информации телефона
        /// </summary>
        public bool IsVisiblePermission
        {
            get => _isVisiblePermission;
            set => SetProperty(ref _isVisiblePermission, value);
        }

        /// <summary>
        ///Открытие окна ввода пароля администртатора 
        /// </summary>
        public bool IsOpenSetting
        {
            get => _isOpenSetting;
            set => SetProperty(ref _isOpenSetting, value);
        }

        /// <summary>
        /// Проверка на пустоту ввода логина
        /// </summary>
        public bool CheckLogin
        {
            get => _checkLogin;
            set => SetProperty(ref _checkLogin, value);
        }
        #endregion

        #region Свойства для чтения
        /// <summary>
        /// Свойство логина пользователя
        /// </summary>
        public string FullName => UserAccount.Login;
        #endregion

        #region Конструкторы
        public LoginPageViewModel()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var isParse = int.TryParse(_manager.GetValue(Resources.RestTypeTransfer), out int indexType);

                if (isParse)
                {
                    IsUsbWork = !SettingTSD.TypeTransfers[indexType].IsNetwork;
                    TypeTransfer = SettingTSD.TypeTransfers[indexType].Name;
                }
                else
                {
                    IsUsbWork = !SettingTSD.TypeTransfers.FirstOrDefault().IsNetwork;
                    TypeTransfer = SettingTSD.TypeTransfers.FirstOrDefault().Name;

                    _manager.SetValue(Resources.RestTypeTransfer, (0).ToString());
                }


                if (_manager.IsValue(Resources.RestBaseUrl))
                {
                    _address = _manager.IsValue(Resources.RestHost) ? _manager.GetValue(Resources.RestHost) : string.Empty;
                }

                if (!await ServiceDevice.IsCheckPermissions())
                {
                    IsVisiblePermission = true;
                    IsVisibleUser = false;
                }
                else
                {
                    Imei = DependencyService.Get<IInformationDevice>().GetDeviceCode();

                    VerifyToken();
                }

                UpdateProperties();
            });

            if (!_manager.IsValue(Resources.RestAdminPass))
            {
                _manager.SetValue(Resources.RestAdminPass, Resources.DefaultHashPassword);
            }



            Auth = new Command(Auth_Login);
            SignOut = new Command(Sign_Out);
            SignIn = new Command(Sign_In);
            AcceptPermission = new Command(Accept_Permission);
            CancelCommand = new Command(CancelView);
            AcceptOpenSetting = new Command(AcceptOpenSettingMethod);
            OpenSettingConnect = new Command(OpenSettingMethod);

            MessagingCenter.Subscribe<MessageClass<object>>(this, Resources.MsgCenterTagLogout, (sender) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DependencyService.Get<IWorkService>().StopForegroundServiceCompact(ServiceEnums.WaitLogin);
                    Sign_Out();
                });
            });

            MessagingCenter.Subscribe<MessageClass<object>>(this, Resources.MsgCenterTagUpdateFormLogin, (sender) =>
            {
                var isParse = int.TryParse(_manager.GetValue(Resources.RestTypeTransfer), out int indexType);

                

                if (isParse)
                {
                    if (IsUsbWork != !SettingTSD.TypeTransfers[indexType].IsNetwork)
                    {
                        Sign_Out();
                    }

                    IsUsbWork = !SettingTSD.TypeTransfers[indexType].IsNetwork;
                    TypeTransfer = SettingTSD.TypeTransfers[indexType].Name;
                }
            });

            UpdateProperties();
        }
        #endregion

        #region Методы для команд
        /// <summary>
        /// Метод входа
        /// </summary>
        private void Sign_In()
        {
            if (IsUsbWork)
            {
                SignInUsb();
            }
            else
            {
                SignInNetwork();
            }

        }

        /// <summary>
        /// Метод выхода
        /// </summary>
        private void Sign_Out()
        {
            WorkDataBase.IsAllUnblockPosition();
            IsVisibleUser = false;
            OpenBarcodesVisible();
            _manager.DeleteValue(Resources.RestKeyToken);
            _manager.DeleteValue(Resources.SharedLogin);
            _manager.DeleteValue(Resources.RestKeyExpire);
            _manager.DeleteValue(Resources.SharedAutoLogin);

            Login = string.Empty;
            Password = string.Empty;
            IsAutoSign = false;

            UserAccount.Clear();
        }

        /// <summary>
        /// Метод авторизации
        /// </summary>
        private void Auth_Login()
        {
            if (IsUsbWork)
            {
                Auth_Usb();
            }
            else
            {
                Auth_Network();
            }
        }

        /// <summary>
        /// Метод разрешения доступа к информации телефона
        /// </summary>
        private async void Accept_Permission()
        {
            if (!await ServiceDevice.IsVerifyPermissions())
            {
                IsVisiblePermission = true;
            }
            else
            {
                IsVisiblePermission = false;
                Imei = DependencyService.Get<IInformationDevice>().GetDeviceCode();

                VerifyToken();
            }

            UpdateProperties();
        }

        /// <summary>
        /// Метод открытия окна настроек при правильном вводе пароля
        /// </summary>
        private async void AcceptOpenSettingMethod()
        {
            if (PasswordAdmin is null && PasswordAdmin.Length == 0)
            {
                await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageNotPasswordAdmin, Resources.OK);

                PasswordAdmin = string.Empty;

                return;
            }

            if (!_manager.IsValue(Resources.RestAdminPass))
            {
                await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageNotPasswordAdminManager, Resources.OK);

                PasswordAdmin = string.Empty;

                return;
            }

            var hashPassExcept = _manager.GetValue(Resources.RestAdminPass);
            var hashPassActual = Cryptography.ComputeSHA256(PasswordAdmin);

            if (!hashPassActual.Contains(hashPassExcept))
            {
                await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageInvalidPassAdmin, Resources.OK);

                PasswordAdmin = string.Empty;

                return;
            }

            await Application.Current.MainPage.Navigation.PushModalAsync(new SettingPage());

            PasswordAdmin = string.Empty;
            IsOpenSetting = false;
        }
        #endregion

        #region Внутренние методы (не для команд)
        /// <summary>
        /// Метод проверки токена (хранение в аккаунт-менеджере токена, срок годности токена)
        /// </summary>
        private void VerifyToken()
        {
            try
            {

                if (IsUsbWork)
                {
                    VerifyTokenUsb();
                }
                else
                {
                    VerifyTokenNetwork();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { nameof(LoginPageViewModel), nameof(VerifyToken) } });
            }
        }

        private async void VerifyTokenNetwork()
        {
            if (_manager.IsValue(Resources.RestKeyToken))
            {
                if (_manager.IsValue(Resources.RestKeyExpire) && (DateTime.Parse(_manager.GetValue(Resources.RestKeyExpire)) > DateTime.UtcNow))
                {
                    UserAccount.UserId = WorkDataBase.SaveUser(_manager.GetValue(Resources.SharedLogin));
                    UserAccount.Login = _manager.IsValue(Resources.SharedLogin) ? _manager.GetValue(Resources.SharedLogin) : string.Empty;
                    IsAutoSign = _manager.IsValue(Resources.SharedAutoLogin) && bool.Parse(_manager.GetValue(Resources.SharedAutoLogin).ToString());

                    IsVisibleUser = true;

                    UpdateProperties();

                    if (ServiceDevice.IsNetwork &&
                        _manager.IsValue(Resources.SharedAutoLogin) &&
                        bool.Parse(_manager.GetValue(Resources.SharedAutoLogin).ToString()))
                    {
                        Sign_In();
                    }
                        
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageRepeatAuth, Resources.OK);
                    Sign_Out();
                }
            }
            else
            {
                Sign_Out();
            }
        }

        private async void VerifyTokenUsb()
        {
            if (_manager.IsValue(Resources.SharedLogin))
            {
                UserAccount.UserId = WorkDataBase.SaveUser(_manager.GetValue(Resources.SharedLogin));
                UserAccount.Login = _manager.IsValue(Resources.SharedLogin) ? _manager.GetValue(Resources.SharedLogin) : string.Empty;
                IsAutoSign = _manager.IsValue(Resources.SharedAutoLogin) && bool.Parse(_manager.GetValue(Resources.SharedAutoLogin).ToString());

                IsVisibleUser = true;

                UpdateProperties();

                if (ServiceDevice.IsNetwork &&
                    _manager.IsValue(Resources.SharedAutoLogin) &&
                    bool.Parse(_manager.GetValue(Resources.SharedAutoLogin).ToString()))
                    await Application.Current.MainPage.Navigation.PushModalAsync(new AppPage());
            }
            else
            {
                Sign_Out();
            }
        }

        /// <summary>
        /// Метод проверки устройства на лазерный сканер (HoneyWell)
        /// </summary>
        private async void OpenBarcodesVisible() => await DependencyService.Get<IBarcodeService>().IsBarcodeScannerAsync();

        /// <summary>
        /// Обновление свойств страницы входа
        /// </summary>
        private void UpdateProperties()
        {
            RaisePropertyChanged(nameof(FullName));
        }

        /// <summary>
        /// Метод открытия панели настроек
        /// </summary>
        private void OpenSettingMethod()
        {
            IsOpenSetting = true;
        }

        /// <summary>
        /// Закрытие настроек
        /// </summary>
        private void CancelView()
        {
            IsOpenSetting = false;
            PasswordAdmin = string.Empty;
        }

        /// <summary>
        /// Метод авторизации в режиме REST API
        /// </summary>
        private async void Auth_Network()
        {
            IsActivity = true;
            StateText = Resources.StateVerifyConnectMessage;

            try
            {
                _isConnectService = await RestService.TestConnect(_manager.GetValue(Resources.RestBaseUrl));

                if (_isConnectService)
                {
                    if (ServiceDevice.IsNetwork)
                    {
                        StateText = Resources.StateConnectAuth;

                        var requestJson = new JObject
                        {
                            { Resources.RestKeyLogin, Cryptography.ComputeSHA256(Login.ToString()) },
                            { Resources.RestKeyPassword, Cryptography.ComputeSHA256(Password.ToString()) },
                            { Resources.RestKeyImei, Cryptography.ComputeSHA256(DependencyService.Get<IInformationDevice>().GetDeviceCode()) }
                        };

                        var response = await RestService.SendRequest(ServiceDevice.BaseURL + Resources.UrlGetToken, HttpMethodRest.POST, requestJson);

                        if (response != null)
                        {
                            if (response.ContainsKey(Resources.RestKeyToken))
                            {
                                WorkDataBase.IsAllUnblockPosition();
                                _manager.SetValue(Resources.RestKeyToken, response[Resources.RestKeyToken].ToString());
                                _manager.SetValue(Resources.SharedLogin, Login.ToString());
                                _manager.SetValue(Resources.RestKeyExpire, response[Resources.RestKeyExpire].ToString());

                                StateText = Resources.StateUserInterface;

                                UserAccount.UserId = WorkDataBase.SaveUser(_manager.GetValue(Resources.SharedLogin));
                                UserAccount.Login = _manager.IsValue(Resources.SharedLogin) ? _manager.GetValue(Resources.SharedLogin) : string.Empty;

                                IsVisibleUser = true;

                                UpdateProperties();

                            }
                            else
                            {
                                await Application.Current.MainPage.DisplayAlert(Resources.Information, response[Resources.RestKeyResult].ToString(), Resources.OK);
                            }
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageNotConnect, Resources.OK);
                        }
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageAuthNotInternet, Resources.OK);
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageConnectStringFail, Resources.OK);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { nameof(LoginPageViewModel), nameof(Auth_Network) } });
            }

            IsActivity = false;
            Login = string.Empty;
            Password = string.Empty;
        }

        /// <summary>
        /// Метод авторизации в режиме USB
        /// </summary>
        private async void Auth_Usb()
        {
            IsActivity = true;
            StateText = Resources.StateSearchUser;

            try
            {

                var fileNameUser = FileService.GetUsersFileName();

                if (fileNameUser == null)
                {
                    await Application.Current.MainPage.DisplayAlert(Resources.Information, "Авторизация отклонена, список разрешенных пользователей отсутствует.", Resources.OK);

                    Login = string.Empty;
                    IsActivity = false;

                    return;
                }


                var json = FileService.GetJsonFromFile(fileNameUser);

                StateText = Resources.StateUserInterface;

                var isLoadUser = UserAccount.FromJsonUser(json, Login);

                if (!isLoadUser)
                {
                    await Application.Current.MainPage.DisplayAlert(Resources.Information, $"Авторизация отклонена, пользователь {Login} не найден в списке разрешенных пользователей.", Resources.OK);

                    Login = string.Empty;
                    IsActivity = false;

                    return;
                }

                _manager.SetValue(Resources.SharedLogin, Login.ToString());

                UserAccount.UserId = WorkDataBase.SaveUser(Login.ToString());
                UserAccount.Login = Login.ToString();

                IsVisibleUser = true;

                UpdateProperties();

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { nameof(LoginPageViewModel), nameof(Auth_Usb) } });
            }

            IsActivity = false;
            Login = string.Empty;
        }

        /// <summary>
        /// Вход в режиме REST API
        /// </summary>
        private async void SignInNetwork()
        {
            IsActivity = true;
            StateText = Resources.StateVerifyConnectMessage;

            _isConnectService = await RestService.TestConnect(_manager.GetValue(Resources.RestBaseUrl));

            if (!_isConnectService)
            {
                await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageConnectStringFail, Resources.OK);
                
                IsActivity = false;

                return;
            }

            UserAccount.ClearMenus();

            Device.BeginInvokeOnMainThread(async () =>
            {
                StateText = Resources.StateTextInTSD;

                if (_manager.IsValue(Resources.RestTypeDevice))
                {
                    SettingTSD.SelectDevice = int.Parse(_manager.GetValue(Resources.RestTypeDevice));

                    if (SettingTSD.Devices[SettingTSD.SelectDevice].TypeDevice == TypeDevice.Honeywell)
                    {
                        BarcodeService.StartBarcode();
                    }
                    else
                    {
                        BarcodeService.StopBarcode();
                    }
                }

                if (_manager.IsValue(Resources.RestTypeTransfer))
                {
                    SettingTSD.SelectTypeTransfer = int.Parse(_manager.GetValue(Resources.RestTypeTransfer));
                }

                if (ServiceDevice.IsNetwork)
                    await Application.Current.MainPage.Navigation.PushModalAsync(new AppPage());
                else
                    await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageNotConnect, Resources.OK);
                IsActivity = false;
            });
        }

        private async void SignInUsb()
        {
            UserAccount.ClearMenus();

            if (FileService.GetMenuFileName() != null)
            {
                await Application.Current.MainPage.Navigation.PushModalAsync(new AppPage());
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(Resources.Information, "На устройстве меню не обнаружено. Вход невозможен!", Resources.OK);
            }

        }
        #endregion
    }
}
