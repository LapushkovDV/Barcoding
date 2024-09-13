using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Galaktika.ERP.Interop;
using Galaktika.ESB.Adapter.Atlantis.Api;
using Galaktika.ESB.ServiceAdapterApi;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Galaktika.ESB.Adapter.Atlantis
{
    /// <summary>
    /// Main service for Atlantis Adapter
    /// </summary>
    public partial class AtlantisAdapterService : ServiceBase, IAdapterService
    {
        private int _killAdapterTimeout;
        /// <summary>
        /// Timeout in minutes when to start checking adapter.
        /// </summary>
        public int KillAdapterTimeOut
        {
            get
            {
                if (_killAdapterTimeout == 0)
                {
                    _killAdapterTimeout = 1;
                    var param = Settings["KillAdapterTimeout"];
                    if (param != null)
                    {
                        int parse;
                        if (int.TryParse(param.ToString(), out parse))
                            _killAdapterTimeout = parse;
                    }
                }
                return _killAdapterTimeout;
            }
        }



        private int _requestTimeout;
        /// <summary>
        /// Timeout in seconds for delay on request for checking adapter
        /// </summary>
        public int RequestTimeOut
        {
            get
            {
                if (_requestTimeout == 0)
                {
                    _requestTimeout = 300*1000;
                    var param = Settings["RequestTimeOut"];
                    if (param != null)
                    {
                        int parse;
                        if (int.TryParse(param.ToString(), out parse))
                            _requestTimeout = parse*1000;
                    }
                }
                return _requestTimeout;
            }
        }

        ///// <summary>
        ///// List of required services. Requried services based on setting value "State"
        ///// </summary>
        //public override IEnumerable<Type> DefaultRequiredServiceTypes
        //{
        //    get
        //    {
        //        var state = (string)this.Settings["State"];
        //        if (state == null)
        //            state = string.Empty;

        //        switch (state.ToLower())
        //        {
        //            case "disableadapter":
        //                yield return typeof(AtlantisODataService);
        //                break;
        //            case "disableupdatescheck":
        //                yield return typeof(AtlantisODataAdapter);
        //                break;
        //            default:
        //                yield return typeof(AtlantisUpdatesService);
        //                break;
        //        }
        //    }
        //}

        /// <summary>
        /// Requiured service types
        /// </summary>
        public override IEnumerable<Type> DefaultRequiredServiceTypes => new[] { typeof(AtlantisODataAdapter) };

        /// <summary>
        /// Method check's is adapter still working or not.
        /// </summary>
        /// <returns>True - still working, false - not</returns>
        public bool IsAdapterAlive()
        {
            var timeout = new TimeSpan(0, 0, KillAdapterTimeOut, 0);
            if (_longOperationTimeout != TimeSpan.Zero)
                timeout = _longOperationTimeout;

            if (LastAdapterActivity != DateTime.MinValue &&
                DateTime.Now - LastAdapterActivity > timeout)
            {
                _Trace_LastAdapterActivity_0_LastDispatcherTread_1();

                var ct = new CancellationTokenSource();
                var task = Task.Run(new Func<FILE>(GetFile), ct.Token);

                var isFinished = task.Wait(RequestTimeOut);
                if (!isFinished)
                {
                    ct.Cancel();
                    _Info_Выполнение_запроса_длилось_более_0_секунд_Адаптер_упадет_по_таймауту();
                    return false;
                }
            }
            return true;
        }

        private TimeSpan _longOperationTimeout = TimeSpan.Zero;

        /// <summary>
        /// Установка таймаута проверки адаптера на работоспособность. Используется
        /// при выполнении длительных операций.
        /// </summary>
        /// <param name="operationTimeout">Значение таймаута</param>
        public void SetLongOperationTimeout(TimeSpan operationTimeout)
        {
            _longOperationTimeout = operationTimeout;
        }

        /// <summary>
        /// Сброс таймаута на занчение по умолчанию.
        /// </summary>
        public void ResetLongOperationTimeout()
        {
            _longOperationTimeout = TimeSpan.Zero;
        }

        private FILE GetFile()
        {
            using (var ctx = new DictionaryApi.DictionaryContext())
            {
                _Info_Запрос_на_то_живой_ли_адаптер();
                try
                {
                    var file = ctx.FILE.AsNoTracking().FirstOrDefault();
                    _Info_Возвращение_результата_запроса();
                    return file;
                }
                catch (Exception ex)
                {
                    throw new AtlantisDbException(GalaktikaErrorCode.CheckLifeAdapterError, "var file = ctx.FILE.AsNoTracking().FirstOrDefault().Exception inside checking IsAdapterAlive." + ex.Message);
                }
            }
        }

        private DateTime LastAdapterActivity => DispatcherHelper.LastDispatcherActivity > DsqlHelper.LastContextActivity 
            ? DispatcherHelper.LastDispatcherActivity : DsqlHelper.LastContextActivity;

        /// <summary>
        /// Id адаптера. Должен быть уникальным среди всех запущенных адаптеров.
        /// Используется в частности для отображения и фильтрации метрик по конкретному адаптеру.
        /// </summary>
        public string AdapterId => Settings.Get<string>("AdapterId");

        /// <summary>
        /// Method gets all info about tables and puts it in json file.
        /// This file is needed to generate entity framework context.
        /// </summary>
        public void RunBuildMode()
        {
            using (var ctx = new DictionaryApi.DictionaryContext())
            {
                var filesWithouRelations = ctx.FILE.Select(fil => new
                {
                    XF_CODE = fil.XF_CODE,
                    XF_ATTR = fil.XF_ATTR,
                    XF_CHECKSUM = fil.XF_CHECKSUM,
                    XF_COMPONENT = fil.XF_COMPONENT,
                    XF_FLAGS = fil.XF_FLAGS,
                    XF_FLAGS2 = fil.XF_FLAGS2,
                    XF_FORMAT = fil.XF_FORMAT,
                    XF_LOC = fil.XF_LOC,
                    XF_LOC2 = fil.XF_LOC2,
                    XF_NAME = fil.XF_NAME,
                    XF_OWNERNAME = fil.XF_OWNERNAME,
                    XF_PAGESIZE = fil.XF_PAGESIZE,
                    XF_PRIMARYKEYNO = fil.XF_PRIMARYKEYNO,
                    XF_RECORDFIXED = fil.XF_RECORDFIXED,
                    XF_TITLE = fil.XF_TITLE,
                    XF_RECORDSIZE = fil.XF_RECORDSIZE,
                }).ToList();

                var relationsByForeign = from r in ctx.RELATE
                                         select new
                                         {
                                             XP_FFILECODE = r.XP_FFILECODE,
                                             XP_DELETERULE = r.XP_DELETERULE,
                                             XP_FINDEXCODE = r.XP_FINDEXCODE,
                                             XP_FLAGS = r.XP_FLAGS,
                                             XP_INSERTRULE = r.XP_INSERTRULE,
                                             XP_OBJINDEXCODE = r.XP_OBJINDEXCODE,
                                             XP_OBJREF = r.XP_OBJREF,
                                             XP_SWITCHFIELDOFS = r.XP_SWITCHFIELDOFS,
                                             XP_PFILECODE = r.XP_PFILECODE,
                                             XP_PINDEXCODE = r.XP_PINDEXCODE,
                                             XP_SWITCHVALUE = r.XP_SWITCHVALUE,
                                             XP_UPDATERULE = r.XP_UPDATERULE,
                                             PrimaryTable = filesWithouRelations.SingleOrDefault(f => f.XF_CODE == r.XP_PFILECODE)
                                         };

                var relationsByPrimary = from r in ctx.RELATE
                                         select new
                                         {
                                             XP_FFILECODE = r.XP_FFILECODE,
                                             XP_DELETERULE = r.XP_DELETERULE,
                                             XP_FINDEXCODE = r.XP_FINDEXCODE,
                                             XP_FLAGS = r.XP_FLAGS,
                                             XP_INSERTRULE = r.XP_INSERTRULE,
                                             XP_OBJINDEXCODE = r.XP_OBJINDEXCODE,
                                             XP_OBJREF = r.XP_OBJREF,
                                             XP_SWITCHFIELDOFS = r.XP_SWITCHFIELDOFS,
                                             XP_PFILECODE = r.XP_PFILECODE,
                                             XP_PINDEXCODE = r.XP_PINDEXCODE,
                                             XP_SWITCHVALUE = r.XP_SWITCHVALUE,
                                             XP_UPDATERULE = r.XP_UPDATERULE,
                                             PrimaryTable = filesWithouRelations.SingleOrDefault(f => f.XF_CODE == r.XP_PFILECODE),
                                             ForeignTable = filesWithouRelations.SingleOrDefault(f => f.XF_CODE == r.XP_FFILECODE),
                                         };
                var files = ctx.FILE.Select(fil => new
                {
                    XF_CODE = fil.XF_CODE,
                    XF_ATTR = fil.XF_ATTR,
                    XF_CHECKSUM = fil.XF_CHECKSUM,
                    XF_COMPONENT = fil.XF_COMPONENT,
                    XF_FLAGS = fil.XF_FLAGS,
                    XF_FLAGS2 = fil.XF_FLAGS2,
                    XF_FORMAT = fil.XF_FORMAT,
                    XF_LOC = fil.XF_LOC,
                    XF_LOC2 = fil.XF_LOC2,
                    XF_NAME = fil.XF_NAME,
                    XF_OWNERNAME = fil.XF_OWNERNAME,
                    XF_PAGESIZE = fil.XF_PAGESIZE,
                    XF_PRIMARYKEYNO = fil.XF_PRIMARYKEYNO,
                    XF_RECORDFIXED = fil.XF_RECORDFIXED,
                    XF_TITLE = fil.XF_TITLE,
                    XF_RECORDSIZE = fil.XF_RECORDSIZE,
                    Fields = ctx.FIELD.Where(f => f.XE_FILECODE == fil.XF_CODE),
                    RelationsByForeign = relationsByForeign.Where(r => r.XP_FFILECODE == fil.XF_CODE),
                    RelationsByPrimary = relationsByPrimary.Where(r => r.XP_PFILECODE == fil.XF_CODE)
                });

                var result = files.ToList();

                var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Model");
                var fileName = "TableInfo.json";
                var json = JsonConvert.SerializeObject(new JObject { [@"TableInfo"] = JToken.FromObject(result) }, Formatting.Indented);
                if (!Directory.Exists(jsonPath))
                    Directory.CreateDirectory(jsonPath);
                using (var fs = new FileStream(Path.Combine(jsonPath, fileName), FileMode.Create))
                {
                    var array = Encoding.UTF8.GetBytes(json);
                    fs.Write(array, 0, array.Length);
                }
            }
        }
    }
}











































