using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TSD.Model;
using TSD.Model.User;
using TSD.Services;
using TSD.Services.DataBase;
using TSD.Services.Extensions;
using TSD.Services.FileServices;
using TSD.Services.Interfaces;
using TSD.Services.Rest;
using Xamarin.Essentials;
using Xamarin.Forms;
using SettingTSD = TSD.Model.Setting.SettingTSD;
namespace TSD.ViewModels
{
    /// <summary>
    /// Flyout Page - выпадающее меню слева (ViewModel)
    /// </summary>
    public class AppPageViewModel : ViewModelBase
    {
        #region Приватные поля
        private AbstractMenu _selectMenu = null;
        private bool _isUsbWork = false;
        #endregion

        #region Свойства
        /// <summary>
        /// Выбранный пункт меню
        /// </summary>
        public AbstractMenu SelectMenu
        {
            get => _selectMenu;
            set => SetProperty(ref _selectMenu, value);
        }
        #endregion

        #region Свойства для чтения
        /// <summary>
        /// Список пунктов меню отсортированные по ключу
        /// </summary>
        public ObservableCollection<AbstractMenu> MenuItems => UserAccount.Menus.OrderBy(x => x.Key).ToObservableCollection();

        /// <summary>
        /// Свойство поля идетификатора (Login + Id)
        /// </summary>
        public string Login => $"{UserAccount.Login} ({UserAccount.UserId})";

        /// <summary>
        /// Версия приложения
        /// </summary>
        public string VersionApp => AppInfo.Version.ToString();

        /// <summary>
        /// Если режим сетевой
        /// </summary>
        public bool IsNetwork => !UserAccount.IsUsbWork;
        #endregion

        #region Команды
        /// <summary>
        /// Команда выхода
        /// </summary>
        public ICommand LogoutCommand { get; set; }

        /// <summary>
        /// Команда открытия окна задач
        /// </summary>
        public ICommand TaskCommand { get; set; }

        /// <summary>
        /// Очитска БД (только режим USB)
        /// </summary>
        public ICommand ClearDb { get; set; }

        /// <summary>
        /// Принудительная загрузка документов
        /// </summary>
        public ICommand RefreshDocuments { get; set; }
        #endregion

