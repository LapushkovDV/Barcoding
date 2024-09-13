using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Galaktika.ESB.Contract;
using Galaktika.ESB.Core;
using Galaktika.ESB.ServiceAdapterApi;
using Galaktika.ESB.Storage.Adapter;
using Newtonsoft.Json.Linq;

namespace Galaktika.ESB.Adapter.Atlantis
{
    /// <summary>
    /// Базовый сервис обновлений Атлантиса
    /// </summary>
    public abstract class AtlantisBaseUpdatesService : ServiceBase, IDbUpdatesParserService, IUpdatesService
    {
        private int _delayAfterSqlException;

        /// <summary>
        /// Отслеживаемые таблицы
        /// </summary>
        protected BitArray TrackedTables;

        /// <summary>
        /// Получение отслеживаемых таблицы
        /// </summary>
        /// <param name="typesToListen">Типы прослушивания</param>
        /// <returns></returns>
        public abstract Task<JArray> GetTrackingTables(List<TrackingType> typesToListen);

        /// <summary>
        /// Проверка трэкинга
        /// </summary>
        /// <returns></returns>
        public abstract bool CheckTracking();

        /// <summary>
        /// Проверка изменений в бд
        /// </summary>
        /// <returns></returns>
        public abstract Task CheckDbUpdates();

        /// <summary>
        /// Старт таймера
        /// </summary>
        public abstract void StartTimer();

        /// <summary>
        /// Обновление остлеживаемых таблиц
        /// </summary>
        /// <param name="dbTablesJArray">Таблицы</param>
        public virtual void UpdateTrackingTables(JArray dbTablesJArray)
        {
            try
            {
                using (var context = new AdapterDbContext(LogService))
                {
                    var list = dbTablesJArray.Select(_ => new DbTrackingTable
                    {
                        TableName = _.Get<string>("Name"),
                        PrimaryKeyField = _.Get<string>("PrimaryKey"),
                        IsTrackingEnabled = _.Get<bool>("IsTrackingEnabled"),
                        ModelId = _.Get<string>("ModelId")
                    });

                    context.DbTrackingTables.RemoveRange(context.DbTrackingTables);
                    context.DbTrackingTables.AddRange(list);
                    context.SaveChanges();
                    _dbTables = context.DbTrackingTables.ToList();
                    TrackedTables = null;
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex);
                throw;
            }
        }

        private int _maxRecordChangesCountInChunk;
        /// <summary>
        /// Максимальное изменение за раз
        /// </summary>
        public virtual int MaxChangesCountInChunk
        {
            get
            {
                if (_maxRecordChangesCountInChunk < 1)
                {
                    var sMaxRecordChangesCountInChunk = Settings["MaxRecordChangesCountInChunk"];
                    if (sMaxRecordChangesCountInChunk == null)
                        _maxRecordChangesCountInChunk = 10000;
                    else
                        _maxRecordChangesCountInChunk = (int)sMaxRecordChangesCountInChunk;
                }
                return _maxRecordChangesCountInChunk;
            }
        }

        private int _interval;
        /// <summary>
        /// Интервал обновления
        /// </summary>
        public virtual int UpdatesCheckingInterval
        {
            get
            {
                if (_interval == 0)
                {
                    var sErpUpdatesServiceIntervalSec = Settings["ErpUpdatesServiceIntervalSec"];
                    if (null == sErpUpdatesServiceIntervalSec)
                        _interval = 10;
                    else
                        _interval = (int)sErpUpdatesServiceIntervalSec;
                }

                return _interval;
            }
        }

        private IEnumerable<DbTrackingTable> _dbTables;
        /// <summary>
        /// Таблицы бд
        /// </summary>
        public virtual IEnumerable<DbTrackingTable> DbTables
        {
            get
            {
                if (_dbTables == null)
                {
                    using (var context = new AdapterDbContext(LogService))
                        _dbTables = context.DbTrackingTables.ToList();
                }

                return _dbTables;
            }
        }

        /// <summary>
        /// Процесс проверки обновлений в бд
        /// </summary>
        /// <returns></returns>
        protected async Task CheckDbUpdatesProcessing()
        {
            try
            {
                await CheckDbUpdates().ConfigureAwait(false);
                _delayAfterSqlException = 0;
            }
            catch (SqlException ex)
            {
                LogService.Error(ex);
                await Task.Delay(_delayAfterSqlException += 100);
            }
            catch (Exception ex)
            {
                LogService.Error(ex);
            }
        }
    }
}
