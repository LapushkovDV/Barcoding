using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using TSD.Model;
using TSD.Model.User;
using TSD.Services;
using TSD.Services.Cryptography;
using TSD.Services.DataBase;
using TSD.Services.Interfaces;
using TSD.Services.Rest;
using TSD.Services.Scanner;
using Xamarin.Forms;

namespace TSD.ViewModels
{
    /// <summary>
    /// Окно просроченного токена (ViewModel)
    /// </summary>
    public class ExpireTokenViewModel : ViewModelBase
    {
        #region Приватные поля
        private bool _isExpire = false;
        private bool _isLoginForm = false;
        private string _login = string.Empty;
        private string _password = string.Empty;
        private bool _isBarcode = false;
        private bool _isBarcodeSwitch = false;
        private string _labelSwitch = Resources.LabelSwitchUserPass;
        #endregion

        #region Свойства
        /// <summary>
        /// Свойство поля с текстом выбранного типа авторизации
        /// </summary>
        public string LabelSwitch
        {
            get => _labelSwitch;
            set => SetProperty(ref _labelSwitch, value);
        }

        /// <summary>
        /// Свойство поля видимости типа авторизации
        /// </summary>
        public bool IsBarcodeSwitch
        {
            get => _isBarcodeSwitch;
            set => SetProperty(ref _isBarcodeSwitch, value);
        }

        /// <summary>
        /// Свойство поля переключателя типа авторизации (true - по штрихкоду, иначе по логину и паролю)
        /// </summary>
        public bool IsBarcode
        {
            get => _isBarcode;
            set
            {
                SetProperty(ref _isBarcode, value);

                LabelSwitch = value ? Resources.LabelSwitchBarcode : Resources.LabelSwitchUserPass;

                if (value)
                {
                    BarcodeService.StartBarcode();
                }
                else
                {
                    BarcodeService.StopBarcode();
                }

            }
        }

        /// <summary>
        /// Свойство поля видимости окна просроченного токена
        /// </summary>
        public bool IsExpire
        {
            get => _isExpire;
            set
            {
                SetProperty(ref _isExpire, value);

                if (_isExpire)
                {
                    MessagingCenter.Send(new MessageClass<int>(0), Resources.MsgCenterTagGestureFlyout);
                }
                else
                {

                    MessagingCenter.Send(new MessageClass<int>(1), Resources.MsgCenterTagGestureFlyout);
                }
            }
        }

        /// <summary>
        /// Свойство поля видимости ввода логина и пароля
        /// </summary>
        public bool IsLoginForm
        {
            get => _isLoginForm;
            set => SetProperty(ref _isLoginForm, value);
        }

        /// <summary>
        /// Свойство поля логин
        /// </summary>
        public string Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }

        /// <summary>
        /// Свойство поля пароль
        /// </summary>
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        #endregion

        #region Команды
        /// <summary>
        /// Команда октрытия ввода логина и пароля
        /// </summary>
        public ICommand AcceptCommand { get; set; }

        /// <summary>
        /// Команда отмены действий
        /// </summary>
        public ICommand CancelCommand { get; set; }

        /// <summary>
        /// Команда авторизации
        /// </summary>
        public ICommand LoginCommand { get; set; }
        #endregion

        #region Конструктор
        public ExpireTokenViewModel()
        {
            OpenBarcodesVisible();
            CancelCommand = new Command(CancelMethod);
            AcceptCommand = new Command(AcceptMethod);
            LoginCommand = new Command(LoginMethod);

            MessagingCenter.Subscribe<MessageClass<object>>(this, Resources.MsgCenterTagWaitAuth, (sender) =>
            {
                if (!IsExpire)
                {
                    IsExpire = true;
                    DependencyService.Get<IWorkService>().StopForegroundServiceCompact(ServiceEnums.WaitLogin);
                    MessagingCenter.Send(new MessageClass<bool>(!IsExpire), Resources.MsgCenterTagIsEnabledView);
                }
            });
        }
        #endregion

        #region Приватные методы
        /// <summary>
        /// Метод очистки окна просроченного токена
        /// </summary>
        private void ClearExpireForm()
        {
            IsExpire = false;
            IsLoginForm = false;
            Login = string.Empty;
            Password = string.Empty;
        }

        /// <summary>
        /// Метод октрытия окна ввода логина и пароля
        /// </summary>
        private void AcceptMethod() => IsLoginForm = true;

        /// <summary>
        /// Метод авторизации
        /// </summary>
        private async void LoginMethod()
        {
            IsActivity = true;
            StateText = Resources.StateVerifyConnectMessage;

            try
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
                            IsExpire = false;

                            ClearExpireForm();
                            DependencyService.Get<IWorkService>().StartForegroundServiceCompact(ServiceEnums.WaitLogin);

                            MessagingCenter.Send(new MessageClass<bool>(!IsExpire), Resources.MsgCenterTagIsEnabledView);
                            await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageUpdateToken, Resources.OK);

                            if (UserAccount.SelectedMenu != null)
                            {
                                WorkDataBase.LoadCurrentDocument(UserAccount.SelectedMenu.CurrentDocument.Id);

                                MessagingCenter.Send(new MessageClass<object>(), Resources.MsgCenterTagUpdateUniversal);
                            }
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
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { nameof(ExpireTokenViewModel), nameof(LoginMethod) } });
            }

            IsActivity = false;
        }

        /// <summary>
        /// Метод отмены действий
        /// </summary>
        private async void CancelMethod()
        {
            if (IsLoginForm)
            {
                IsLoginForm = !IsLoginForm;
            }
            else
            {
                IsExpire = false;

                MessagingCenter.Send(new MessageClass<object>(), Resources.MsgCenterTagLogout);
                MessagingCenter.Send(new MessageClass<bool>(!IsExpire), Resources.MsgCenterTagIsEnabledView);
                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
        }

        /// <summary>
        /// Метод проверки устройства на лазерный сканер (HoneyWell)
        /// </summary>
        private async void OpenBarcodesVisible() => IsBarcodeSwitch = await DependencyService.Get<IBarcodeService>().IsBarcodeScannerAsync();
        #endregion
    }
}