        #region Конструктор
        public AppPageViewModel()
        {
            MessagingCenter.Subscribe<MessageClass<object>>(this, Resources.MsgCenterTagUpdateMenus, x =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (_manager.IsValue(Resources.RestTypeTransfer))
                    {
                        var isParse = int.TryParse(_manager.GetValue(Resources.RestTypeTransfer), out int indexType);

                        if (isParse)
                        {
                            _isUsbWork = !SettingTSD.TypeTransfers[indexType].IsNetwork;
                        }
                    }

                    DependencyService.Get<IWorkService>().StartForegroundServiceCompact(ServiceEnums.WaitLogin);

                    if (!await UserGet())
                    {
                        UserAccount.Menus.Clear();
                        UserAccount.SelectedMenu = null;

                        DependencyService.Get<IWorkService>().StopForegroundServiceCompact(ServiceEnums.WaitLogin);
                    }

                    RaisePropertyChanged(nameof(MenuItems));
                    RaisePropertyChanged(nameof(Login));
                    RaisePropertyChanged(nameof(VersionApp));
                });
            });
            
            LogoutCommand = new Command(LogoutMethod);
            TaskCommand = new Command(OpenTaskMetnod);
            ClearDb = new Command(ClearDBMethod);
            RefreshDocuments = new Command(UpdateAllDocuments);
        }
        #endregion

        #region Приватные методы
        /// <summary>
        /// Метод выхода пользователя
        /// </summary>
        private async void LogoutMethod()
        {
            if (await Application.Current.MainPage.DisplayAlert(Resources.TitleReselectUser, Resources.MessageQuestionLogout, Resources.Yes, Resources.No))
            {
                DependencyService.Get<IWorkService>().StopForegroundServiceCompact(ServiceEnums.WaitLogin);
                MessagingCenter.Send(new MessageClass<object>(), Resources.MsgCenterTagLogout);

                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
        }

        /// <summary>
        /// Метод загрузки данных о пользователе
        /// </summary>
        /// <returns></returns>
        private async Task<bool> UserGet()
        {
            if (_isUsbWork)
            {
                return await UserGetUsb();
            }
            else
            {
                return await UserGetNetwork();
            }
        }

        /// <summary>
        /// Метод парсирования пункта меню из JSON
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="objectValue"></param>
        private async void GetMenuObject(AbstractMenu menu, JObject objectValue)
        {
            if (objectValue != null && objectValue.ContainsKey(Resources.RestKeyValue))
            {
                var values = JObject.Parse(RestService.Decompress(objectValue[Resources.RestKeyValue].ToString()));
                menu.AllowAddRows = ((JObject)values[Resources.RestKeyObjectDescription]).ContainsKey(Resources.RestKeyAllowAddRows) &&
                                    bool.Parse(values[Resources.RestKeyObjectDescription][Resources.RestKeyAllowAddRows].ToString());
                menu.IsBlockProcessedRows = ((JObject)values[Resources.RestKeyObjectDescription]).ContainsKey(Resources.RestKeyBlockProcessedRows) &&
                                            bool.Parse(values[Resources.RestKeyObjectDescription][Resources.RestKeyBlockProcessedRows].ToString());
                menu.IsBatchLoad = ((JObject)values[Resources.RestKeyObjectDescription]).ContainsKey(Resources.RestKeyBatchLoad) &&
                                            bool.Parse(values[Resources.RestKeyObjectDescription][Resources.RestKeyBatchLoad].ToString());
                menu.IsDoc = ((JObject)values[Resources.RestKeyObjectDescription]).ContainsKey(Resources.RestKeyIsDoc) &&
                                            bool.Parse(values[Resources.RestKeyObjectDescription][Resources.RestKeyIsDoc].ToString());

                if (((JObject)values[Resources.RestKeyObjectDescription]).ContainsKey(Resources.RestKeyActions))
                    menu.Actions = JsonConvert.DeserializeObject<ObservableCollection<AbstractAction>>(values[Resources.RestKeyObjectDescription]
                        [Resources.RestKeyActions].ToString());

                if (((JObject)values[Resources.RestKeyObjectDescription]).ContainsKey(Resources.RestKeyFields))
                    menu.Fields = JsonConvert.DeserializeObject<ObservableCollection<AbstractField>>(values[Resources.RestKeyObjectDescription]
                        [Resources.RestKeyFields].ToString());

                if (((JObject)values[Resources.RestKeyObjectDescription]).ContainsKey(Resources.RestKeyColumns))
                {
                    menu.Columns = JsonConvert.DeserializeObject<ObservableCollection<AbstractColumn>>(values[Resources.RestKeyObjectDescription]
                        [Resources.RestKeyColumns].ToString());

                    var sum = menu.Columns.Where(element => element.Browse == 1).Sum(element => element.Size);

                    menu.Columns = menu.Columns.Select(x =>
                    {
                        if (x.Browse == 1)
                        {
                            x.SizeColumn = ((DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density) - 45)
                            * (x.Size / (double)sum);
                        }

                        return x;
                    }).ToObservableCollection();
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageUnknownErrorQuery, Resources.OK);
            }
        }

        /// <summary>
        /// Метод открытия окна задач
        /// </summary>
        private void OpenTaskMetnod()
        {
            MessagingCenter.Send(new MessageClass<object>(), Resources.MsgCenterTagOpenTask);
            MessagingCenter.Send(new MessageClass<object>(), Resources.MsgCenterTagCloseHamburger);
        }

        /// <summary>
        /// Загрузка данных о пользователе через сеть
        /// </summary>
        /// <returns>Выводит true - если загрузка произошла успешно, иначе false</returns>
        private async Task<bool> UserGetNetwork()
        {
            try
            {
                if (ServiceDevice.IsNetwork)
                {
                    var userResponse = await RestService.SendRequest(ServiceDevice.BaseURL + Resources.UrlGetUser, HttpMethodRest.GET, null, _manager.GetValue(Resources.RestKeyToken).ToString());

                    if (userResponse != null)
                    {
                        UserAccount.FromJsonResponse(userResponse);
                    }

                    MessagingCenter.Send(new MessageClass<object>(), Resources.MsgCenterTagMenuLoad);

                    for (int i = 0; i < UserAccount.Menus.Count; i++)
                    {
                        AbstractMenu menu = UserAccount.Menus[i];

                        var actionContext = new JObject
                        {
                            { Resources.RestKeyActionName,  menu.Action }
                        };

                        var descriptionResponse = await RestService.SendRequest(ServiceDevice.BaseURL + Resources.UrlGetDescr, HttpMethodRest.POST, actionContext, _manager.GetValue(Resources.RestKeyToken));

                        if (descriptionResponse != null && descriptionResponse.ContainsKey(Resources.RestKeyDataContext))
                        {
                            if (descriptionResponse[Resources.RestKeyDataContext].ToString() != string.Empty)
                            {
                                GetMenuObject(menu, descriptionResponse);
                            }
                            else
                            {
                                menu.IsEnabled = false;
                            }
                        }
                        else
                        {
                            menu.IsEnabled = false;
                        }
                    }

                    MessagingCenter.Send(new MessageClass<object>(), Resources.MsgCenterTagMenuLoad);

                    RaisePropertyChanged(nameof(MenuItems));
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageNotConnect, Resources.OK);

                    await Application.Current.MainPage.Navigation.PopModalAsync();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { nameof(AppPageViewModel), nameof(UserGetNetwork) } });
            }

            return true;
        }

        /// <summary>
        /// Загрузка данных о пользователе через USB
        /// </summary>
        /// <returns>Выводит true - если загрузка произошла успешно, иначе false</returns>
        private async Task<bool> UserGetUsb()
        {
            DependencyService.Get<IWorkService>().StopForegroundServiceCompact(ServiceEnums.Sync);
            MessagingCenter.Send(new MessageClass<object>(), Resources.MsgCenterTagMenuLoad);
            try
            {
                var menuFileName = FileService.GetMenuFileName();

                if (menuFileName == null) return false;

                var json = FileService.GetJsonFromFile(menuFileName);

                if (json == null) return false;

                UserAccount.FromJsonFileMenu(json);

                RaisePropertyChanged(nameof(MenuItems));

                MessagingCenter.Send(new MessageClass<object>(), "LoadDocuments");

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { nameof(AppPageViewModel), nameof(UserGetUsb) } });

                MessagingCenter.Send(new MessageClass<object>(), Resources.MsgCenterTagMenuLoad);
            }

            MessagingCenter.Send(new MessageClass<object>(), Resources.MsgCenterTagMenuLoad);

            return true;
        }

        /// <summary>
        /// Метод очистки БД
        /// </summary>
        private async void ClearDBMethod()
        {
            if (await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageQuestionDeleteAllDoc, Resources.Yes, Resources.No))
            {
                var isDelete = WorkDataBase.ClearDataBaseByUser();
                
                if (isDelete == false)
                {
                    await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageEmptyByDelete, Resources.OK);

                    return;
                }

                if (isDelete == null)
                {
                    await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageUnknownError, Resources.OK);

                    return;
                }

                UserAccount.ClearDocMenus();

                MessagingCenter.Send(new MessageClass<object>(), Resources.MsgCenterTagIsVisibleHomeUpdate);

                await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageClearDBSuccess, Resources.OK);

            }
        }

        private void UpdateAllDocuments()
        {
            MessagingCenter.Send(new MessageClass<object>(), "LoadDocuments");
        }
        #endregion
    }
}
