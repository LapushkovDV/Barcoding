using Galaktika.ESB.Utils.LogService;

namespace Galaktika.ESB.Adapter.Atlantis
{
    /// <summary>
    /// Updates service for adapter ERP. Checks database change and publishes them.
    /// </summary>
    public partial class AtlantisUpdatesService
    {
        [Log(MethodName = "LogWarnTableIsNotFound", DefaultMessage = "ErpUpdatesService.GetTrackingTables: Table is not found for type = {trackingType}. Exception message: {eMessage}", FormatMessage = "ErpUpdatesService.GetTrackingTables: Table is not found for type = {0}. Exception message: {1}", LogLevel = "Warn", LocalId = 0)]
        private void LogWarnTableIsNotFound(object trackingType, object eMessage, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 0);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Warn", msgDef.Message, trackingType, eMessage);
            else
                LogService.Warn(msgDef.Message, trackingType, eMessage);
        }

        [Log(MethodName = "LogWarnUserNotFound", DefaultMessage = "Не обнаружен пользователь для которого определять офис. UserName = {userName}. Адаптер работает только в режиме приема сообщений.", FormatMessage = "Не обнаружен пользователь для которого определять офис. UserName = {0}. Адаптер работает только в режиме приема сообщений.", LogLevel = "Warn", LocalId = 1)]
        private void LogWarnUserNotFound(object userName, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 1);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Warn", msgDef.Message, userName);
            else
                LogService.Warn(msgDef.Message, userName);
        }

        [Log(MethodName = "LogWarnConfigNotFound", DefaultMessage = "Не обнаружено описание конфигурации журнала для пользователя {login} с офисом {office}", FormatMessage = "Не обнаружено описание конфигурации журнала для пользователя {0} с офисом {1}", LogLevel = "Warn", LocalId = 2)]
        private void LogWarnConfigNotFound(object login, object office, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 2);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Warn", msgDef.Message, login, office);
            else
                LogService.Warn(msgDef.Message, login, office);
        }

        [Log(MethodName = "LogWarnLoggingOff", DefaultMessage = "Журнализация для пользователя {userName} с офисом {officeNumber} выключена. Адаптер работает только в режиме приема сообщений.", FormatMessage = "Журнализация для пользователя {0} с офисом {1} выключена. Адаптер работает только в режиме приема сообщений.", LogLevel = "Warn", LocalId = 3)]
        private void LogWarnLoggingOff(object userName, object officeNumber, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 3);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Warn", msgDef.Message, userName, officeNumber);
            else
                LogService.Warn(msgDef.Message, userName, officeNumber);
        }

        [Log(MethodName = "LogWarnNoUserForConfigFound", DefaultMessage = "Не найден абонент для конфигурации с офисом №{journalConfOFFICENO} для пользователя {loginb}", FormatMessage = "Не найден абонент для конфигурации с офисом №{0} для пользователя {1}", LogLevel = "Warn", LocalId = 4)]
        private void LogWarnNoUserForConfigFound(object journalConfOFFICENO, object loginb, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 4);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Warn", msgDef.Message, journalConfOFFICENO, loginb);
            else
                LogService.Warn(msgDef.Message, journalConfOFFICENO, loginb);
        }

        [Log(MethodName = "LogWarnNoTrackingEntityesFound", DefaultMessage = "В модели не определены сущности для отслеживания. Адаптер работает только в режиме приема сообщений.", FormatMessage = "В модели не определены сущности для отслеживания. Адаптер работает только в режиме приема сообщений.", LogLevel = "Warn", LocalId = 5)]
        private void LogWarnNoTrackingEntityesFound(LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 5);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Warn", msgDef.Message);
            else
                LogService.Warn(msgDef.Message);
        }

        [Log(MethodName = "LogWarnLoggingOffForTable", DefaultMessage = "Журнализация отключена для таблицы с номером {tableNum}, которая определена в модели интеграции.", FormatMessage = "Журнализация отключена для таблицы с номером {0}, которая определена в модели интеграции.", LogLevel = "Warn", LocalId = 6)]
        private void LogWarnLoggingOffForTable(object tableNum, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 6);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Warn", msgDef.Message, tableNum);
            else
                LogService.Warn(msgDef.Message, tableNum);
        }

        [Log(MethodName = "LogWarnLoggingOffForAllTables", DefaultMessage = "Журнализация выключена для всех таблиц, определенных в модели интеграции. Адаптер работает только в режиме приема сообщений.", FormatMessage = "Журнализация выключена для всех таблиц, определенных в модели интеграции. Адаптер работает только в режиме приема сообщений.", LogLevel = "Warn", LocalId = 7)]
        private void LogWarnLoggingOffForAllTables(LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 7);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Warn", msgDef.Message);
            else
                LogService.Warn(msgDef.Message);
        }

        [Log(MethodName = "LogTraceStartOffice", DefaultMessage = "CheckDbUpdates4Office. START. OFFICE = {office}", FormatMessage = "CheckDbUpdates4Office. START. OFFICE = {0}", LogLevel = "Trace", LocalId = 8)]
        private void LogTraceStartOffice(object office, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 8);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Trace", msgDef.Message, office);
            else
                LogService.Trace(msgDef.Message, office);
        }

        [Log(MethodName = "LogTraceMinNrec", DefaultMessage = "CheckDbUpdates4Office. minNrec = {minNrec}", FormatMessage = "CheckDbUpdates4Office. minNrec = {0}", LogLevel = "Trace", LocalId = 9)]
        private void LogTraceMinNrec(object minNrec, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 9);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Trace", msgDef.Message, minNrec);
            else
                LogService.Trace(msgDef.Message, minNrec);
        }

        [Log(MethodName = "LogTraceMaxNrec", DefaultMessage = "CheckDbUpdates4Office. maxNrec = {maxNrec}", FormatMessage = "CheckDbUpdates4Office. maxNrec = {0}", LogLevel = "Trace", LocalId = 10)]
        private void LogTraceMaxNrec(object maxNrec, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 10);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Trace", msgDef.Message, maxNrec);
            else
                LogService.Trace(msgDef.Message, maxNrec);
        }

        [Log(MethodName = "LogTrace2Step1", DefaultMessage = "CheckDbUpdates4Office. 2. step-1", FormatMessage = "CheckDbUpdates4Office. 2. step-1", LogLevel = "Trace", LocalId = 11)]
        private void LogTrace2Step1(LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 11);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Trace", msgDef.Message);
            else
                LogService.Trace(msgDef.Message);
        }

        [Log(MethodName = "LogTrace2Step2", DefaultMessage = "CheckDbUpdates4Office. 2. step-2", FormatMessage = "CheckDbUpdates4Office. 2. step-2", LogLevel = "Trace", LocalId = 12)]
        private void LogTrace2Step2(LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 12);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Trace", msgDef.Message);
            else
                LogService.Trace(msgDef.Message);
        }

        [Log(MethodName = "LogTrace2Step3", DefaultMessage = "CheckDbUpdates4Office. 2. step-3", FormatMessage = "CheckDbUpdates4Office. 2. step-3", LogLevel = "Trace", LocalId = 13)]
        private void LogTrace2Step3(LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 13);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Trace", msgDef.Message);
            else
                LogService.Trace(msgDef.Message);
        }

        [Log(MethodName = "LogTrace2Step4", DefaultMessage = "CheckDbUpdates4Office. 2. step-4", FormatMessage = "CheckDbUpdates4Office. 2. step-4", LogLevel = "Trace", LocalId = 14)]
        private void LogTrace2Step4(LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 14);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Trace", msgDef.Message);
            else
                LogService.Trace(msgDef.Message);
        }

        [Log(MethodName = "LogTrace3MinRec", DefaultMessage = "CheckDbUpdates4Office. 3. minNrec = {minNrec}", FormatMessage = "CheckDbUpdates4Office. 3. minNrec = {0}", LogLevel = "Trace", LocalId = 15)]
        private void LogTrace3MinRec(object minNrec, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 15);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Trace", msgDef.Message, minNrec);
            else
                LogService.Trace(msgDef.Message, minNrec);
        }

        [Log(MethodName = "LogTrace3MaxRec", DefaultMessage = "CheckDbUpdates4Office. 3. maxNrec = {maxNrec}", FormatMessage = "CheckDbUpdates4Office. 3. maxNrec = {0}", LogLevel = "Trace", LocalId = 16)]
        private void LogTrace3MaxRec(object maxNrec, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 16);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Trace", msgDef.Message, maxNrec);
            else
                LogService.Trace(msgDef.Message, maxNrec);
        }

        [Log(MethodName = "LogTracePublishDictLength", DefaultMessage = "_publishDict length = {publishDictCount}", FormatMessage = "_publishDict length = {0}", LogLevel = "Trace", LocalId = 17)]
        private void LogTracePublishDictLength(object publishDictCount, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 17);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Trace", msgDef.Message, publishDictCount);
            else
                LogService.Trace(msgDef.Message, publishDictCount);
        }

        [Log(MethodName = "LogTrace1MinRec", DefaultMessage = "CheckDbUpdates4Office. 1. minNrec = {minNrec}", FormatMessage = "CheckDbUpdates4Office. 1. minNrec = {0}", LogLevel = "Trace", LocalId = 18)]
        private void LogTrace1MinRec(object minNrec, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 18);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Trace", msgDef.Message, minNrec);
            else
                LogService.Trace(msgDef.Message, minNrec);
        }

        [Log(MethodName = "LogTraceMinNRec", DefaultMessage = "minNRec {minNrecToString}", FormatMessage = "minNRec {0}", LogLevel = "Trace", LocalId = 19)]
        private void LogTraceMinNRec(object minNrecToString, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 19);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Trace", msgDef.Message, minNrecToString);
            else
                LogService.Trace(msgDef.Message, minNrecToString);
        }

        [Log(MethodName = "LogTrace1MaxNRec", DefaultMessage = "CheckDbUpdates4Office. 1. maxNrec = {maxNrec}", FormatMessage = "CheckDbUpdates4Office. 1. maxNrec = {0}", LogLevel = "Trace", LocalId = 20)]
        private void LogTrace1MaxNRec(object maxNrec, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 20);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Trace", msgDef.Message, maxNrec);
            else
                LogService.Trace(msgDef.Message, maxNrec);
        }

        [Log(MethodName = "LogTraceBegin", DefaultMessage = "CheckDbUpdates. Begin. {states}", FormatMessage = "CheckDbUpdates. Begin. {0}", LogLevel = "Trace", LocalId = 21)]
        private void LogTraceBegin(object states, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 21);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Trace", msgDef.Message, states);
            else
                LogService.Trace(msgDef.Message, states);
        }

        [Log(MethodName = "LogTracePublishChangeFor", DefaultMessage = "Publish change for {tableDAssemblyQualifiedName}", FormatMessage = "Publish change for {0}", LogLevel = "Trace", LocalId = 22)]
        private void LogTracePublishChangeFor(object tableDAssemblyQualifiedName, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 22);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Trace", msgDef.Message, tableDAssemblyQualifiedName);
            else
                LogService.Trace(msgDef.Message, tableDAssemblyQualifiedName);
        }

        [Log(MethodName = "LogTraceEnd", DefaultMessage = "CheckDbUpdates. End. {states}", FormatMessage = "CheckDbUpdates. End. {0}", LogLevel = "Trace", LocalId = 23)]
        private void LogTraceEnd(object states, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 23);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Trace", msgDef.Message, states);
            else
                LogService.Trace(msgDef.Message, states);
        }

        [Log(MethodName = "_Info_Чистка_утерянных_нреков_по_времени_Количество_0", DefaultMessage = "Чистка утерянных нреков по времени. Количество = {lostNrecsForCleanUpCount}", FormatMessage = "Чистка утерянных нреков по времени. Количество = {0}", LogLevel = "Info", LocalId = 24)]
        private void _Info_Чистка_утерянных_нреков_по_времени_Количество_0(object lostNrecsForCleanUpCount, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 24);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Info", msgDef.Message, lostNrecsForCleanUpCount);
            else
                LogService.Info(msgDef.Message, lostNrecsForCleanUpCount);
        }

        [Log(MethodName = "_Info_Добавление_новых_дырок_Количество_0", DefaultMessage = "Добавление новых \"дырок\". Количество = {lostJournalNrecsCount}", FormatMessage = "Добавление новых \"дырок\". Количество = {0}", LogLevel = "Info", LocalId = 25)]
        private void _Info_Добавление_новых_дырок_Количество_0(object lostJournalNrecsCount, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 25);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Info", msgDef.Message, lostJournalNrecsCount);
            else
                LogService.Info(msgDef.Message, lostJournalNrecsCount);
        }

        [Log(MethodName = "_Info_Чистка_проверенно_записи_в_утерянных_нреках_Старт_0_конец_1_Количество_2", DefaultMessage = "Чистка проверенно записи в утерянных нреках. Старт = {rangeStartToString}, конец = {rangeEndToString}. Количество = {clearCount}", FormatMessage = "Чистка проверенно записи в утерянных нреках. Старт = {0}, конец = {1}. Количество = {2}", LogLevel = "Info", LocalId = 26)]
        private void _Info_Чистка_проверенно_записи_в_утерянных_нреках_Старт_0_конец_1_Количество_2(object rangeStartToString, object rangeEndToString, object clearCount, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 26);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Info", msgDef.Message, rangeStartToString, rangeEndToString, clearCount);
            else
                LogService.Info(msgDef.Message, rangeStartToString, rangeEndToString, clearCount);
        }

        [Log(MethodName = "_Info_UserOffice_0", DefaultMessage = "UserOffice = {ownUserXU_USEROFFICE}", FormatMessage = "UserOffice = {0}", LogLevel = "Info", LocalId = 27)]
        private void _Info_UserOffice_0(object ownUserXU_USEROFFICE, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 27);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Info", msgDef.Message, ownUserXU_USEROFFICE);
            else
                LogService.Info(msgDef.Message, ownUserXU_USEROFFICE);
        }

        [Log(MethodName = "_Info_AbonNrec_0", DefaultMessage = "AbonNrec = {abonNrec}", FormatMessage = "AbonNrec = {0}", LogLevel = "Info", LocalId = 28)]
        private void _Info_AbonNrec_0(object abonNrec, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 28);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Info", msgDef.Message, abonNrec);
            else
                LogService.Info(msgDef.Message, abonNrec);
        }

        [Log(MethodName = "_Info_Диапазон_потери_начало_0_конец_1", DefaultMessage = "Диапазон \"потери\" начало = {lostJournalNrecStartRangeNrec}, конец = {lostJournalNrecEndRangeNrec}", FormatMessage = "Диапазон \"потери\" начало = {0}, конец = {1}", LogLevel = "Info", LocalId = 29)]
        private void _Info_Диапазон_потери_начало_0_конец_1(object lostJournalNrecStartRangeNrec, object lostJournalNrecEndRangeNrec, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 29);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Info", msgDef.Message, lostJournalNrecStartRangeNrec, lostJournalNrecEndRangeNrec);
            else
                LogService.Info(msgDef.Message, lostJournalNrecStartRangeNrec, lostJournalNrecEndRangeNrec);
        }

        [Log(MethodName = "_Trace_Result_true_при_проверке_потерянных_нреков_Запись_диапазона_Старт_0_конец_1", DefaultMessage = "Result = true при проверке потерянных нреков. Запись диапазона. Старт = {rangeStart}, конец = {rangeEnd}", FormatMessage = "Result = true при проверке потерянных нреков. Запись диапазона. Старт = {0}, конец = {1}", LogLevel = "Trace", LocalId = 30)]
        private void _Trace_Result_true_при_проверке_потерянных_нреков_Запись_диапазона_Старт_0_конец_1(object rangeStart, object rangeEnd, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 30);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Trace", msgDef.Message, rangeStart, rangeEnd);
            else
                LogService.Trace(msgDef.Message, rangeStart, rangeEnd);
        }

        [Log(MethodName = "_Trace_Запись_состояния_потерянных_нреков_Перед_if__rangeEnd_0__rangeStart_0_End_0_Start_1", DefaultMessage = "Запись состояния потерянных нреков. Перед if(_rangeEnd != 0 && _rangeStart != 0). End = {rangeEnd}, Start = {rangeStart}", FormatMessage = "Запись состояния потерянных нреков. Перед if(_rangeEnd != 0 && _rangeStart != 0). End = {0}, Start = {1}", LogLevel = "Trace", LocalId = 31)]
        private void _Trace_Запись_состояния_потерянных_нреков_Перед_if__rangeEnd_0__rangeStart_0_End_0_Start_1(object rangeEnd, object rangeStart, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 31);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Trace", msgDef.Message, rangeEnd, rangeStart);
            else
                LogService.Trace(msgDef.Message, rangeEnd, rangeStart);
        }

        [Log(MethodName = "_Trace_Начало_чистки_проверенных_потерянных_нреков", DefaultMessage = "Начало чистки проверенных потерянных нреков", FormatMessage = "Начало чистки проверенных потерянных нреков", LogLevel = "Trace", LocalId = 32)]
        private void _Trace_Начало_чистки_проверенных_потерянных_нреков(LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 32);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Trace", msgDef.Message);
            else
                LogService.Trace(msgDef.Message);
        }

        [Log(MethodName = "_Trace_Сохранение_чистки_проверенных_потерянных_нреков", DefaultMessage = "Сохранение чистки проверенных потерянных нреков", FormatMessage = "Сохранение чистки проверенных потерянных нреков", LogLevel = "Trace", LocalId = 33)]
        private void _Trace_Сохранение_чистки_проверенных_потерянных_нреков(LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 33);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Trace", msgDef.Message);
            else
                LogService.Trace(msgDef.Message);
        }

        [Log(MethodName = "_Trace_Journal_notes_count_0_Min_NREC_1_Max_NREC_2", DefaultMessage = "Journal notes count = {journalNotesLength}. Min NREC = {minNrec}, Max NREC = {maxNrec}", FormatMessage = "Journal notes count = {0}. Min NREC = {1}, Max NREC = {2}", LogLevel = "Trace", LocalId = 34)]
        private void _Trace_Journal_notes_count_0_Min_NREC_1_Max_NREC_2(object journalNotesLength, object minNrec, object maxNrec, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 34);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Trace", msgDef.Message, journalNotesLength, minNrec, maxNrec);
            else
                LogService.Trace(msgDef.Message, journalNotesLength, minNrec, maxNrec);
        }

        [Log(MethodName = "_Info_Не_удалось_преобразовать_строку_0_в_int", DefaultMessage = "Не удалось преобразовать строку - {p} в int!", FormatMessage = "Не удалось преобразовать строку - {0} в int!", LogLevel = "Info", LocalId = 35)]
        private void _Info_Не_удалось_преобразовать_строку_0_в_int(object p, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 35);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Info", msgDef.Message, p);
            else
                LogService.Info(msgDef.Message, p);
        }

        [Log(MethodName = "_Warn_Отсутствует_абонент_с_номером_0_либо_журнализация_для_него_выключена", DefaultMessage = "Отсутствует абонент с номером {oft} либо журнализация для него выключена.", FormatMessage = "Отсутствует абонент с номером {0} либо журнализация для него выключена.", LogLevel = "Warn", LocalId = 36)]
        private void _Warn_Отсутствует_абонент_с_номером_0_либо_журнализация_для_него_выключена(object oft, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisUpdatesService>.StartMessageId + 36);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Warn", msgDef.Message, oft);
            else
                LogService.Warn(msgDef.Message, oft);
        }
    }
}