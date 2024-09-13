using Galaktika.ESB.Utils.LogService;

namespace Galaktika.ESB.Adapter.Atlantis
{
    public partial class AtlantisStarterService
    {
        [Log(MethodName = "_Info_Settings_info", DefaultMessage = "Settings info:", FormatMessage = "Settings info:", LogLevel = "Info", LocalId = 0)]
        private void _Info_Settings_info(LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisStarterService>.StartMessageId + 0);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Info", msgDef.Message);
            else
                LogService.Info(msgDef.Message);
        }

        [Log(MethodName = "_Info_GalaktikaPreloadedLibraryList_0", DefaultMessage = "GalaktikaPreloadedLibraryList = {galaktikaPreloadedLibraryList}", FormatMessage = "GalaktikaPreloadedLibraryList = {0}", LogLevel = "Info", LocalId = 1)]
        private void _Info_GalaktikaPreloadedLibraryList_0(object galaktikaPreloadedLibraryList, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisStarterService>.StartMessageId + 1);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Info", msgDef.Message, galaktikaPreloadedLibraryList);
            else
                LogService.Info(msgDef.Message, galaktikaPreloadedLibraryList);
        }

        [Log(MethodName = "_Info_GalaktikaInitCommandArgs_0", DefaultMessage = "GalaktikaInitCommandArgs = {galaktikaInitCommandArgs}", FormatMessage = "GalaktikaInitCommandArgs = {0}", LogLevel = "Info", LocalId = 2)]
        private void _Info_GalaktikaInitCommandArgs_0(object galaktikaInitCommandArgs, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisStarterService>.StartMessageId + 2);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Info", msgDef.Message, galaktikaInitCommandArgs);
            else
                LogService.Info(msgDef.Message, galaktikaInitCommandArgs);
        }

        [Log(MethodName = "_Info_GalaktikaSessionCommandArgs_0", DefaultMessage = "GalaktikaSessionCommandArgs = {galaktikaSessionCommandArgs}", FormatMessage = "GalaktikaSessionCommandArgs = {0}", LogLevel = "Info", LocalId = 3)]
        private void _Info_GalaktikaSessionCommandArgs_0(object galaktikaSessionCommandArgs, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisStarterService>.StartMessageId + 3);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Info", msgDef.Message, galaktikaSessionCommandArgs);
            else
                LogService.Info(msgDef.Message, galaktikaSessionCommandArgs);
        }

        [Log(MethodName = "_Info_LogicalProcessorCount_0", DefaultMessage = "LogicalProcessorCount = {logicalProcessorCountValue}", FormatMessage = "LogicalProcessorCount = {0}", LogLevel = "Info", LocalId = 4)]
        private void _Info_LogicalProcessorCount_0(object logicalProcessorCountValue, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisStarterService>.StartMessageId + 4);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Info", msgDef.Message, logicalProcessorCountValue);
            else
                LogService.Info(msgDef.Message, logicalProcessorCountValue);
        }

        [Log(MethodName = "_Info_MaxSleepOnStartCount_0", DefaultMessage = "MaxSleepOnStartCount = {maxSleepOnStartCountValue}", FormatMessage = "MaxSleepOnStartCount = {0}", LogLevel = "Info", LocalId = 5)]
        private void _Info_MaxSleepOnStartCount_0(object maxSleepOnStartCountValue, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisStarterService>.StartMessageId + 5);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Info", msgDef.Message, maxSleepOnStartCountValue);
            else
                LogService.Info(msgDef.Message, maxSleepOnStartCountValue);
        }

        [Log(MethodName = "_Info_Concurrency_0", DefaultMessage = "Concurrency = {concurrencyValue}", FormatMessage = "Concurrency = {0}", LogLevel = "Info", LocalId = 6)]
        private void _Info_Concurrency_0(object concurrencyValue, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisStarterService>.StartMessageId + 6);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Info", msgDef.Message, concurrencyValue);
            else
                LogService.Info(msgDef.Message, concurrencyValue);
        }

        [Log(MethodName = "_Info_LoggingLevel_0", DefaultMessage = "LoggingLevel = {loggingLevelValue}", FormatMessage = "LoggingLevel = {0}", LogLevel = "Info", LocalId = 7)]
        private void _Info_LoggingLevel_0(object loggingLevelValue, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisStarterService>.StartMessageId + 7);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Info", msgDef.Message, loggingLevelValue);
            else
                LogService.Info(msgDef.Message, loggingLevelValue);
        }

        [Log(MethodName = "_Info_UseNewStarter_0", DefaultMessage = "UseNewStarter {useNewStarterValue}", FormatMessage = "UseNewStarter {0}", LogLevel = "Info", LocalId = 8)]
        private void _Info_UseNewStarter_0(object useNewStarterValue, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisStarterService>.StartMessageId + 8);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Info", msgDef.Message, useNewStarterValue);
            else
                LogService.Info(msgDef.Message, useNewStarterValue);
        }

        [Log(MethodName = "_Info_UseWorkerContexts_0", DefaultMessage = "UseWorkerContexts = {useWorkerContextsValue}", FormatMessage = "UseWorkerContexts = {0}", LogLevel = "Info", LocalId = 9)]
        private void _Info_UseWorkerContexts_0(object useWorkerContextsValue, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisStarterService>.StartMessageId + 9);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Info", msgDef.Message, useWorkerContextsValue);
            else
                LogService.Info(msgDef.Message, useWorkerContextsValue);
        }

        [Log(MethodName = "_Info_UseAtlContexts_0", DefaultMessage = "UseAtlContexts = {useAtlContextsValue}", FormatMessage = "UseAtlContexts = {0}", LogLevel = "Info", LocalId = 10)]
        private void _Info_UseAtlContexts_0(object useAtlContextsValue, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisStarterService>.StartMessageId + 10);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Info", msgDef.Message, useAtlContextsValue);
            else
                LogService.Info(msgDef.Message, useAtlContextsValue);
        }

        [Log(MethodName = "_Info_UseDispatcherLight_0", DefaultMessage = "UseDispatcherLight = {useDispatcherLightValue}", FormatMessage = "UseDispatcherLight = {0}", LogLevel = "Info", LocalId = 11)]
        private void _Info_UseDispatcherLight_0(object useDispatcherLightValue, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisStarterService>.StartMessageId + 11);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Info", msgDef.Message, useDispatcherLightValue);
            else
                LogService.Info(msgDef.Message, useDispatcherLightValue);
        }

        [Log(MethodName = "_Info_ShutdownAppOnGalThreadWorkerExit_0", DefaultMessage = "ShutdownAppOnGalThreadWorkerExit = {shutdownAppOnGalThreadWorkerExitValue}", FormatMessage = "ShutdownAppOnGalThreadWorkerExit = {0}", LogLevel = "Info", LocalId = 12)]
        private void _Info_ShutdownAppOnGalThreadWorkerExit_0(object shutdownAppOnGalThreadWorkerExitValue, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisStarterService>.StartMessageId + 12);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Info", msgDef.Message, shutdownAppOnGalThreadWorkerExitValue);
            else
                LogService.Info(msgDef.Message, shutdownAppOnGalThreadWorkerExitValue);
        }

        [Log(MethodName = "_Info_GalExePath_0", DefaultMessage = "GalExePath = {galExe}", FormatMessage = "GalExePath = {0}", LogLevel = "Info", LocalId = 13)]
        private void _Info_GalExePath_0(object galExe, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisStarterService>.StartMessageId + 13);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Info", msgDef.Message, galExe);
            else
                LogService.Info(msgDef.Message, galExe);
        }

        [Log(MethodName = "_Info_IsDsqlInsert_0", DefaultMessage = "IsDsqlInsert = {res}", FormatMessage = "IsDsqlInsert = {0}", LogLevel = "Info", LocalId = 14)]
        private void _Info_IsDsqlInsert_0(object res, LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisStarterService>.StartMessageId + 14);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Info", msgDef.Message, res);
            else
                LogService.Info(msgDef.Message, res);
        }

		[Log(MethodName = "_Info_GetAppEntitiesInOperatorItemsCount_0", DefaultMessage = "GetAppEntitiesInOperatorItemsCount = {res}", FormatMessage = "GetAppEntitiesInOperatorItemsCount = {0}", LogLevel = "Info", LocalId = 15)]
		private void _Info_GetAppEntitiesInOperatorItemsCount_0(object res, LogPackager logPackager = null)
		{
			var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisStarterService>.StartMessageId + 15);
			if (!msgDef.IsEnabled)
				return;
			if (logPackager != null)
				logPackager.AddMessage("Info", msgDef.Message, res);
			else
				LogService.Info(msgDef.Message, res);
		}
	}
}