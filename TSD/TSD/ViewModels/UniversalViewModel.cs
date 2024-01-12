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
using TSD.Model.Setting;
using TSD.Model.User;
using TSD.Services;
using TSD.Services.DataBase;
using TSD.Services.Extensions;
using TSD.Services.FileServices;
using TSD.Services.Interfaces;
using TSD.Services.Rest;
using TSD.Services.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Device = Xamarin.Forms.Device;

namespace TSD.ViewModels
{
    /// <summary>
    /// FlyoutPage - Универсальная форма для пунктов меню (основная форма)
    /// </summary>
    public class UniversalViewModel : ViewModelBase
    {
        #region Приватные поля
        private bool _isSelectDocNetwork = false;
        private bool _isSelectDocLocal = false;
        private bool _isActionsVisible = false;
        private bool _isAddElementVisible = false;
        private bool _isOnlyBarcode = false;
        private bool _isUp = false;
        private bool _isDown = false;
        private bool _isOpenDetail = false;
        private bool _isOpenFilter = false;
        private string _textSearch = Resources.PlaceHolderBarcodeScan;
        private string _identification = string.Empty;
        private string _searchText = string.Empty;
        private string _scan = string.Empty;
        private bool _isRefreshing = false;
        private bool _isEnabledView = true;
        private int _page = 1;
        private readonly int _pageSize = (int)Math.Ceiling((DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density) / 70); // строки в таблице
        private int _maxPage = 0;
        private ObservableCollection<DocumentTask> _documentsLocal = new ObservableCollection<DocumentTask>();
        private ObservableCollection<AbstractColumns> _elementsColumns = new ObservableCollection<AbstractColumns>();
        private bool _isUsbWork = false;
        private string _filterText = string.Empty;
        private bool _isBlock = false;
        #endregion

        #region Команды
        /// <summary>
        /// Команда подтверждения действия
        /// </summary>
        public ICommand AcceptDocument { get; set; }

        /// <summary>
        /// Команда отмены действия
        /// </summary>
        public ICommand CancelCommand { get; set; }

        /// <summary>
        /// Команда по открытию окна по добавлению элементов в таблицу
        /// </summary>
        public ICommand AddElements { get; set; }

        /// <summary>
        /// Команда по добавлению элемента в таблицу
        /// </summary>
        public ICommand AddElement { get; set; }

        /// <summary>
        /// Команда по открытию окна с действиями (операциями)
        /// </summary>
        public ICommand ActionsCommand { get; set; }

        /// <summary>
        /// Команда по открытию документа
        /// </summary>
        public ICommand OpenDoc { get; set; }

        /// <summary>
        /// Команда по обновлению данных в таблице документа
        /// </summary>
        public ICommand RefreshList { get; set; }

        /// <summary>
        /// Команда по удалению текущего документа из БД
        /// </summary>
        public ICommand DeleteDoc { get; set; }
        /// <summary>
        /// Инкремент для пагинации
        /// </summary>
        public ICommand UpPage { get; set; }
        /// <summary>
        /// Декремент для пагинации
        /// </summary>
        public ICommand DownPage { get; set; }
        /// <summary>
        /// Пакетная загрузка документов
        /// </summary>
        public ICommand BatchLoad { get; set; }
        /// <summary>
        /// Открыть окно фильтров
        /// </summary>
        public ICommand OpenFilter { get; set; }

        /// <summary>
        /// Открыть окно фильтров
        /// </summary>
        public ICommand AcceptFilter { get; set; }

        public ICommand CancelFilter { get; set; }
        public ICommand ResetFilter { get; set; }
        #endregion

