using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Galaktika.ERP.Interop;
using Galaktika.ESB.Adapter.DbUpdates;
using Galaktika.ESB.Adapter.ERP.Adapter;
using Galaktika.ESB.Adapter.Services;
using Galaktika.ESB.Contract;
using Galaktika.ESB.Core.Contract;
using Galaktika.ESB.Core.Contract.DbUpdates;
using Galaktika.ESB.Core.Exceptions;
using Galaktika.ESB.Core.Storage.Core;
using Galaktika.ESB.ServiceAdapterApi;
using Galaktika.ESB.Storage.Adapter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace Galaktika.ESB.Adapter.Atlantis
{
    /// <summary>
    /// Updates service for adapter ERP. Checks database change and publishes them.
    /// </summary>
    public partial class AtlantisUpdatesService : AtlantisBaseUpdatesService, IEsbConsumersImplementation
    {
        private Lazy<AtlMeta> AtlMeta = new Lazy<AtlMeta>(() =>
        {
            using (var ctx = new DictionaryContext())
            {
                return ctx.GetMeta;
            }
        });

        private IPublisherService _publisherService;

        private TimeSpan _lostNrecsLifeTimeInMins;

        private int _lostNrecsLifeTime;

        private string _userName;

        private List<int> _offices = new List<int>();

        private IParametersService _parametersService;
        /// <summary>
        /// С таким префиксом, в частности, будет храниться последний NREC для офиса в таблице Parameters
        /// </summary>
        private const string OffConst = "Office №";

        /// <summary>
        /// Словарь Код таблицы - NREC записи и запись из журнала изменений
        /// </summary>
        ///private readonly Dictionary<int, Dictionary<long, UpdatesJournal>> _changes = new Dictionary<int, Dictionary<long, UpdatesJournal>>();
        /// <summary>
        /// Словарь. Код таблицы - NREC записи и тип изменения
        /// </summary>
        //private readonly Dictionary<int, Dictionary<string, TypeStorageOperation>> _publishDict = new Dictionary<int, Dictionary<string, TypeStorageOperation>>();

        /// <summary>
        /// Актуальный MinNRec для определенного офиса
        /// </summary>
        private readonly Dictionary<int, string> _minNRecs = new Dictionary<int, string>();

        private Task _updateTask = Task.CompletedTask;

        private bool _isNeedToComplete;

        /// <summary>
        /// Configures updates service
        /// </summary>
        /// <param name="services"></param>
        public override void Configure(IServiceCollection services)
        {
            base.Configure(services);
            services.AddSingleton(this);
        }

        ///// <summary>
        ///// Requiured service types
        ///// </summary>
        //public override IEnumerable<Type> DefaultRequiredServiceTypes => new[] { typeof(AtlantisODataAdapter) };

        #region Settings

        /// <summary>
        /// Types of consumers
        /// </summary>
        public Type[] ConsumerTypes => new[] { typeof(AppStoreListenerSettingsConsumer), typeof(TrackingTablesConsumer) };

        private int LostNrecsLifeTime
        {
            get
            {
                if (_lostNrecsLifeTime != 0) return _lostNrecsLifeTime;
                _lostNrecsLifeTime = 60;
                if (Settings.ContainsKey("LostNrecsLifeTime"))
                    _lostNrecsLifeTime = (int)Settings["LostNrecsLifeTime"];
                return _lostNrecsLifeTime;
            }
        }

        private string UserName
        {
            get
            {
                if (!String.IsNullOrEmpty(_userName)) return _userName;
                _userName = (string)Settings["UserName"];
                if (String.IsNullOrEmpty(_userName))
                {
                    _userName = WindowsIdentity.GetCurrent().Name;
                }
                return _userName;
            }
        }


        private List<int> OfficesForTracking
        {
            get
            {
                var offices = new List<int>();
                var set = (string)Settings["OfficesForTracking"];
                if (!string.IsNullOrEmpty(set))
                {
                    foreach (var p in set.Split(';', ',', '\n', '\r', '\t')
                        .Select((s) => s.Trim())
                        .Where((s) => !string.IsNullOrEmpty(s)))
                    {
                        if (int.TryParse(p, out var of))
                            offices.Add(of);
                        else
                            _Info_Не_удалось_преобразовать_строку_0_в_int(p);
                    }
                }
                return offices;
            }
        }


        #endregion

        #region Реализация получения настроек списка таблиц

        /// <summary>
        /// Gets JArray of tracking tables
        /// </summary>
        /// <param name="typesToListen">List with tracking type</param>
        /// <returns>JArray of tracking tables</returns>
        public override Task<JArray> GetTrackingTables(List<TrackingType> typesToListen)
        {
            //await WaitHandle.WaitAsync();
            var resultJArray = new JArray();
            foreach (var trackingType in typesToListen)
            {
                FILE file;

                try
                {
                    file = AtlMeta.Value.GetFile(trackingType.AppEntityTypeName);
                    if (file == null)
                        throw new InvalidOperationException("file == null for TyepaName=" + trackingType);
                }
                catch (Exception e)
                {
                    LogWarnTableIsNotFound(trackingType, e.Message);
                    continue;
                }

                resultJArray.Add(new JObject
                                {
                                        {"Name", file.XF_CODE.ToString()},
                                        {"PrimaryKey", "NREC"},
                                        {"IsTrackingEnabled", trackingType.IsTrackingEnabled}
                                });
            }

            return Task.FromResult(resultJArray);
        }

        #endregion

        private object _startLocker = new object();
        //AsyncManualResetEvent WaitHandle = new AsyncManualResetEvent(false);
        /// <summary>
        /// Start of updates service. Check if the journalization is on, if it's not - stops the service.
        /// </summary>
        /// <returns></returns>
        public override async Task Start()
        {
            if (_parametersService == null)
            {
                lock (_startLocker)
                {
                    if (_parametersService != null)
                    {
                        LogService.Info("ErpChangeTrackingService already started");
                        return;
                    }
                    LogService.Info("Get parameter service");
                    _parametersService = Adapter.GetService<IParametersService>();
                    _publisherService = Adapter.GetService<IPublisherService>();
                }
            }
            else
            {
                LogService.Info("ErpChangeTrackingService already started");
                return;
            }

            _lostNrecsLifeTimeInMins = new TimeSpan(0, 0, LostNrecsLifeTime, 0);
            LogService.Trace("Check tracking");
            if (!CheckTracking())
                return;
            _isNeedToComplete = false;
            _trackedTableCodes = new Lazy<List<int>>(() => DbTables.Select(x => int.Parse(x.TableName)).ToList());

            //"ERP Updates check is started OK".ErpInfo();
            await base.Start();
        }

        private void RunCheckUpdates(TimeSpan interval)
        {
            //Нам тут нужно просто запустить и забыть
            _updateTask = Task.Run(async () =>
            {
                while (!_isNeedToComplete)
                {
                    await Task.Delay(interval).ConfigureAwait(false);
                    if (!_isNeedToComplete)
                    {
                        LogService.Trace("* Started new processing circle");
                        await CheckDbUpdatesProcessing().ConfigureAwait(false);
                        LogService.Trace("* Finished processing circle");
                    }
                }
                LogService.Trace("AtlantisUpdatesService was stopped");
            });
        }

        /// <summary>
        /// Checks if tracking is enabled
        /// </summary>
        /// <returns>true if tracking is enabled, false - it's not</returns>
        public override bool CheckTracking()
        {
            if (!IsModelActivated()) return false;
            using (var cntx = new DictionaryContext())
            {
                foreach (var oft in OfficesForTracking)
                {
                    LogService.Trace($"Office for tracking number = {oft}");
                    var abon =
                        cntx.ABONENTS.FirstOrDefault(a =>
                            a.OFFICENO == oft);
                    if (abon != null && abon.JOURNALMODE == 1)
                    {
                        LogService.Trace($"Add office number {oft}");
                        LogService.Trace($"Abonent name = {abon.NAME}");
                        _offices.Add(oft);
                    }
                    else
                        _Warn_Отсутствует_абонент_с_номером_0_либо_журнализация_для_него_выключена(oft);
                }

                if (_offices.Any())
                    return true;
                LogService.Trace("No offices for tracking.");
                _offices = new List<int> { 0 };

                var ownUser = cntx.X_USERS.SingleOrDefault(u => u.XU_LOGINNAME == UserName);
                if (ownUser == null)
                {
                    LogWarnUserNotFound(UserName);
                    return false;
                }
                _Info_UserOffice_0(ownUser.XU_USEROFFICE);
                var journalConf = cntx.X_JOURNALCONFIG.SingleOrDefault(jc => jc.OFFICENO == ownUser.XU_USEROFFICE);
                if (journalConf == null)
                {
                    LogWarnConfigNotFound(ownUser.XU_LOGINNAME, ownUser.XU_USEROFFICE);
                    return false;
                }

                if (journalConf.TYPEJOURNAL == 0)
                {
                    LogWarnLoggingOff(UserName, ownUser.ATL_ORIGINOFFICE);
                    return false;
                }
                var trackedTables = DbTables.Where(table => table.IsTrackingEnabled)
                        .Select(table => table.TableName).ToArray();

                var abonent =
                        cntx.ABONENTS.SingleOrDefault(
                                a => a.OFFICENO == journalConf.OFFICENO);
                if (abonent == null)
                {
                    LogWarnNoUserForConfigFound(journalConf.OFFICENO, ownUser.XU_LOGINNAME);
                    return false;
                }

                var abonNrec = abonent.ATL_NREC;
                _Info_AbonNrec_0(abonNrec);
                var journalTables = cntx.TABLESFORJOURNAL.Where(i => i.COFFICENO == abonNrec).Select(i => i.TABLECODE.ToString()).ToArray();
                if (trackedTables.Any())
                {
                    var missed = trackedTables.Except(journalTables).ToArray();
                    if (missed.Any())
                    {
                        foreach (var tableNum in missed)
                            LogWarnLoggingOffForTable(tableNum);
                    }
                    if (missed.Length == trackedTables.Length)
                    {
                        LogWarnLoggingOffForAllTables();
                        return false;
                    }
                }
                foreach (var ab in cntx.ABONENTS.ToArray())
                    _offices.Add(ab.OFFICENO);
                return true;
            }
        }

        /// <summary>
        /// Признак того, активирована ли модель
        /// </summary>
        /// <returns>true - если активирована, иначе false</returns>
        private bool IsModelActivated()
        {
            using (var context = new AdapterDbContext(LogService))
                return context.DbTrackingTables.Any(tt => tt.IsTrackingEnabled);
        }


        /// <summary>
        /// Читает значения minNrec для офисов
        /// </summary>
        public virtual void ReadState()
        {
            foreach (var office in _offices)
            {
                try
                {
                    var lastNrec = _parametersService.GetValue(OffConst + office, true);
                    if (string.IsNullOrEmpty(lastNrec))
                    {
                        lastNrec = LoadLastNrec(office);
                        _parametersService.SetValue(OffConst + office, lastNrec, true);
                    }
                    _minNRecs[office] = lastNrec;
                }
                catch (Exception e)
                {
                    LogService.Error(e);
                    throw;
                }
            }
        }

        /// <summary>
        /// Записывает состояние отслеживания. 
        /// </summary>
        public virtual void WriteState()
        {
            foreach (var office in _offices)
            {
                if (string.IsNullOrEmpty(_minNRecs[office]))
                    throw new Exception("Попытка записи пустой строки как последний проверенный NREC  в журнале!");
                _parametersService.SetValue(OffConst + office, _minNRecs[office], true);
            }
        }

        /// <summary>
        /// Останавливает update сервис
        /// </summary>
        /// <returns></returns>
        public override Task Stop()
        {
            _isNeedToComplete = true;
            _updateTask?.Wait();
            _updateTask = null;

            _publisherService?.Stop();
            return base.Stop();
        }

        /// <summary>
        /// Получение последнего, самого свежего nrec для офиса
        /// </summary>
        /// <param name="office">Номер офиса</param>
        /// <returns>nrec</returns>
        private string LoadLastNrec(int office)
        {
            using (var ctx = new DictionaryContext())
            {
                var minNrec = MinNrec(office);
                var maxNrec = MaxNrec(office);
                try
                {
                    var journal = ctx.JOURNAL.Where(j => j.NREC >= minNrec && j.NREC <= maxNrec).Max(j => j.NREC);
                    if (journal == 0)
                        return minNrec.ToString();
                    return journal.ToString();
                }
                catch (ArgumentNullException)
                {
                    return minNrec.ToString();
                }
            }
        }

        /// <summary>
        /// Минимально допустимый nrec для офиса
        /// </summary>
        /// <param name="office">Номер офиса</param>
        /// <returns>nrec</returns>
        long MinNrec(int office)
        {
            return ((long)office) << 48;
        }

        /// <summary>
        /// Максимально допусмтимый nrec для офиса
        /// </summary>
        /// <param name="office">Номер офиса</param>
        /// <returns>nrec</returns>
        long MaxNrec(int office)
        {
            return ((office + 1L) << 48) - 1L;
        }

        /// <summary>
        /// Определение того, отслеживается ли таблица
        /// </summary>
        /// <param name="tableCode">Код таблицы</param>
        /// <returns></returns>
        private bool IsTrackingTable(int tableCode)
        {
            //return DbTables.Select(p => p.TableName).ToList().Contains(tableCode.ToString());

            if (tableCode < 0)
                throw new InvalidOperationException("tableCode < 0");

            if (TrackedTables == null)
            {
                TrackedTables = new BitArray(800);

                foreach (var tbl in DbTables.ToList())
                {
                    var tblcode = int.Parse(tbl.TableName);
                    if (tblcode >= TrackedTables.Length)
                        //                        throw new Exception("tblcode >= TrackedTables.Length");
                        TrackedTables.Length = tblcode * 2;

                    TrackedTables[tblcode] = true;
                }
            }
            if (tableCode >= TrackedTables.Length)
                return false;


            return TrackedTables[tableCode];
        }

        /// <summary>
        /// Коды отслеживаемых таблиц
        /// </summary>
        private Lazy<List<int>> _trackedTableCodes;

        /// <summary>
        /// Пустой список для возврата, чтобы не создавать каждый раз
        /// </summary>
        private static readonly List<LostJournalNrec> _emptyLostJournalNrecList = new List<LostJournalNrec>();

        /// <summary>
        /// Публикация события об изменения на шине
        /// </summary>
        /// <param name="office">Номер офиса</param>
        /// <param name="message">Сообщение</param>
        /// <param name="journalNotes">Список записей в журнале на тот случай, чтобы если не удастся отправить сообщение - сохранить записи как потерянные nrec</param>
        /// <returns>Список потерянных nrec на случай если не удастся отправить сообщение</returns>
        private async Task<List<LostJournalNrec>> PublishChanges(int office, EntityForTracking message, List<UpdatesJournal> journalNotes)
        {
            try
            {
                LogService.Trace($"Publish changes for table[{message.TableCode}] and [{message.Changes.Select(x => $"[{x.Key}:{x.Value}]").Aggregate((f, s) => f + s)}]");
                await _publisherService.PublishMainEntities(AtlMeta.Value.GetTypeByTableCode(message.TableCode), message.Changes, null, PubContext.DbTracking).ConfigureAwait(false);

                return _emptyLostJournalNrecList;
            }
            catch (Exception ex)
            {
                LogService.Error($"Publishing changes failed with error [{ex.Message}]");

                return journalNotes.Where(x => x.TABLECODE == message.TableCode && message.Changes.ContainsKey(x.TABLENREC.ToString()))
                                    .Select(x => new LostJournalNrec
                                    {
                                        StartRangeNrec = x.NREC.ToString(),
                                        EndRangeNrec = x.NREC.ToString(),
                                        Time = DateTime.Now,
                                        OfficeNumber = office
                                    }).ToList();
            }
        }

        /// <summary>
        /// Обработка изменений в конкретном офисе
        /// </summary>
        /// <param name="office">Идентификатор офиса</param>
        /// <returns>Задача по обработке</returns>
        private async Task CheckDbUpdates4Office(int office)
        {
            var minNrec = long.Parse(_minNRecs[office]);

            long currentLastNrec = GetCurrentLastNrecForOffice(minNrec, MaxNrec(office));
            LogService.Trace($"--> Start processing office[{office}]. NRecs range[{minNrec}:{currentLastNrec}]");

            using (var ctx = new AdapterDbContext(LogService))
            {
                var lostedNrecs = await GetLostJournalNrecsToProceed(ctx, office, minNrec, currentLastNrec).ConfigureAwait(false);
                if (lostedNrecs.Count == 0)
                {
                    LogService.Trace($"For office[{office}] nothing to proceed");
                    return;
                }

                List<Tuple<long, long>> rangesForQuerys = GetJournalQueryRanges(lostedNrecs, MaxChangesCountInChunk);
                var ranges = rangesForQuerys.Select(x => $"({x.Item1}:{x.Item2})").Aggregate((f, s) => f + s);
                LogService.Trace($"Defined next ranges to select[{ranges}]");

                List<UpdatesJournal> journalItems = null;       // Текущий фрейм элементов на обработку
                int curRangeForQueryIndex = 0;
                long curMaxSelectedNrec = -1;
                foreach (var nrecsRange in lostedNrecs)
                {
                    if (_isNeedToComplete)
                        break;
                    long startRangeNrec = long.Parse(nrecsRange.StartRangeNrec);
                    long endRangeNrec = long.Parse(nrecsRange.EndRangeNrec);

                    if (startRangeNrec > curMaxSelectedNrec)
                    {   // Нужно обновить обрабатываемый фрейм с записями из журнала
                        journalItems = await LoadJournalItems(rangesForQuerys[curRangeForQueryIndex].Item1, rangesForQuerys[curRangeForQueryIndex].Item2).ConfigureAwait(false);
                        curMaxSelectedNrec = rangesForQuerys[curRangeForQueryIndex++].Item2;
                    }

                    List<LostJournalNrec> lostNrecRanges = new List<LostJournalNrec>();     // Найденные в результате обработки, потерянные NREC
                    long nextTimeMinNrec = startRangeNrec;                                  // Минимальный NREC для следующего цикла обработки

                    //Оставляем только записи нужного нам диапазона
                    var itemsToProceed = journalItems.Where(j => j.NREC >= startRangeNrec && j.NREC <= endRangeNrec).ToList();
                    if (itemsToProceed.Any())
                    {
                        LogService.Trace($"Start to proceed [{itemsToProceed.Count}] changes in range [{startRangeNrec}:{endRangeNrec}({endRangeNrec - startRangeNrec + 1})]");
                        nextTimeMinNrec = itemsToProceed.Last().NREC + 1;

                        lostNrecRanges = await ProceedDbUpdates4Office(office,
                                                                        startRangeNrec,
                                                                        endRangeNrec,
                                                                        itemsToProceed,
                                                                        nrecsRange.Time).ConfigureAwait(false);
                    }
                    else
                    {
                        LogService.Trace($"No changes in losted range[{ startRangeNrec}:{ endRangeNrec} ({ endRangeNrec - startRangeNrec + 1})]");
                    }

                    _minNRecs[office] = Math.Max(nextTimeMinNrec, minNrec).ToString();

                    if ((nextTimeMinNrec > startRangeNrec && nrecsRange.Id != Guid.Empty) || GetDuration(nrecsRange.Time) > _lostNrecsLifeTimeInMins)
                    {   // Если мы обработали из секции хотя бы один элемент и эта секция не фиктивная или она поэкспайрилась, то нужно удалять секцию
                        var history = new LostJournalNrecHistory
                        {
                            StartRangeNrec = nrecsRange.StartRangeNrec,
                            EndRangeNrec = nrecsRange.EndRangeNrec,
                            OfficeNumber = nrecsRange.OfficeNumber,
                            Time = nrecsRange.Time
                        };

                        await ctx.LostJournalNrecHistory.AddAsync(history).ConfigureAwait(false);

                        LogService.Trace($"Remove losted nrec[{nrecsRange.StartRangeNrec}:{nrecsRange.EndRangeNrec}] because has processed items or expired");
                        ctx.LostJournalNrec.Remove(nrecsRange);
                    }

                    lostNrecRanges.ForEach(x => LogService.Trace($"Created new losted Nrec range[{x.StartRangeNrec}:{x.EndRangeNrec}] based on previouse range[{nrecsRange.StartRangeNrec}:{nrecsRange.EndRangeNrec}]"));
                    await ctx.LostJournalNrec.AddRangeAsync(lostNrecRanges).ConfigureAwait(false);
                }

                LogService.Trace($"Save changes in losted nrecs office[{office}]. NRecs range[{minNrec}:{currentLastNrec}]");
                await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            LogService.Trace($"--> Finish processing office[{office}]. NRecs range[{minNrec}:{currentLastNrec}]");
        }

        /// <summary>
        /// Обработка(публикация) изменений в заданном диапазоне и получение списка потерянных nrec
        /// </summary>
        /// <param name="office">Номер офиса</param>
        /// <param name="minNrec">Минимальный nrec для данного офиса</param>
        /// <param name="maxNrec">Максимальный nrec для данного офиса</param>
        /// <param name="items">Элементы журнала для обработки</param>
        /// <param name="dt">Дата и время для фиксации</param>
        /// <returns></returns>
        private async Task<List<LostJournalNrec>> ProceedDbUpdates4Office(int office, long minNrec, long maxNrec, List<UpdatesJournal> items, DateTime dt)
        {
            // Получаем набор элементов, которые есть в диапазоне
            var workedItems = items.Select(j => j.NREC).ToList();
            // Добавляем края диапазона. Повторы(если крайние точки уже есть) не мешают
            workedItems.Insert(0, Math.Max(minNrec - 1, 0));
            workedItems.Add(Math.Min(maxNrec + 1, MaxNrec(office)));
            var lostNrecRanges = SearchForMissedNrecs(workedItems, dt, office);

            lostNrecRanges.ForEach(x => LogService.Trace($"Found new losted nrec[{x.StartRangeNrec}:{x.EndRangeNrec}({long.Parse(x.EndRangeNrec) - long.Parse(x.StartRangeNrec) + 1})] in range [{minNrec}:{maxNrec}]"));

            // Оставляем только записи для таблиц, которые мы отслеживаем
            // Они нужны были нам ранее, чтобы запомнить дыры в диапазоне
            var journalItemsToProceed = items.Where(x => _trackedTableCodes.Value.Contains(x.TABLECODE)).ToList();
            var needToSendItems = journalItemsToProceed.GroupJoin(journalItemsToProceed,
                                                                j1 => new { j1.TABLECODE, j1.TABLENREC },
                                                                j2 => new { j2.TABLECODE, j2.TABLENREC },
                                                                (i, g) => g.Where(k => k.NREC == g.Max(u => u.NREC)).First())
                                                                .Distinct()
                                                                .OrderBy(x => x.NREC)
                                                                .ToList();

            LogService.Trace($"Founded [{needToSendItems.Count}] changes for send to queue");

            // Фиктивный элемент в конец, чтобы не писать лишний if но при этом отправить последнюю порцию данных
            // OPERATION = 2 потому что без разницы
            needToSendItems.Add(new UpdatesJournal() { TABLECODE = -1, TABLENREC = 0, OPERATION = 2 });

            var changesChunk = new EntityForTracking() { Changes = new Dictionary<string, TypeStorageOperation>() };
            changesChunk.TableCode = needToSendItems[0].TABLECODE;
            foreach (var journalNote in needToSendItems)
            {
                if (journalNote.TABLECODE != changesChunk.TableCode)
                {
                    lostNrecRanges.AddRange(await PublishChanges(office, changesChunk, journalItemsToProceed).ConfigureAwait(false));
                    changesChunk.Changes.Clear();
                    changesChunk.TableCode = journalNote.TABLECODE;
                }
                changesChunk.Changes[journalNote.TABLENREC.ToString()] = GetOperationType(journalNote);
            }

            return lostNrecRanges;
        }

        /// <summary>
        /// Загрузка и формирование дырок(необработанных диапазонов) журнала
        /// Грузятся потерянные (не загруженные вовремя) области, а так же новая обрасть для обработки
        /// </summary>
        /// <param name="context">Контекст для чтения потерянных дырок из БД</param>
        /// <param name="office">ID офиса для которого грузятся дырки</param>
        /// <param name="minNrec">Минимальный NRec для которого нужно производить загрузку</param>
        /// <param name="currentLastNrec">Текущий максимальный NRec</param>
        /// <returns>Созданная задача для получения потерянный Nrec</returns>
        private async Task<List<LostJournalNrec>> GetLostJournalNrecsToProceed(AdapterDbContext context, int office, long minNrec, long currentLastNrec)
        {
            // Грузим сохраненные дырки
            var lostedNrecs = await context.LostJournalNrec.Where(l => l.OfficeNumber == office).OrderBy(l => l.EndRangeNrec).ToListAsync().ConfigureAwait(false);
            // Сообщение для отладки
            string lr = lostedNrecs.Select(x => $"({x.StartRangeNrec}:{x.EndRangeNrec})").DefaultIfEmpty().Aggregate((f, s) => f + s) ?? string.Empty;
            LogService.Trace($"Was found [{lostedNrecs.Count}] old losted nrecs[{lr}]");
            // К ним добавляем новые фиктивные отрезки из рабочего диапазона
            var dummyLostNrecs = GetDummyLostNrecs(office, minNrec, currentLastNrec, MaxChangesCountInChunk);

            lr = dummyLostNrecs.Select(x => $"({x.StartRangeNrec}:{x.EndRangeNrec})").DefaultIfEmpty().Aggregate((f, s) => f + s) ?? string.Empty;
            LogService.Trace($"Was found [{dummyLostNrecs.Count}] new nrec ranges [{lr}] to processing");

            lostedNrecs.AddRange(dummyLostNrecs);
            return lostedNrecs;
        }

        /// <summary>
        /// Считывание записей журнала в заданном окне [minNrec, maxNrec]
        /// </summary>
        /// <param name="minNrec">Минимальный NRec</param>
        /// <param name="maxNrec">Максимальный NRec</param>
        /// <returns>Задача, получающая записи</returns>
        private Task<List<UpdatesJournal>> LoadJournalItems(long minNrec, long maxNrec)
        {
            LogService.Trace($"Load items in range [{minNrec}:{maxNrec}]");

            using (var cntx = new DictionaryContext())
            {
                // Считывание записей журнала в заданном окне [minNrec, maxNrec]
                return cntx.JOURNAL.Where(j => j.NREC >= minNrec && j.NREC <= maxNrec)
                                    .OrderBy(j => j.NREC)
                                    .Select(x => new UpdatesJournal
                                    {
                                        //LASTTIME = x.LASTTIME,
                                        TABLENREC = x.TABLENREC,
                                        //LASTDATE = x.LASTDATE,
                                        TABLECODE = x.TABLECODE,
                                        NREC = x.NREC,
                                        OPERATION = x.OPERATION
                                    })
                                    .ToListAsync();
            }
        }

        /// <summary>
        /// Разбиение диапазона загрузки на отрезки
        /// Требуется для того, чтобы не грузить пол базы одним запросом
        /// </summary>
        /// <param name="office">Номер офиса</param>
        /// <param name="from">Начало диапазона</param>
        /// <param name="to">Конец диапазона</param>
        /// <param name="maxSize">Размер пакета</param>
        /// <returns>Список областей для загрузки</returns>
        private static List<LostJournalNrec> GetDummyLostNrecs(int office, long from, long to, int maxSize)
        {
            DateTime now = DateTime.Now;
            List<LostJournalNrec> ret = new List<LostJournalNrec>();
            //Формируем фиктивные дырки для поиска в новых записях
            for (long n = from; n <= to; n += maxSize)
            {
                ret.Add(new LostJournalNrec
                {
                    Id = Guid.Empty,
                    StartRangeNrec = n.ToString(),
                    EndRangeNrec = Math.Min(n + maxSize - 1, to).ToString(),
                    Time = now,
                    OfficeNumber = office
                });
            }

            return ret;
        }

        /// <summary>
        /// Из набора областей, которые нужно обработать, формирует набор областей для загрузки из базы
        /// Не все загруженные элементы будут использоваться. Нужно для оптимизации производительности, потому что
        /// Грузить по одной записи из БД очень дорого
        /// </summary>
        /// <param name="ranges">Набор областей, которые нужно покрыть</param>
        /// <param name="oneQueryChunkMaxSize">Максимальный размер области для запроса</param>
        /// <returns>Размер областей для загрузки</returns>
        private static List<Tuple<long, long>> GetJournalQueryRanges(List<LostJournalNrec> ranges, long oneQueryChunkMaxSize)
        {
            List<Tuple<long, long>> ret = new List<Tuple<long, long>>();
            long curChunnkSize = 0;
            long curStart = long.Parse(ranges[0].StartRangeNrec);
            for (int n = 1; n < ranges.Count(); n++)
            {
                curChunnkSize = long.Parse(ranges[n].EndRangeNrec) - curStart + 1;
                if (curChunnkSize > oneQueryChunkMaxSize)
                {   // У нас размер одного чанка всё равно не должен быть больше oneQueryChunkMaxSize
                    ret.Add(new Tuple<long, long>(curStart, long.Parse(ranges[n - 1].EndRangeNrec)));

                    curChunnkSize = 0;
                    curStart = long.Parse(ranges[n].StartRangeNrec);
                }
            }
            long lastEndNrec = long.Parse(ranges[ranges.Count() - 1].EndRangeNrec);
            // Добавляем последний ошметок
            var lastChunks = Enumerable.Range(0, int.MaxValue)
                                                .Select(i => curStart + (i * oneQueryChunkMaxSize))
                                                .TakeWhile(i => i <= lastEndNrec)
                                                .ToList();
            lastChunks.ForEach(x => ret.Add(new Tuple<long, long>(x, Math.Min(lastEndNrec, x + oneQueryChunkMaxSize - 1))));
            return ret;
        }

        /// <summary>
        /// Возвражает идентификатор последней(самой свежей) записи в журнале для определенного офиса (определяется диапазоном)
        /// </summary>
        /// <param name="minNrec">Начальный nrec для офиса</param>
        /// <param name="maxNrecForOffice">Максимальный nrec для офиса</param>
        /// <returns>Текущий самый последний nrec для офиса</returns>
        private static long GetCurrentLastNrecForOffice(long minNrec, long maxNrecForOffice)
        {
            using (var ctx = new DictionaryContext())
            {
                // Определяем на текущий момент максимальный nrec
                return ctx.JOURNAL
                        .Where(j => j.NREC >= minNrec && j.NREC <= maxNrecForOffice)
                        .DefaultIfEmpty()
                        .Max(j => j.NREC);
            }
        }

        /// <summary>
        /// Возвращает длительность, от начала и до текущего момента
        /// </summary>
        /// <param name="startTime">Стартовая точка во времени</param>
        /// <returns>Длительность</returns>
        private static TimeSpan GetDuration(DateTime startTime)
        {
            return DateTime.Parse(DateTime.Now.ToString()).Subtract(DateTime.Parse(startTime.ToString()));
        }

        /// <summary>
        /// Заполняем список диапазонов потерянных nrec(дырок) на основании набора чисел
        /// Для корректной работы в диапазоны перед вызовом должны быть внесены крайние точки (начало диапазона и конец)
        /// </summary>
        /// <param name="numbers">Отсортированный набор чисел для анализа</param>
        /// <param name="dt">Дата и время для фиксации создания</param>
        /// <param name="officeNum">Номер офиса</param>
        /// <returns>Список потерянных диапазонов</returns>
        private static List<LostJournalNrec> SearchForMissedNrecs(List<long> numbers, DateTime dt, int officeNum)
        {
            var lostJournalNrecs = new List<LostJournalNrec>(numbers.Count() + 1);

            long startPoint = numbers.First();
            foreach (var num in numbers)
            {
                if (startPoint < num)
                {
                    lostJournalNrecs.Add(new LostJournalNrec
                    {
                        StartRangeNrec = startPoint.ToString(),
                        EndRangeNrec = (num - 1).ToString(),
                        Time = dt,
                        OfficeNumber = officeNum
                    });
                }

                startPoint = num + 1;
            }

            return lostJournalNrecs;
        }

        //private bool _isPublishStarted = false;
        /// <summary>
        /// Method checking updates 
        /// </summary>
        /// <returns></returns>
        public override async Task CheckDbUpdates()
        {
            if (!IsModelActivated())
            {
                LogService.Warn($"Model is not activated! Check database disabled!");
                return;
            }

            ReadState();

            foreach (var office in _offices.TakeWhile(office => !_isNeedToComplete))
            {
                try
                {
                    await CheckDbUpdates4Office(office).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    LogService.Error(e);
                }
            }

            try
            {
                WriteState();
            }
            catch (Exception e)
            {
                /// Предусмотреть аварийное сохранение счетчиков в файл
                LogService.Error(e, "CheckDbUpdates");
                throw;
            }
        }

        /// <summary>
        /// Starts the timer for checking updates
        /// </summary>
        /// <returns></returns>
        public override void StartTimer()
        {
            LogService.Trace("Set interval");
            var interval = new TimeSpan(0, 0, UpdatesCheckingInterval);
            LogService.Trace($"Interval = {interval}");

            LogService.Info("Timer Start");
            RunCheckUpdates(interval);
        }

        /// <summary>
        /// Получение типа операции на основании Записи в журнале
        /// </summary>
        /// <param name="journal">Запись в журнале</param>
        /// <returns>Тип выполненной операции</returns>
        private static TypeStorageOperation GetOperationType(UpdatesJournal journal)
        {
            switch (journal.OPERATION)
            {
                case 2:
                    return TypeStorageOperation.Create;
                case 4:
                    return TypeStorageOperation.Update;
                case 8:
                    return TypeStorageOperation.Delete;
            }

            throw new EsbException("Operation type is not determined on ErpUpdatesService");
        }
    }
}
