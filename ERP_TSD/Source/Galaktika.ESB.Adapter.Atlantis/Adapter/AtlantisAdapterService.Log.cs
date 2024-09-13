using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Galaktika.ERP.Interop;
using Galaktika.ESB.Adapter.Atlantis.Api;
using Galaktika.ESB.Adapter.ERP;
using Galaktika.ESB.Core.Exceptions;
using Galaktika.ESB.ServiceAdapterApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Galaktika.ESB.Utils.LogService;

namespace Galaktika.ESB.Adapter.Atlantis
{
    public partial class AtlantisAdapterService
    {
        [Log(MethodName = "_Trace_LastAdapterActivity_0_LastDispatcherTread_1", DefaultMessage = "LastAdapterActivity = {LastAdapterActivity}. LastDispatcherTread = {DispatcherHelper.LastDispatcherActivityThreadId}", FormatMessage = "LastAdapterActivity = {0}. LastDispatcherTread = {1}", LogLevel = "Trace", LocalId = 0)]
        private void _Trace_LastAdapterActivity_0_LastDispatcherTread_1(LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisAdapterService>.StartMessageId + 0);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Trace", msgDef.Message, LastAdapterActivity, DispatcherHelper.LastDispatcherActivityThreadId);
            else
                LogService.Trace(msgDef.Message, LastAdapterActivity, DispatcherHelper.LastDispatcherActivityThreadId);
        }

        [Log(MethodName = "_Info_Выполнение_запроса_длилось_более_0_секунд_Адаптер_упадет_по_таймауту", DefaultMessage = "Выполнение запроса длилось более {RequestTimeOut / 1000} секунд. Адаптер упадет по таймауту", FormatMessage = "Выполнение запроса длилось более {0} секунд. Адаптер упадет по таймауту", LogLevel = "Info", LocalId = 1)]
        private void _Info_Выполнение_запроса_длилось_более_0_секунд_Адаптер_упадет_по_таймауту(LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisAdapterService>.StartMessageId + 1);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Info", msgDef.Message, RequestTimeOut / 1000);
            else
                LogService.Info(msgDef.Message, RequestTimeOut / 1000);
        }

        [Log(MethodName = "_Info_Запрос_на_то_живой_ли_адаптер", DefaultMessage = "Запрос на то, живой ли адаптер", FormatMessage = "Запрос на то, живой ли адаптер", LogLevel = "Info", LocalId = 2)]
        private void _Info_Запрос_на_то_живой_ли_адаптер(LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisAdapterService>.StartMessageId + 2);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Info", msgDef.Message);
            else
                LogService.Info(msgDef.Message);
        }

        [Log(MethodName = "_Info_Возвращение_результата_запроса", DefaultMessage = "Возвращение результата запроса", FormatMessage = "Возвращение результата запроса", LogLevel = "Info", LocalId = 3)]
        private void _Info_Возвращение_результата_запроса(LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisAdapterService>.StartMessageId + 3);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Info", msgDef.Message);
            else
                LogService.Info(msgDef.Message);
        }
    }
}