        #region Конструкторы
        public UniversalViewModel()
        {
            AcceptDocument = new Command(x => MultiGetDocument(null));
            CancelCommand = new Command(CancelView);
            ActionsCommand = new Command(ActionsCommandMethod);
            OpenDoc = new Command(OpenDocMethod, () => IsEnabledView);
            AddElements = new Command(AddElementsMethod, () => IsEnabledView);
            AddElement = new Command(AddElementMethod);
            RefreshList = new Command(UpdateDocumentLocal);
            DeleteDoc = new Command(DeleteDocMethod, () => IsEnabledView);
            UpPage = new Command(UpPageMethod, () => _page != _maxPage);
            DownPage = new Command(DownPageMethod, () => _page != 1);
            BatchLoad = new Command(BatchLoadMethod, () => IsBatchLoad);
            OpenFilter = new Command(OpenFilterMethod);
            AcceptFilter = new Command(UpdateDocumentLocal);
            CancelFilter = new Command(() => CancelFilterMethod());
            ResetFilter = new Command(ResetFilterMethod);


            MessagingCenter.Subscribe<MessageClass<object>>(this, Resources.MsgCenterTagIsVisibleHomeUpdate, x =>
            {
                UserAccount.SelectedMenu = null;

                RaisePropertyChanged(nameof(IsVisibleHome));
            });

            MessagingCenter.Subscribe<MessageClass<object>>(this, Resources.MsgCenterTagLoading, x =>
            {
                if (!IsActivity)
                    StartLoading(Resources.MessageStatusSyncOperation);
                else
                    StopLoading();
            });

            MessagingCenter.Subscribe<MessageClass<bool>>(this, "DictionaryLoading", x =>
            {
                if (x.Data)
                    StartLoading("Загрузка справочников...");
                else
                    StopLoading();
            });

            MessagingCenter.Subscribe<MessageClass<bool>>(this, "DocumentsLoading", x =>
            {
                if (x.Data)
                    StartLoading("Загрузка документов...");
                else
                    StopLoading();
            });

            MessagingCenter.Subscribe<MessageClass<object>>(this, Resources.MsgCenterTagCancelView, x =>
            {
                CancelView();
            });
            MessagingCenter.Subscribe<MessageClass<object>>(this, Resources.MsgCenterTagSyncRow, x =>
            {
                RaisePropertyChanged(nameof(ElementsActions));
            });
            MessagingCenter.Subscribe<MessageClass<object>>(this, Resources.MsgCenterTagMenuLoad, x =>
            {
                if (!IsActivity)
                    StartLoading(Resources.MessageStatusLoadingMenu);
                else
                    StopLoading();
            });

            MessagingCenter.Subscribe<MessageClass<bool>>(this, Resources.MsgCenterTagLoadingView, x =>
            {
                if (x.Data)
                    StartLoading();
                else
                    StopLoading();
            });

            MessagingCenter.Subscribe<MessageClass<object>>(this, Resources.MsgCenterTagUpdateUniversal, x =>
            {
                RefreshList.Execute(null);
            });

            MessagingCenter.Subscribe<MessageClass<bool>>(this, Resources.MsgCenterTagIsEnabledView, x =>
            {
                IsEnabledView = x.Data;
                UpdateToolbars();
            });
            MessagingCenter.Subscribe<MessageClass<string>>(this, Resources.MsgCenterTagSendBarcode, x =>
            {
                if (UserAccount.SelectedMenu != null)
                {
                    if (!_isOpenDetail)
                    {
                        if (UserAccount.SelectedMenu.CurrentDocument != null &&
                            UserAccount.SelectedMenu.CurrentDocument.Id != 0)
                        {
                            if (IsAddElementVisible)
                            {
                                Identification = x.Data;

                                ((Command)AddElement).Execute(null);
                            }
                            else if (IsSelectDocNetwork)
                            {
                                Identification = x.Data;
                            }
                            else if (IsOpenFilter)
                            {
                                FilterText = x.Data;
                            }
                            else
                            {
                                Device.BeginInvokeOnMainThread(() => Scanner(x.Data));
                            }
                        }
                        else
                            Device.BeginInvokeOnMainThread(() => LoadDocByIdent(x.Data));
                    }
                    else
                    {
                        if (IsSelectDocNetwork)
                        {
                            Identification = x.Data;
                        }
                        else
                        {
                            MessagingCenter.Send(new MessageClass<string>(x.Data.Trim()), Resources.MsgCenterTagPasteData);
                        }   
                    }
                }

                UserAccount.IsBlock = false;


            });

            MessagingCenter.Subscribe<MessageClass<bool>>(this, Resources.MsgCenterTagIsOpenDetail, x =>
            {
                _isOpenDetail = x.Data;
            });
            MessagingCenter.Subscribe<MessageClass<object>>(this, Resources.MsgCenterTagCloseContentDialog, x =>
            {
                CloseContentView();

                if (!_isOpenDetail)
                    MessagingCenter.Send(new MessageClass<object>(), Resources.MsgCenterTagCancelSearch);
            });

            MessagingCenter.Subscribe<MessageClass<object>>(this, Resources.MsgCenterUpdateTypeTransfer, x => 
            {
                var isParse = int.TryParse(_manager.GetValue(Resources.RestTypeTransfer), out int indexType);

                if (isParse)
                {
                    _isUsbWork = !SettingTSD.TypeTransfers[indexType].IsNetwork;
                }
            });
            

            MessagingCenter.Subscribe<MessageClass<object>>(this, "LoadDocuments", x =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var result = 0;

                    await Task.Run(() => {
                        DependencyService.Get<IWorkService>().StopForegroundServiceCompact(ServiceEnums.Sync);
                        MessagingCenter.Send(new MessageClass<bool>(true), "DocumentsLoading");
                        result = LoadDocuments();
                        MessagingCenter.Send(new MessageClass<bool>(false), "DocumentsLoading");

                        MessagingCenter.Send(new MessageClass<bool>(true), "DictionaryLoading");
                        result += LoadDictionaries();
                        MessagingCenter.Send(new MessageClass<bool>(false), "DictionaryLoading");

                        DependencyService.Get<IWorkService>().StartForegroundServiceCompact(ServiceEnums.Sync);

                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            if (result > 0)
                            {
                                await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageUpdateNewDoc, Resources.OK);
                            }
                        });
                    });         
                });
            });

        }
        #endregion

        #region Свойства для чтения
        /// <summary>
        /// Заголовок универсальной форм
        /// </summary>
        public string HomeTitle { get; set; } = Resources.TitleHome;

        /// <summary>
        /// Видимость колонок
        /// </summary>
        public bool IsVisibleColumns => UserAccount.SelectedMenu != null && UserAccount.SelectedMenu.Columns.Count != 0;

        /// <summary>
        /// Видимость полей
        /// </summary>
        public bool IsVisibleFields => UserAccount.SelectedMenu != null && UserAccount.SelectedMenu.Fields.Count != 0;

        /// <summary>
        /// Видимость кнопок
        /// </summary>
        public bool IsVisibleActions => UserAccount.SelectedMenu != null && UserAccount.SelectedMenu.Actions.Count != 0;

        /// <summary>
        /// Видимость всей формы
        /// </summary>
        public bool IsVisibleHome => UserAccount.SelectedMenu != null && UserAccount.SelectedMenu.CurrentDocument?.IdDoc != string.Empty;

        /// <summary>
        /// Разрешение на добавление строк в таблицу с колонками
        /// </summary>
        public bool IsAllowAddRow => UserAccount.SelectedMenu != null && UserAccount.SelectedMenu.CurrentDocument?.IdDoc != string.Empty && UserAccount.SelectedMenu.AllowAddRows && UserAccount.SelectedMenu.CurrentDocument.IsAllowEditDoc;

        /// <summary>
        /// Заголовок для полей с основной информацией по объекту
        /// </summary>
        public string HeaderFields => UserAccount.SelectedMenu != null && UserAccount.SelectedMenu.Fields.Count != 0 ?
            string.Join(", ", UserAccount.SelectedMenu.Fields.Where(x => x.Row == UserAccount.SelectedMenu.Fields.Min(element => element.Row)).Select(x => $"{x.Name}: {x.Value}")) : string.Empty;

        /// <summary>
        /// Проверка на то, что один из пунктов меню был выбран
        /// </summary>
        public bool IsSelectMenu => !(UserAccount.SelectedMenu is null);

        /// <summary>
        /// Поля текущего объекта (документа)
        /// </summary>
        public ObservableCollection<AbstractField> ElementsFields => UserAccount.SelectedMenu != null ? UserAccount.SelectedMenu.Fields.Where(x => x.BrowseCard).OrderBy(x => x.Row).ToObservableCollection() : new ObservableCollection<AbstractField>();

        /// <summary>
        /// Действия, которые можно совершать над объектом (документом)
        /// </summary>
        public ObservableCollection<AbstractAction> ElementsActions => UserAccount.SelectedMenu != null ? UserAccount.SelectedMenu.Actions.OrderBy(x => x.Row).ToObservableCollection() : new ObservableCollection<AbstractAction>();

        /// <summary>
        /// Заголовки колонок таблицы
        /// </summary>
        public ObservableCollection<AbstractColumn> HeadersColumns => UserAccount.SelectedMenu != null
            ? UserAccount.SelectedMenu.Columns.Where(x => x.Browse == 1).ToObservableCollection()
            : new ObservableCollection<AbstractColumn>();

        /// <summary>
        /// Свойство для label "Для чтения", если документ для чтения
        /// </summary>
        public bool IsReadOnlyDoc => UserAccount.SelectedMenu != null && !UserAccount.SelectedMenu.CurrentDocument.IsAllowEditDoc;

        /// <summary>
        /// Свойство, доступна ли пакетная загрузка
        /// </summary>
        public bool IsBatchLoad => UserAccount.SelectedMenu != null && UserAccount.SelectedMenu.IsBatchLoad && !UserAccount.IsUsbWork;

        /// <summary>
        /// Свойство, если не документ поля разворачивать
        /// </summary>
        public bool IsExpand => UserAccount.SelectedMenu != null && !UserAccount.SelectedMenu.IsDoc;
        
        /// <summary>
        /// Свойство, если документ
        /// </summary>
        public bool IsOpenDoc => UserAccount.SelectedMenu != null && UserAccount.SelectedMenu.CurrentDocument?.IdDoc != string.Empty && UserAccount.SelectedMenu.IsDoc;
        #endregion

        #region Свойства
        /// <summary>
        /// Колонки объекта (документа) - для реализации таблицы
        /// </summary>
        public ObservableCollection<AbstractColumns> ElementsColumns
        {
            get => _elementsColumns;
            set => SetProperty(ref _elementsColumns, value);
        }

        /// <summary>
        /// Список локальных документов
        /// </summary>
        public ObservableCollection<DocumentTask> DocumentsLocal
        {
            get => _documentsLocal;
            set => SetProperty(ref _documentsLocal, value);
        }

        /// <summary>
        /// Выбранный локальный документ
        /// </summary>
        public DocumentTask SelectDocumentLocal
        {
            get => null;
            set => MultiGetDocument(value);
        }

        /// <summary>
        /// Окно с вводом идентификатора - показывать или не показывать окно
        /// </summary>
        public bool IsSelectDocNetwork
        {
            get => _isSelectDocNetwork;
            set
            {
                SetProperty(ref _isSelectDocNetwork, value);

                IsGestureEnabled();
            }
        }

        /// <summary>
        /// Окно со списком документов из локальной БД - показывать или не показывать окно
        /// </summary>
        public bool IsSelectDocLocal
        {
            get => _isSelectDocLocal;
            set
            {
                SetProperty(ref _isSelectDocLocal, value);

                IsGestureEnabled();
            }
        }

        /// <summary>
        /// Окно с операциями (действиями) - показывать или не показывать окно
        /// </summary>
        public bool IsActionsVisible
        {
            get => _isActionsVisible;
            set
            {
                SetProperty(ref _isActionsVisible, value);

                IsGestureEnabled();
            }
        }

        /// <summary>
        /// Окно добавления эелементов (спецификаций) - показывать или не показывать
        /// </summary>
        public bool IsAddElementVisible
        {
            get => _isAddElementVisible;
            set
            {
                SetProperty(ref _isAddElementVisible, value);

                IsGestureEnabled();
            }
        }

        /// <summary>
        /// Поле для ввода идентификатора
        /// </summary>
        public string Identification
        {
            get => _identification;
            set => SetProperty(ref _identification, value);
        }

        /// <summary>
        /// Поле поиска
        /// </summary>
        public string TextSearch
        {
            get => _textSearch;
            set => SetProperty(ref _textSearch, value);
        }

        /// <summary>
        /// Переключатель поиска - если true - то поиск по штрихкоду, иначе по текстовому вводу в поле
        /// </summary>
        public bool IsOnlyBarcode
        {
            get => _isOnlyBarcode;
            set
            {
                SetProperty(ref _isOnlyBarcode, value);

                TextSearch = !IsOnlyBarcode ? Resources.PlaceHolderBarcodeScan : Resources.PlaceHolderSearch;

                //MessagingCenter.Send(new MessageClass<object>(), Resources.MsgCenterTagCancelSearch);
            }
        }

        /// <summary>
        /// Условие для обновления таблицы (ListView)
        /// </summary>
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        /// <summary>
        /// Условие активности основной формы
        /// </summary>
        public bool IsEnabledView
        {
            get => _isEnabledView;
            set => SetProperty(ref _isEnabledView, value);
        }
        public int Page
        {
            get => _page;
            set
            {
                SetProperty(ref _page, value);
            }
        }
        public int MaxPage
        {
            get => _maxPage;
            set => SetProperty(ref _maxPage, value);
        }
        public bool IsUp
        {
            get => _isUp;
            set => SetProperty(ref _isUp, value);
        }
        public bool IsDown
        {
            get => _isDown;
            set => SetProperty(ref _isDown, value);
        }

        public string FilterText
        {
            get => _filterText;
            set => SetProperty(ref _filterText, value);
        }

        public bool IsOpenFilter
        {
            get => _isOpenFilter;
            set => SetProperty(ref _isOpenFilter, value);
        }

        public bool IsBlock
        {
            get => _isBlock;
            set => SetProperty(ref _isBlock, value);
        }
        #endregion

        #region Приватные методы (для команд и внутренние)
        /// <summary>
        /// Загрузка документов (два режима: из локальной БД, из сети по идентификатору)
        /// </summary>
        /// <param name="documentTask"></param>
        private void MultiGetDocument(DocumentTask documentTask)
        {
            if (IsSelectDocLocal)
            {
                GetDocumentLocal(documentTask);
            }
            else
            {
                if (UserAccount.IsUsbWork)
                {
                    GetDocumentUsb();
                }
                else
                {
                    if (UserAccount.SelectedMenu != null && UserAccount.SelectedMenu.Actions.Count != 0)
                    {
                        GetDocument();
                    }
                    else
                    {
                        GetDocument(!ServiceDevice.IsNetwork);
                    }
                }

            }
        }

        /// <summary>
        /// Метод для загрузки документа из локальной БД
        /// </summary>
        /// <param name="documentTask"></param>
        private async void GetDocumentLocal(DocumentTask documentTask)
        {
            if (!(documentTask is null))
            {
                Pagination(documentTask.Id, (_page - 1) * _pageSize, _pageSize);
                CancelView();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageSelectDocInList, Resources.OK);
            }
        }

        /// <summary>
        /// Обновление данных документа в таблице
        /// </summary>
        private void UpdateDocumentLocal()
        {
            StartLoading(Resources.MessageStatusLoading);
            
            if (UserAccount.SelectedMenu.CurrentDocument != null && UserAccount.SelectedMenu.CurrentDocument.Id != 0)
            {
                if (UserAccount.SelectedMenu.IsDoc)
                {
                    Pagination(UserAccount.SelectedMenu.CurrentDocument.Id, (_page - 1) * _pageSize, _pageSize, FilterText != string.Empty ? FilterText : null);

                }
                else
                {
                    WorkDataBase.LoadCurrentDictionary(UserAccount.SelectedMenu.CurrentDocument.Id);

                    
                }
            }
            
            IsRefreshing = false;

            StopLoading();

            CancelFilterMethod(false);
        }

        /// <summary>
        /// Метод для загрузки документа сети или локальной БД
        /// </summary>
        /// <param name="isLocal"></param>
        private async void GetDocument(bool isLocal = false)
        {

            if (Identification == null || Identification == string.Empty)
            {
                return;
            }

            JObject response, valuesJson = null;

            var isOk = false;
            var isMultiLoad = false;
            var menu = UserAccount.SelectedMenu;

            if (UserAccount.SelectedMenu != null)
            {
                StartLoading(Resources.MessageStatusComplitingQuery);

                Page = 1;

                try
                {
                    if (!isLocal)
                    {
                        var json = new JObject
                    {
                        { Resources.RestKeyActionNameLow, menu.Action },
                        { Resources.RestKeyIdent, Identification.Trim() }
                    };

                        response = await RestService.SendRequest(ServiceDevice.BaseURL + Resources.UrlGetObjectById, HttpMethodRest.POST, json,
                              _manager.GetValue(Resources.RestKeyToken));

                        if (response != null && response.ContainsKey(Resources.RestKeyDataContext))
                        {
                            if (response[Resources.RestKeyDataContext].ToString() != string.Empty)
                            {
                                valuesJson = JObject.Parse(RestService.Decompress(response[Resources.RestKeyValue].ToString()));

                                if (valuesJson[Resources.RestKeyResultMsgDescr].ToString() == Resources.OK)
                                {
                                    if (valuesJson[Resources.RestKeyObjectValues] is JObject)
                                    {
                                        var jsonObject = valuesJson[Resources.RestKeyObjectValues] as JObject;
                                        var jsonFields = jsonObject.ContainsKey(Resources.RestKeyFields) ? JObject.Parse(jsonObject[Resources.RestKeyFields].ToString()) : new JObject();
                                        var jsonColumns = jsonObject.ContainsKey(Resources.RestKeyColumns) ? JArray.Parse(jsonObject[Resources.RestKeyColumns].ToString()) : new JArray();

                                        isOk = LoadDocMenu(menu, jsonObject, jsonFields, jsonColumns);
                                    }
                                    else if (valuesJson[Resources.RestKeyObjectValues] is JArray)
                                    {
                                        if ((valuesJson[Resources.RestKeyObjectValues] as JArray).Count == 1)
                                        {
                                            var jsonObject = (valuesJson[Resources.RestKeyObjectValues] as JArray).First as JObject;
                                            var jsonFields = jsonObject.ContainsKey(Resources.RestKeyFields) ? JObject.Parse(jsonObject[Resources.RestKeyFields].ToString()) : new JObject();
                                            var jsonColumns = jsonObject.ContainsKey(Resources.RestKeyColumns) ? JArray.Parse(jsonObject[Resources.RestKeyColumns].ToString()) : new JArray();

                                            isOk = LoadDocMenu(menu, jsonObject, jsonFields, jsonColumns);
                                        }
                                        else
                                        {
                                            isMultiLoad = true;

                                            foreach (JObject jsonObject in valuesJson[Resources.RestKeyObjectValues])
                                            {
                                                var jsonFields = jsonObject.ContainsKey(Resources.RestKeyFields) ? JObject.Parse(jsonObject[Resources.RestKeyFields].ToString()) : new JObject();
                                                var jsonColumns = jsonObject.ContainsKey(Resources.RestKeyColumns) ? JArray.Parse(jsonObject[Resources.RestKeyColumns].ToString()) : new JArray();

                                                if (LoadDocMenu(menu, jsonObject, jsonFields, jsonColumns))
                                                {
                                                    WorkDataBase.SaveCurrentDocument(menu.Actions.Count == 0 ? StatusEnum.Dictionary : StatusEnum.InWork);
                                                }
                                            }

                                            UserAccount.SelectedMenu.Fields = UserAccount.SelectedMenu.Fields.Select(x =>
                                            {
                                                x.Value = string.Empty; return x;
                                            }).ToObservableCollection();
                                            UserAccount.SelectedMenu.ColumnsValue.Clear();
                                            UserAccount.SelectedMenu.CurrentDocument = new CurrentDocument();
                                        }
                                    }
                                }

                                if (isOk)
                                {
                                    CancelView();

                                    UserAccount.SelectedMenu.CurrentDocument.Id = WorkDataBase.SaveCurrentDocument(StatusEnum.InWork);
                                    Pagination(UserAccount.SelectedMenu.CurrentDocument.Id, (_page - 1) * _pageSize, _pageSize);
                                }
                                else if (isMultiLoad)
                                {
                                    CancelView();

                                    await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MsgResultMultiDocLoad, Resources.OK);

                                    if (UpdateDocuments())
                                    {
                                        IsSelectDocLocal = !IsSelectDocLocal;
                                    }
                                    else
                                    {
                                        await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageDataBaseMultiNoDoc, Resources.OK);
                                    }
                                }
                                else
                                {
                                    CancelView();

                                    await Application.Current.MainPage.DisplayAlert(Resources.Information, valuesJson[Resources.RestKeyResultMsg].ToString(), Resources.OK);
                                }
                            }
                            else
                            {
                                await Application.Current.MainPage.DisplayAlert(Resources.Information, response.ContainsKey(Resources.RestKeyValue)
                                        ? response[Resources.RestKeyValue].ToString() : Resources.MessageUnknownErrorQuery, Resources.OK);
                            }
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageWarinigNullResponseAction + " " + menu.Action, Resources.OK);
                        }
                    }
                    else
                    {
                        if (!LoadDocMenuLocal(Identification))
                        {
                            await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageDataBaseNoDoc, Resources.OK);
                        }

                        CancelView();
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { nameof(UniversalViewModel), nameof(GetDocument) } });
                }
                StopLoading();
            }
        }

        /// <summary>
        /// Перечитывание свойств представления универсальной формы
        /// </summary>
        private void UpdateProperties()
        {
            ElementsColumns = UserAccount.SelectedMenu.ColumnsValue.Select(x => { x.UpdateProperties(); return x; }).ToObservableCollection();
            RaisePropertyChanged(nameof(IsVisibleColumns));
            RaisePropertyChanged(nameof(IsVisibleFields));
            RaisePropertyChanged(nameof(IsVisibleActions));
            RaisePropertyChanged(nameof(IsVisibleHome));
            RaisePropertyChanged(nameof(ElementsFields));
            RaisePropertyChanged(nameof(ElementsColumns));
            RaisePropertyChanged(nameof(ElementsActions));
            RaisePropertyChanged(nameof(HeadersColumns));
            RaisePropertyChanged(nameof(HeaderFields));
            RaisePropertyChanged(nameof(HeadersColumns));
            RaisePropertyChanged(nameof(HeaderFields));
            RaisePropertyChanged(nameof(IsAllowAddRow));
            RaisePropertyChanged(nameof(IsReadOnlyDoc));
            RaisePropertyChanged(nameof(IsOpenDoc));
        }

        /// <summary>
        /// Обновление состояния меню
        /// </summary>
        private void UpdateToolbars()
        {
            ((Command)OpenDoc).ChangeCanExecute();
            ((Command)DeleteDoc).ChangeCanExecute();
            ((Command)AddElements).ChangeCanExecute();
        }

        /// <summary>
        /// Выбор типа загрузки документа (true - если с локальной БД, иначе с сети
        /// </summary>
        /// <returns></returns>
        private async Task<TypeLoadDoc> SelectDocIdent()
        {
            var menuActionsDoc = new List<string>() { Resources.MessageIdentificatorSelect };

            if (ServiceDevice.IsNetwork && !UserAccount.IsUsbWork)
            {
                if (UserAccount.SelectedMenu.IsBatchLoad)
                    menuActionsDoc.Add(Resources.MessageBatchSelect);

                if (UpdateDocuments() && DocumentsLocal != null && DocumentsLocal.Count != 0)
                    menuActionsDoc.Add(Resources.MessageDataBaseSelect);

                StopLoading();

                if (menuActionsDoc.Count > 1)
                {
                    var select = await Application.Current.MainPage.DisplayActionSheet(Resources.Information, Resources.Cancel, null, menuActionsDoc.ToArray());

                    if (select == Resources.MessageIdentificatorSelect)
                        return TypeLoadDoc.Identificator;
                    else if (select == Resources.MessageBatchSelect)
                        return TypeLoadDoc.BatchLoad;
                    else if (select == Resources.MessageDataBaseSelect)
                        return TypeLoadDoc.LocalBD;
                    else
                        return TypeLoadDoc.Cancel;
                }
                else
                    return TypeLoadDoc.Identificator;
            }
            else if (UserAccount.IsUsbWork)
            {
                if (UpdateDocuments() && DocumentsLocal != null && DocumentsLocal.Count != 0)
                    menuActionsDoc.Add(Resources.MessageDataBaseSelect);

                StopLoading();

                if (menuActionsDoc.Count > 1 && UserAccount.SelectedMenu.IsDoc)
                {
                    var select = await Application.Current.MainPage.DisplayActionSheet(Resources.Information, Resources.Cancel, null, menuActionsDoc.ToArray());

                    if (select == Resources.MessageIdentificatorSelect)
                        return TypeLoadDoc.Identificator;
                    else if (select == Resources.MessageDataBaseSelect)
                        return TypeLoadDoc.LocalBD;
                    else
                        return TypeLoadDoc.Cancel;
                }
                else
                    return TypeLoadDoc.Identificator;
            }
            else
            {
                StopLoading();

                if (UpdateDocuments() && DocumentsLocal != null && DocumentsLocal.Count != 0)
                    return TypeLoadDoc.LocalBD;
                else
                {
                    if (UserAccount.SelectedMenu.Actions.Count != 0)
                        await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageBlockWork, Resources.OK);

                    return TypeLoadDoc.Cancel;
                }
            }
        }

        /// <summary>
        /// Определение окна загрузки документа (Окно загрузки сети или окно загрузки с локальной БД)
        /// </summary>
        public async void SwitchSelectDocView()
        {
            IsSelectDocLocal = false;
            IsSelectDocNetwork = false;

            var typeDocLoad = await SelectDocIdent();

            if (UserAccount.SelectedMenu.Actions.Count != 0 && typeDocLoad != TypeLoadDoc.LocalBD && !ServiceDevice.IsNetwork)
            {
                return;
            }
            else
            {
                if (typeDocLoad == TypeLoadDoc.LocalBD)
                {
                    IsSelectDocLocal = !IsSelectDocLocal;
                }
                else if (typeDocLoad == TypeLoadDoc.Identificator)
                {
                    IsSelectDocNetwork = !IsSelectDocNetwork;
                }
                else if (typeDocLoad == TypeLoadDoc.BatchLoad)
                {
                    if (IsUsbWork)
                    {
                        Identification = Resources.BatchLoad;
                        GetDocumentUsb();
                    }
                    else
                    {
                        Identification = Resources.BatchLoad;
                        GetDocument(false);
                    }
                }
                else
                {
                    return;
                }

            }
        }


        /// <summary>
        /// Закрытие открытых окон и очистка поля идентификатора
        /// </summary>
        public void CancelView()
        {
            IsSelectDocLocal = false;
            IsSelectDocNetwork = false;
            IsActionsVisible = false;
            IsAddElementVisible = false;
            Identification = string.Empty; 
            FilterText = string.Empty;
            
            if (IsOpenFilter)
            {
                IsOpenFilter = false;
                IsBlock = false;
                
                IsGestureEnabled();

                RefreshList.Execute(null);
            }
            
        }

        public void CancelFilterMethod(bool isClear = true)
        {
            IsOpenFilter = false;
            IsBlock = false;

            IsGestureEnabled();

            if (isClear)
            {
                FilterText = string.Empty;
            }
        }

        private void ResetFilterMethod()
        {
            FilterText = string.Empty;
            
            if (IsOpenFilter)
            {
                IsOpenFilter = false;
                IsBlock = false;

                IsGestureEnabled();

                RefreshList.Execute(null);
            }
        }


        /// <summary>
        /// Метод открытия окна с операциями
        /// </summary>
        public void ActionsCommandMethod() => IsActionsVisible = true;

        /// <summary>
        /// Выгрузка всех документов в окно с документами
        /// </summary>
        /// <returns></returns>
        private bool UpdateDocuments()
        {
            if (UserAccount.SelectedMenu.Actions.Count != 0)
            {
                DocumentsLocal = WorkDataBase.GetDocTasks(UserAccount.SelectedMenu, 2).ToObservableCollection();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Метод загрузки документа на форму
        /// </summary>
        private void OpenDocMethod()
        {
            if (UserAccount.SelectedMenu != null)
            {
                SwitchSelectDocView();
            }
        }

        /// <summary>
        /// Метод загрузки документа на форму
        /// </summary>
        private void OpenFilterMethod()
        {
            IsOpenFilter = true;
            IsBlock = true;

            IsGestureEnabled();
        }

        /// <summary>
        /// Загрузка документа в пункт меню из JSON
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="jsonObject"></param>
        /// <param name="jsonFields"></param>
        /// <param name="jsonColumns"></param>
        /// <returns></returns>
        private bool LoadDocMenu(AbstractMenu menu, JObject jsonObject, JObject jsonFields, JArray jsonColumns)
        {
            UserAccount.SelectedMenu.CurrentDocument.IdDoc = jsonObject[Resources.RestKeyIdObject].ToString();
            UserAccount.SelectedMenu.CurrentDocument.Barcode = jsonObject.ContainsKey(Resources.RestKeyBarcode) ? jsonObject[Resources.RestKeyBarcode].ToString() : string.Empty;
            UserAccount.SelectedMenu.CurrentDocument.IsAllowEditDoc = !jsonObject.ContainsKey(Resources.RestKeyAllowModify) ||
                bool.Parse(jsonObject[Resources.RestKeyAllowModify].ToString());

            menu.Fields = menu.Fields.Select(x =>
            {
                x.Value = jsonFields.ContainsKey(x.SysName) ? jsonFields[x.SysName].ToString() : string.Empty;

                return x;
            }).ToObservableCollection();


            menu.ColumnsValue = jsonColumns.Select(x =>
            {
                var element = x as JObject;
                var columns = new AbstractColumns();

                foreach (var column in menu.Columns)
                {
                    columns.ColumnsElement.Add(new AbstractColumn
                    {
                        Action = column.Action,
                        Modif = UserAccount.SelectedMenu.CurrentDocument.IsAllowEditDoc && column.Modif,
                        Name = column.Name,
                        Npp = column.Npp,
                        Size = column.Size,
                        SysName = column.SysName,
                        WIsActive = column.WIsActive,
                        DataType = column.DataType,
                        Browse = column.Browse,
                        IsModify = column.IsModify,
                        Value = element.ContainsKey(column.SysName) ? element[column.SysName].ToString() : string.Empty,
                        ValueOld = element.ContainsKey(column.SysName) ? element[column.SysName].ToString() : string.Empty,
                        SizeColumn = column.SizeColumn,
                        BrowseCard = column.BrowseCard,
                        Nullable = column.Nullable
                    });
                };

                return columns;
            }).ToObservableCollection();

            return true;
        }

        /// <summary>
        /// Загрузка документа по идетификатору из локальной БД
        /// </summary>
        /// <param name="identificator"></param>
        /// <returns></returns>
        private bool LoadDocMenuLocal(string identificator)
        {
            identificator = identificator.Trim();
            
            if (UserAccount.SelectedMenu.IsDoc)
            {
                var docId = WorkDataBase.GetDocIdByBarcode(identificator);

                if (docId != 0)
                {
                    Pagination(docId, (_page - 1) * _pageSize, _pageSize);

                    return true;
                }
            }
            else
            {
                var docId = WorkDataBase.GetDictionaryIdByBarcode(identificator);
                    
                if (docId != 0)
                {
                    WorkDataBase.LoadCurrentDictionary(docId);

                    UpdateProperties();

                    return true;
                }
            }    

            return false;
        }

        /// <summary>
        /// Метод старта загрузочного экрана, с параметром строки состояния
        /// </summary>
        /// <param name="stateText"></param>
        private void StartLoading(string stateText = "Загрузка...")
        {
            StateText = stateText;
            IsActivity = true;

            IsGestureEnabled();
        }

        /// <summary>
        /// Метод остановки загрузочного экрана
        /// </summary>
        private void StopLoading()
        {
            IsActivity = false;

            IsGestureEnabled();
        }

        /// <summary>
        /// Метод открытия окна добавления элементов в объект (документ)
        /// </summary>
        private async void AddElementsMethod()
        {
            if (UserAccount.SelectedMenu.CurrentDocument.IdDoc != string.Empty)
            {
                var action = UserAccount.SelectedMenu.Columns.FirstOrDefault(x => x.Action != string.Empty).Action;

                if (action != string.Empty)
                {
                    IsAddElementVisible = true;
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageBeginSelectDoc, Resources.OK);
            }
        }

        /// <summary>
        /// Метод добавления эелемент в объект (документ)
        /// </summary>
        public async void AddElementMethod()
        {
            StartLoading("Добавление позиции...");
            try
            {
                if (ServiceDevice.IsNetwork)
                {
                    var json = new JObject
                {
                    { Resources.RestKeyActionNameLow, UserAccount.SelectedMenu.Columns.FirstOrDefault(x => x.Action != string.Empty).Action },
                    { Resources.RestKeyIdent, Identification.Trim() }
                };

                    var response = await RestService.SendRequest(ServiceDevice.BaseURL + Resources.UrlGetObjectById, HttpMethodRest.POST, json, _manager.GetValue(Resources.RestKeyToken));

                    if (response != null && response.ContainsKey(Resources.RestKeyValue))
                    {
                        var value = JObject.Parse(RestService.Decompress(response[Resources.RestKeyValue].ToString()));

                        if (value.ContainsKey(Resources.RestKeyObjectValues))
                        {
                            StartLoading(Resources.MessageStatusAddPosition);

                            var objectValue = JObject.Parse(value[Resources.RestKeyObjectValues].First.ToString());
                            var fields = JsonConvert.DeserializeObject<Dictionary<string, string>>(objectValue[Resources.RestKeyFields].ToString());

                            var element = new DocumentTask
                            {
                                IdDoc = objectValue.ContainsKey(Resources.RestKeyIdObject) ? objectValue[Resources.RestKeyIdObject].ToString() : string.Empty,
                                FieldsTasks = fields.Select(x => new Services.Tasks.Models.FieldsTask
                                {
                                    SysName = x.Key,
                                    Value = x.Value
                                }).ToObservableCollection()
                            };


                            AddRowColumn(element);
                            StopLoading();

                            await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MsgCenterTagAddPosition, Resources.OK);

                            MessagingCenter.Send(new MessageClass<AbstractColumns>(UserAccount.SelectedMenu.ColumnsValue.LastOrDefault()), Resources.MsgCenterTagOpenDetail);

                            IsAddElementVisible = false;
                        }
                        else
                        {
                            if (value.ContainsKey(Resources.RestKeyResultMsg))
                            {
                                await Application.Current.MainPage.DisplayAlert(Resources.Information, value[Resources.RestKeyResultMsg].ToString(), Resources.OK);
                            }
                            else
                                await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.ResultMsgNoPosition, Resources.OK);
                        }
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageUnknownErrorQuery, Resources.OK);
                    }
                }
                else
                {
                    var element = WorkDataBase.GetColumnByIdent(Identification);

                    if (element == null)
                    {
                        AddRowColumn(element);
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageNoNetAndDictionaryEmpty, Resources.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { nameof(UniversalViewModel), nameof(AddElementMethod) } });
            }

            Identification = string.Empty;
            UserAccount.IsBlock = false;
            StopLoading();
        }

        /// <summary>
        /// Метод добавления строк с колонками в список колонок
        /// </summary>
        /// <param name="element"></param>
        private void AddRowColumn(DocumentTask element)
        {
            if (element == null) return;

            UserAccount.SelectedMenu.ColumnsValue.Add(new AbstractColumns());

            var row = UserAccount.SelectedMenu.ColumnsValue.Last();

            foreach (var column in UserAccount.SelectedMenu.Columns)
            {
                row.ColumnsElement.Add(new AbstractColumn
                {
                    Action = column.Action,
                    Modif = column.Modif,
                    Name = column.Name,
                    Npp = column.Npp,
                    Size = column.Size,
                    SysName = column.SysName,
                    WIsActive = column.WIsActive,
                    DataType = column.DataType,
                    Browse = column.Browse,
                    IsModify = column.IsModify,
                    Value = element.FieldsTasks.FirstOrDefault(x => x.SysName == column.SysName)?.Value,
                    ValueOld = element.FieldsTasks.FirstOrDefault(x => x.SysName == column.SysName)?.Value,
                    SizeColumn = column.SizeColumn,
                    IsNew = true
                });
            }

            WorkDataBase.InsertRow(UserAccount.SelectedMenu.CurrentDocument.Id, UserAccount.SelectedMenu.ColumnsValue.Last());

            ElementsColumns = UserAccount.SelectedMenu.ColumnsValue.Select(x => { x.UpdateProperties(); return x; }).ToObservableCollection();
            UpdateProperties();
        }

        /// <summary>
        /// Поиск через сканер
        /// </summary>
        /// <param name="searchText"></param>
        private async void Scanner(string searchText)
        {
            StartLoading();

            if (searchText != null && searchText != string.Empty)
            {
                if (searchText.Length > 1)
                {
                    if (UserAccount.SelectedMenu != null)
                    {
                        var abstractColumns = WorkDataBase.GetColumnsByValue(UserAccount.SelectedMenu.CurrentDocument.Id, searchText);

                        if (abstractColumns != null)
                        {
                            MessagingCenter.Send(new MessageClass<AbstractColumns>(abstractColumns), Resources.MsgCenterTagOpenDetail);
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageUnFoundPosition, Resources.OK);
                        }
                    }
                }
            }
            StopLoading();
        }

        /// <summary>
        /// Метод по удалению открытого документ из БД
        /// </summary>
        private async void DeleteDocMethod()
        {
            if (await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageQuestionDeleteDoc, Resources.Yes, Resources.No))
            {
                StartLoading(Resources.MessageStatusDeleteDoc);

                var id = UserAccount.SelectedMenu.CurrentDocument.Id;
                WorkDataBase.DeleteDocument(id);
                StopLoading();

                MessagingCenter.Send(new MessageClass<object>(), Resources.MsgCenterTagDefaultPage);
            }
        }
        /// <summary>
        /// Метод для команды декремент
        /// </summary>
        private void DownPageMethod()
        {
            Page--;

            if (_searchText != null && _searchText != string.Empty)
            {
                Pagination(UserAccount.SelectedMenu.CurrentDocument.Id, (_page - 1) * _pageSize, _pageSize, _searchText);
            }
            else
            {
                Pagination(UserAccount.SelectedMenu.CurrentDocument.Id, (_page - 1) * _pageSize, _pageSize);
            }
        }
        /// <summary>
        /// Метод для команды инкремент
        ///// </summary>
        private void UpPageMethod()
        {
            Page++;

            if (_searchText != null && _searchText != string.Empty)
            {
                Pagination(UserAccount.SelectedMenu.CurrentDocument.Id, (_page - 1) * _pageSize, _pageSize, _searchText);
            }
            else
            {
                Pagination(UserAccount.SelectedMenu.CurrentDocument.Id, (_page - 1) * _pageSize, _pageSize);
            }
        }

        /// <summary>
        ///Пагинация таблицы
        /// </summary>
        /// <param name="id"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="searchValue"></param>
        private void Pagination(int id, int skip, int take, string searchValue = null)
        {
            MaxPage = (int)Math.Ceiling(WorkDataBase.CountRowDocById(id, searchValue) / (double)_pageSize);

            WorkDataBase.LoadCurrentDocument(id, skip, take, searchValue);
            UpdateProperties();
            ChangeEnableUpDown();
        }

        /// <summary>
        /// Переключатель кликабельности кнопок След. и Пред.
        /// </summary>
        private void ChangeEnableUpDown()
        {
            if (_page > 1)
            {
                IsDown = true;
            }
            else
            {
                IsDown = false;
            }

            if (_page < _maxPage)
            {
                IsUp = true;
            }
            else
            {
                IsUp = false;
            }
        }

        /// <summary>
        /// Загрузка документа по идентификатору (для сканера)
        /// </summary>
        /// <param name="identificator">идентификатор документа</param>
        private async void LoadDocByIdent(string identificator)
        {
            if (identificator != null && identificator != string.Empty && UserAccount.SelectedMenu != null)
            {
                Identification = identificator.Trim();

                var docId = UserAccount.SelectedMenu.IsDoc 
                            ? WorkDataBase.GetDocIdByBarcode(Identification)
                            : WorkDataBase.GetDictionaryIdByBarcode(Identification);

                if (docId != 0)
                {
                    if (!UserAccount.IsUsbWork)
                    {
                        if (await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageQuestionRepeatDownloadDoc, Resources.Yes, Resources.No))
                        {
                            MultiGetDocument(null);
                        }
                        else
                        {
                            LoadDocMenuLocal(Identification);

                            Identification = string.Empty;
                        }
                    }
                    else
                    {
                        if(!LoadDocMenuLocal(Identification))
                        {
                            await Application.Current.MainPage.DisplayAlert(Resources.Information, "По данному идентификатору объект в БД не найден", Resources.OK);
                        }

                        Identification = string.Empty;

                        
                    }
                    
                }
                else
                {
                    if (UserAccount.IsUsbWork)
                    {
                        await Application.Current.MainPage.DisplayAlert(Resources.Information, "По данному идентификатору объект в БД не найден", Resources.OK);
                    }
                    else
                    {
                        MultiGetDocument(null);
                    }          
                }
            }

            UserAccount.IsBlock = false;
        }


        private void IsGestureEnabled()
        {
            if (IsActivity || IsActionsVisible || IsSelectDocNetwork || IsSelectDocLocal || IsAddElementVisible || IsOpenFilter)
            {
                MessagingCenter.Send(new MessageClass<int>(0), Resources.MsgCenterTagGestureFlyout);
                MessagingCenter.Send(new MessageClass<bool>(true), Resources.MsgCenterTagBlockToolbar);
            }
            else
            {
                MessagingCenter.Send(new MessageClass<int>(1), Resources.MsgCenterTagGestureFlyout);
                MessagingCenter.Send(new MessageClass<bool>(false), Resources.MsgCenterTagBlockToolbar);
            }
                

        }

        

        private void BatchLoadMethod()
        {
            Identification = Resources.BatchLoad;

            if (UserAccount.SelectedMenu.IsBatchLoad)
            {
                if (_isUsbWork)
                {
                    GetDocumentUsb();
                }
                else
                {

                    GetDocument(false);
                }
            }

        }

        private void CloseContentView()
        {
            IsActivity = IsActionsVisible = IsSelectDocNetwork = IsSelectDocLocal = IsAddElementVisible = false;
        }

        private async void GetDocumentUsb()
        {
            if (Identification == null || Identification == string.Empty)
            {
                return;
            }

            var menu = UserAccount.SelectedMenu;

            if (UserAccount.SelectedMenu != null)
            {
                StartLoading(Resources.MessageStatusComplitingQuery);

                Page = 1;

                try
                {
                    if (UserAccount.SelectedMenu.IsDoc)
                    {
                        if (!LoadDocMenuLocal(Identification))
                        {
                            await Application.Current.MainPage.DisplayAlert(Resources.Information, Resources.MessageDataBaseNoDoc, Resources.OK);
                        }

                        CancelView();
                    }
                    else
                    {

                        var dictionaryId = WorkDataBase.GetDictionaryIdByBarcode(Identification);

                        if (dictionaryId == 0)
                        {
                            await Application.Current.MainPage.DisplayAlert(Resources.Information, "Значение справочника не найдено!", Resources.OK);

                            StopLoading();
                            return;
                        }

                        WorkDataBase.LoadCurrentDictionary(dictionaryId);

                        CancelView();

                        UpdateProperties();

                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { nameof(UniversalViewModel), nameof(GetDocument) } });
                }
                StopLoading();
            }
        }

        private int LoadDictionaries()
        {
            var menuDictionaries = UserAccount.Menus.Where(x => x.IsDoc == false).ToList();
            var result = 0;

            foreach (var menu in menuDictionaries)
            {
                UserAccount.SelectedMenu = menu;
                var dictionaries = FileService.GetDictionariesFileName(menu.Action);

                foreach (var dictionary in dictionaries)
                {
                    var json = FileService.GetJsonFromFile(dictionary);

                    if (json != null)
                    {
                        if (json[Resources.RestKeyObjectValues] is JObject)
                        {
                            var isLoad = LoadDocumentJson(json[Resources.RestKeyObjectValues] as JObject, menu, true);

                            if (isLoad)
                            {
                                result++;
                            }
                        }
                        else if (json[Resources.RestKeyObjectValues] is JArray)
                        {
                            foreach (JObject jsonObject in json[Resources.RestKeyObjectValues])
                            {
                                var isLoad = LoadDocumentJson(jsonObject, menu, true);

                                if (isLoad)
                                {
                                    result++;
                                }
                            }
                        }
                    }

                    FileService.DeleteLocalFile(dictionary);
                }
            }

            UserAccount.ClearDocMenusLocal();

            return result;
        }

        private int LoadDocuments()
        {
            var menuDocuments = UserAccount.Menus.Where(x => x.IsDoc == true).ToList();
            var idDevice = DependencyService.Get<IInformationDevice>().GetDeviceCode();
            var result = 0;

            foreach (var menu in menuDocuments)
            {
                UserAccount.SelectedMenu = menu;

                var documents = FileService.GetDocuments(menu.Action, UserAccount.Login, idDevice);

                foreach (var document in documents)
                {
                    var json = FileService.GetJsonFromFile(document);

                    if (json != null && json[Resources.RestKeyResultCode].ToString() == Resources.RestKeyResultCodeOne)
                    {
                        if (json[Resources.RestKeyObjectValues] is JObject)
                        {
                            var isLoad = LoadDocumentJson(json[Resources.RestKeyObjectValues] as JObject, menu);

                            if (isLoad)
                            {
                                result++;
                            }
                        }
                        else if (json[Resources.RestKeyObjectValues] is JArray)
                        {
                            foreach (JObject jsonObject in json[Resources.RestKeyObjectValues])
                            {
                                var isLoad = LoadDocumentJson(jsonObject, menu);

                                if (isLoad)
                                {
                                    result++;
                                }
                            }
                        }
                    }

                    FileService.DeleteLocalFile(document);                    
                }
            }

            UserAccount.ClearDocMenusLocal();

            return result;
        }

        private bool LoadDocumentJson(JObject jsonObject, AbstractMenu menu, bool isDictionary = false)
        {
            var jsonFields = jsonObject.ContainsKey(Resources.RestKeyFields) ? JObject.Parse(jsonObject[Resources.RestKeyFields].ToString()) : new JObject();
            var jsonColumns = jsonObject.ContainsKey(Resources.RestKeyColumns) ? JArray.Parse(jsonObject[Resources.RestKeyColumns].ToString()) : new JArray();

            if (LoadDocMenu(menu, jsonObject, jsonFields, jsonColumns))
            {
                if (isDictionary)
                {
                    WorkDataBase.SaveCurrentDictionery();
                }
                else
                {
                    WorkDataBase.SaveCurrentDocument(StatusEnum.InWork);
                }
                

                return true;
            }

            return false;
        }
        #endregion
    }
}
