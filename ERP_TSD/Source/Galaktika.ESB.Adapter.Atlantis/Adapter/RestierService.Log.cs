using Galaktika.ESB.Utils.LogService;

namespace Galaktika.ESB.Adapter.Atlantis
{
    public partial class RestierService
    {
        [Log(MethodName = "LogTraceСreatingSession", DefaultMessage = "Сreating session. Thread={threadId};", FormatMessage = "Сreating session. Thread={0};", LogLevel = "Trace", LocalId = 0)]
        private void LogTraceСreatingSession(object threadId, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<RestierService>.StartMessageId + 0);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Trace", msgDef.Message, threadId);
            else
                LogService.Trace(msgDef.Message, threadId);
        }

        [Log(MethodName = "_Warn_Не_удалось_получить_атлантис_сервис_таймаут_на_длительную_операцию_не_будет_установлен_для_данной_функции", DefaultMessage = "Не удалось получить атлантис сервис, таймаут на длительную операцию не будет установлен для данной функции", FormatMessage = "Не удалось получить атлантис сервис, таймаут на длительную операцию не будет установлен для данной функции", LogLevel = "Warn", LocalId = 1)]
        private void _Warn_Не_удалось_получить_атлантис_сервис_таймаут_на_длительную_операцию_не_будет_установлен_для_данной_функции(LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<RestierService>.StartMessageId + 1);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Warn", msgDef.Message);
            else
                LogService.Warn(msgDef.Message);
        }

        [Log(MethodName = "_Info_Set_operation_timeout_0", DefaultMessage = "Set operation timeout = {res}", FormatMessage = "Set operation timeout = {0}", LogLevel = "Info", LocalId = 2)]
        private void _Info_Set_operation_timeout_0(object res, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<RestierService>.StartMessageId + 2);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Info", msgDef.Message, res);
            else
                LogService.Info(msgDef.Message, res);
        }
    }
}