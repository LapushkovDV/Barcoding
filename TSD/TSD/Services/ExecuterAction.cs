using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSD.Model;
using TSD.Model.User;
using TSD.Services.DataBase;
using TSD.Services.Extensions;
using TSD.Services.FileServices;
using TSD.Services.Interfaces;
using TSD.Services.Rest;
using TSD.Services.Tasks;
using Xamarin.Forms;

namespace TSD.Services
{
    /// <summary>
    /// Статический класс для работы с операциями документов
    /// </summary>
    public static class ExecuterAction
    {
        #region Приватные поля
        /// <summary>
        /// Доступ к аккаунт-мененджеру
        /// </summary>
        private static readonly IAccountManagerService _manager = DependencyService.Get<IAccountManagerService>();
        #endregion

        #region Публичные методы
        /// <summary>
        /// Асинхронный метод выполнения операции (действий)
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="isSync"></param>
        /// <param name="isClose"></param>
        /// <returns></returns>
        public static async Task<bool> Execute(string actionName, bool isSync = false, bool isClose = false)
        {
            var selectMenu = UserAccount.SelectedMenu;

            if (selectMenu != null)
            {
                try
                {
                    if (ServiceDevice.IsNetwork && !UserAccount.IsUsbWork)
                    {
                        var fields = WorkDataBase.GetFieldsDoc(selectMenu.CurrentDocument.Id);
                        var columns = WorkDataBase.GetColumnsDoc(selectMenu.CurrentDocument.Id);
                        var data = BuildJsonOutput(actionName, selectMenu.Action, selectMenu.CurrentDocument.IdDoc, fields.ToList(), columns.ToList());

                        if (data.ContainsKey(Resources.RestKeyRequestBody))
                        {
                            if (!isSync)
                            {
                                var response = await RestService.SendRequest(ServiceDevice.BaseURL + Resources.UrlSetDataAsync, HttpMethodRest.POST, data, _manager.GetValue(Resources.RestKeyToken).ToString());

                                if (response != null)
                                {
                                    if (response[Resources.RestKeyResult].ToString() == Resources.OK)
                                    {
                                        UserAccount.SelectedMenu.CurrentDocument.GuidStatus = response[Resources.RestKeyGuid].ToString();

                                        var statusResponse = await RestService.SendRequest(ServiceDevice.BaseURL + Resources.UrlGetStatus, HttpMethodRest.POST, new JObject { { Resources.RestKeyGuid, response[Resources.RestKeyGuid].ToString() } }, _manager.GetValue(Resources.RestKeyToken).ToString());

                                        await Application.Current.MainPage.DisplayAlert(Resources.Information, statusResponse[Resources.RestKeyResult].ToString(), Resources.OK);

                                        UserAccount.SelectedMenu.CurrentDocument.StatusId = (int)StatusEnum.SendInERP;

                                        UserAccount.SelectedMenu.ColumnsValue = UserAccount.SelectedMenu.ColumnsValue.Select(x =>
                                        {
                                            x.ColumnsElement = x.ColumnsElement.Select(element =>
                                            {
                                                if (element.IsModify || element.IsNew)
                                                {
                                                    element.IsModify = false;
                                                    element.IsSync = true;
                                                    element.IsNew = false;
                                                }
                                                return element;
                                            }).ToObservableCollection();

                                            return x;
                                        }).ToObservableCollection();

                                        StopSync();
                                        WorkDataBase.SaveCurrentDocument(StatusEnum.SendInERP);
                                        StartSync();

                                        if (isClose)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else
                                    await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageUnknownErrorQuery, Resources.OK);
                            }
                            else
                            {
                                var response = await RestService.SendRequest(ServiceDevice.BaseURL + Resources.UrlSetData, HttpMethodRest.POST, data, _manager.GetValue(Resources.RestKeyToken).ToString());

                                if (response != null)
                                {
                                    if (response.ContainsKey(Resources.RestKeyValue))
                                    {
                                        var decompressValue = RestService.Decompress(response[Resources.RestKeyValue].ToString());

                                        if (decompressValue != null && decompressValue != string.Empty)
                                        {
                                            var value = JObject.Parse(decompressValue);

                                            await Application.Current.MainPage.DisplayAlert(Resources.Information, value[Resources.RestKeyResultMsg].ToString(), Resources.OK);

                                            if (value[Resources.RestKeyResultCode].ToString() == Resources.RestKeyResultCodeOne)
                                            {
                                                UserAccount.SelectedMenu.CurrentDocument.StatusId = (int)StatusEnum.Complete;
                                                UserAccount.SelectedMenu.ColumnsValue = UserAccount.SelectedMenu.ColumnsValue.Select(x =>
                                                {
                                                    x.ColumnsElement = x.ColumnsElement.Select(element =>
                                                    {
                                                        if (element.IsModify || element.IsNew)
                                                        {
                                                            element.IsModify = false;
                                                            element.IsSync = true;
                                                            element.IsNew = false;
                                                        }
                                                        return element;
                                                    }).ToObservableCollection();

                                                    return x;
                                                }).ToObservableCollection();

                                                StopSync();
                                                WorkDataBase.SaveCurrentDocument(StatusEnum.Complete);
                                                StartSync();

                                                MessagingCenter.Send(new MessageClass<int>(UserAccount.SelectedMenu.CurrentDocument.Id), Resources.MsgCenterUpdateTask);

                                                if (isClose)
                                                {
                                                    return true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.RestMessageValueEmptyJson, Resources.OK);
                                        }
                                    }
                                    else
                                    {
                                        await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.RestMessageAnswerEmpty, Resources.OK);
                                    }
                                }
                                else
                                    await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageUnknownErrorQuery, Resources.OK);
                            }
                        }
                    }
                    else
                    {
                        if (UserAccount.IsUsbWork)
                        {
                            var fields = WorkDataBase.GetFieldsDoc(selectMenu.CurrentDocument.Id);
                            var columns = WorkDataBase.GetColumnsDoc(selectMenu.CurrentDocument.Id);
                            var data = BuildJsonOutput(actionName, selectMenu.Action, selectMenu.CurrentDocument.IdDoc, fields.ToList(), columns.ToList(), false);
                            var idDevice = DependencyService.Get<IInformationDevice>().GetDeviceCode();

                            var fileName = $"{Guid.NewGuid()}_TSD_{idDevice}_{UserAccount.Login.ToUpper()}_{actionName}.json";
                            FileService.SetJsonFileFile(data, fileName);

                            UserAccount.SelectedMenu.CurrentDocument.StatusId = (int)StatusEnum.SendInERP;

                            UserAccount.SelectedMenu.ColumnsValue = UserAccount.SelectedMenu.ColumnsValue.Select(x =>
                            {
                                x.ColumnsElement = x.ColumnsElement.Select(element =>
                                {
                                    if (element.IsModify || element.IsNew)
                                    {
                                        element.IsModify = false;
                                        element.IsSync = true;
                                        element.IsNew = false;
                                    }
                                    return element;
                                }).ToObservableCollection();

                                return x;
                            }).ToObservableCollection();


                            StopSync();
                            WorkDataBase.SaveCurrentDocument(StatusEnum.Complete);
                            StartSync();

                            
                            if (isClose)
                            {
                                await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageUploadDataWithCloseDoc, Resources.OK);

                                return true;
                            }

                            await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageUploadData, Resources.OK);
                        }
                        else
                        {
                            UserAccount.SelectedMenu.CurrentDocument.LastAction = actionName;
                            WorkDataBase.SaveCurrentDocument(StatusEnum.InSyncToServer);

                            await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageNotNet, Resources.OK);

                            if (isClose)
                            {
                                return true;
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { nameof(ExecuterAction), nameof(Execute) } });
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageUnSelectMenu, Resources.OK);
            }

