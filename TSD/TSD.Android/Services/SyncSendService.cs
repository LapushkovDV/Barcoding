using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using TSD.Droid.Services.Dependency;
using TSD.Model;
using TSD.Services;
using TSD.Services.DataBase;
using TSD.Services.Extensions;
using TSD.Services.Rest;
using Xamarin.Essentials;
using Xamarin.Forms;
using ResourcesTerminal = TSD.Resources;

namespace TSD.Droid.Services
{
    /// <summary>
    /// Сервис для постоянной работы с сервером и проверка выполнения задачи по документам
    /// </summary>
    [Service(Label = "SyncService")]
    public class SyncSendService : Service
    {
        #region Приватные поля
        private readonly AccountManagerDroid _manager = new AccountManagerDroid();
        private bool _isTimer = false;
        #endregion

        #region Команды сервиса
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            _isTimer = true;

            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                try
                {
                    switch (Connectivity.NetworkAccess)
                    {
                        case NetworkAccess.Internet:
                        case NetworkAccess.Local:
                        case NetworkAccess.ConstrainedInternet:
                        {
                                var documentTasks = WorkDataBase.GetDocTasks().ToObservableCollection();

                                foreach (var document in documentTasks)
                                {
                                    if (document.StatusId == (int)StatusEnum.SendInERP)
                                    {
                                        var response = Task.Run(async () => await RestService.SendRequest(ServiceDevice.BaseURL + ResourcesTerminal.UrlGetStatus,
                                            HttpMethodRest.POST,
                                            new JObject
                                            {
                                                { ResourcesTerminal.RestKeyGuidLower, document.GuidStatus }
                                            },
                                            _manager.GetValue(ResourcesTerminal.RestKeyToken).ToString())).Result;

                                        if (response != null)
                                        {
                                            if (response.ContainsKey(ResourcesTerminal.RestKeyStatusLower) &&
                                                response[ResourcesTerminal.RestKeyStatusLower].ToString() == ResourcesTerminal.RestStatusOk)
                                            {
                                                document.StatusId = (int)StatusEnum.Complete;
                                                document.GuidStatus = string.Empty;

                                                if (_isTimer)
                                                {
                                                    WorkDataBase.SaveStatusDocumentById(StatusEnum.Complete, document.Id);
                                                    MessagingCenter.Send(new MessageClass<string>(ResourcesTerminal.OK), ResourcesTerminal.MsgCenterTagNetwork);
                                                }
                                            }
                                        }


                                    }

                                    if (document.StatusId == (int)StatusEnum.InSyncToServer)
                                    {
                                        if (_isTimer)
                                        {
                                            ExecuterAction.ExecuteBackgroundOfflineAsync(document);
                                            MessagingCenter.Send(new MessageClass<string>(ResourcesTerminal.OK), ResourcesTerminal.MsgCenterTagNetwork);
                                        }
                                    }
                                }
                                break;
                            }
                        case NetworkAccess.None:
                            {
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                return _isTimer;
            });

            return StartCommandResult.Sticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnDestroy()
        {
            _isTimer = false;
            StopSelf();
            base.OnDestroy();
        }
        #endregion
    }
}