            return false;
        }

        /// <summary>
        /// Асинхронный метод выполнения фоновых операций по документам
        /// </summary>
        /// <param name="documentTask"></param>
        public static async void ExecuteBackgroundOfflineAsync(DocumentTask documentTask)
        {
            try
            {
                if (ServiceDevice.IsNetwork &&
                    documentTask != null &&
                    documentTask.LastAction != string.Empty &&
                    documentTask.StatusId == (int)StatusEnum.InSyncToServer)
                {
                    var fields = WorkDataBase.GetFieldsDoc(documentTask.Id);
                    var columns = WorkDataBase.GetColumnsDoc(documentTask.Id);
                    var data = BuildJsonOutput(documentTask.LastAction, documentTask.MenuAction, documentTask.IdDoc, fields, columns);

                    if (ServiceDevice.IsVerifyToken)
                    {
                        var response = await RestService.SendRequest(ServiceDevice.BaseURL + Resources.UrlSetDataAsync, HttpMethodRest.POST, data, _manager.GetValue(Resources.RestKeyToken));

                        if (response != null)
                        {
                            if (response[Resources.RestKeyResult].ToString() == Resources.OK)
                            {
                                WorkDataBase.SaveStatusDocumentById(StatusEnum.SendInERP, documentTask.Id, response[Resources.RestKeyGuid].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { nameof(ExecuterAction), nameof(ExecuteBackgroundOfflineAsync) } });
            }
        }

        public static bool IsEnabledAction(bool isEnabled = false)
        {
            return !isEnabled || (UserAccount.SelectedMenu is null || (UserAccount.SelectedMenu.ColumnsValue.Count == 0 || UserAccount.SelectedMenu.ColumnsValue.All(x => !x.IsBlock && x.ColumnsElement.All(element => !element.IsSync && !element.IsModify && !element.IsNew))));
        }
        #endregion

        #region Приватные методы
        /// <summary>
        /// Метод построения JSON из исходных данных
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="actionMenu"></param>
        /// <param name="idDoc"></param>
        /// <param name="fields"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        private static JObject BuildJsonOutput(string actionName, string actionMenu, string idDoc, List<AbstractField> fields = null,
            List<AbstractColumns> columns = null, bool isHeaderRequestBody = true)
        {
            var data = new JObject
                {
                    { Resources.RestKeyActionBody, actionName },
                    { Resources.RestKeyObjectValues,
                                new JObject
                                {
                                    { Resources.RestKeyObjectAction, actionMenu},
                                    { Resources.RestKeyIdObject, idDoc },
                                }
                    }
                };

            if (fields.Count != 0)
            {
                var objectElement = new JObject
                    {
                        { fields[0].SysName, fields[0].Value }
                    };

                for (var i = 1; i < fields.Count; i++)
                {
                    var element = fields[i];

                    objectElement.Add(element.SysName, element.Value);
                }

                (data[Resources.RestKeyObjectValues] as JObject).Add(Resources.RestKeyFields, objectElement);
            }

            if (columns.Count != 0)
            {
                var arrayelement = new JArray();

                foreach (var element in columns)
                {
                    var objectElement = new JObject
                        {
                            { element.ColumnsElement[0].SysName, element.ColumnsElement[0].Value }
                        };

                    for (var i = 1; i < element.ColumnsElement.Count; i++)
                    {
                        var item = element.ColumnsElement[i];

                        objectElement.Add(item.SysName, item.Value);
                    }

                    arrayelement.Add(objectElement);
                }

                (data[Resources.RestKeyObjectValues] as JObject).Add(Resources.RestKeyColumns, arrayelement);
            }

            return isHeaderRequestBody ? new JObject() { { Resources.RestKeyRequestBody, data.ToString() } } 
                                       : data;
        }

        /// <summary>
        /// Метод запуска сервиса синхронизации БД
        /// </summary>
        private static void StartSync() => DependencyService.Get<IWorkService>().StartForegroundServiceCompact(ServiceEnums.Sync);

        /// <summary>
        /// Метод остановки сервиса синхронизации БД
        /// </summary>
        private static void StopSync() => DependencyService.Get<IWorkService>().StopForegroundServiceCompact(ServiceEnums.Sync);
        #endregion
    }
}
