using System;
using System.Threading.Tasks;
using Galaktika.ERP.Interop;
using Galaktika.ESB.ServiceAdapterApi;
using Microsoft.Extensions.DependencyInjection;

namespace Galaktika.ESB.Adapter.Atlantis
{
    /// <summary>
    /// Ensure start Galaktika ERP
    /// </summary>
    public partial class AtlantisStarterService : ServiceBase
    {
        /// <summary>
        /// Конфигурирует сервис
        /// </summary>
        /// <param name="services"></param>
        public override void Configure(IServiceCollection services)
        {
            base.Configure(services);
            _Info_Settings_info();
            var galExe = (string)this.Settings["GalaktikaExePath"];

            var galaktikaPreloadedLibraryList = (string)this.Settings["GalaktikaPreloadedLibraryList"];
            if (!string.IsNullOrEmpty(galaktikaPreloadedLibraryList))
            {
                _Info_GalaktikaPreloadedLibraryList_0(galaktikaPreloadedLibraryList);
                GalaktikaErpStarter.PreloadedLibraryList =
                    galaktikaPreloadedLibraryList;
            }

            var galaktikaInitCommandArgs = (string)this.Settings["GalaktikaInitCommandArgs"];
            if (!string.IsNullOrEmpty(galaktikaInitCommandArgs))
            {
                _Info_GalaktikaInitCommandArgs_0(galaktikaInitCommandArgs);
                GalaktikaErpStarter.InitCommandArgs = galaktikaInitCommandArgs;
            }

            var galaktikaSessionCommandArgs = (string)this.Settings["GalaktikaSessionCommandArgs"];
            if (!string.IsNullOrEmpty(galaktikaSessionCommandArgs))
            {
                _Info_GalaktikaSessionCommandArgs_0(galaktikaSessionCommandArgs);
                GalaktikaErpStarter.SessionCommandArgs = galaktikaSessionCommandArgs;
            }

            var logicalProcessorCount = (uint?)this.Settings["LogicalProcessorCount"];
            if (logicalProcessorCount.HasValue)
            {
                _Info_LogicalProcessorCount_0(logicalProcessorCount.Value);
                GalaktikaErpStarter.LogicalProcessorCount =
                    logicalProcessorCount.Value;
            }

            var maxSleepOnStartCount = (int?)this.Settings["MaxSleepOnStartCount"];
            if (maxSleepOnStartCount.HasValue)
            {
                _Info_MaxSleepOnStartCount_0(maxSleepOnStartCount.Value);
                GalaktikaErpStarter.MaxSleepOnStartCount = maxSleepOnStartCount.Value;
            }

            var concurrency = (int?)this.Settings["Concurrency"];
            if (concurrency.HasValue)
            {
                _Info_Concurrency_0(concurrency.Value);
                GalaktikaErpStarter.Concurrency = concurrency.Value;
            }

            var loggingLevel = (int?)this.Settings["LoggingLevel"];
            if (loggingLevel.HasValue)
            {
                _Info_LoggingLevel_0(loggingLevel.Value);
                GalaktikaErpStarter.LoggingLevel = loggingLevel.Value;
            }

            var useNewStarter = (bool?)this.Settings["UseNewStarter"];
            if (useNewStarter.HasValue)
            {
                _Info_UseNewStarter_0(useNewStarter.Value);
                GalaktikaErpStarter.UseNewStarter = useNewStarter.Value;
            }

            var useWorkerContexts = (bool?)this.Settings["UseWorkerContexts"];
            if (useWorkerContexts.HasValue)
            {
                _Info_UseWorkerContexts_0(useWorkerContexts.Value);
                GalaktikaErpStarter.UseWorkerContexts = useWorkerContexts.Value;
            }

            var useAtlContexts = (bool?)this.Settings["UseAtlContexts"];
            if (useAtlContexts.HasValue)
            {
                _Info_UseAtlContexts_0(useAtlContexts.Value);
                GalaktikaErpStarter.UseAtlContexts = useAtlContexts.Value;
            }

            var useDispatcherLight = (bool?)this.Settings["UseDispatcherLight"];
            if (useDispatcherLight.HasValue)
            {
                _Info_UseDispatcherLight_0(useDispatcherLight.Value);
                GalaktikaErpStarter.UseDispatcherLight = useDispatcherLight.Value;
            }

            var shutdownAppOnGalThreadWorkerExit = (bool?)this.Settings["ShutdownAppOnGalThreadWorkerExit"];
            if (shutdownAppOnGalThreadWorkerExit.HasValue)
            {
                _Info_ShutdownAppOnGalThreadWorkerExit_0(shutdownAppOnGalThreadWorkerExit.Value);
                GalaktikaErpStarter.ShutdownAppOnGalThreadWorkerExit =
                    shutdownAppOnGalThreadWorkerExit.Value;
            }

            GalaktikaErpStarter.AppendPathBefore = AppDomain.CurrentDomain.BaseDirectory;
            if (!string.IsNullOrEmpty(galExe))
            {
                _Info_GalExePath_0(galExe);
                GalaktikaErpStarter.AppendPathBefore += ";" + galExe;
            }

            var isDsqlInsert = Settings["IsDsqlInsert"];
            if (isDsqlInsert != null)
            {
                if (bool.TryParse(isDsqlInsert.ToString(), out var res))
                {
                    _Info_IsDsqlInsert_0(res);
                    AtlantisTable.IsDsqlInsert = res;
                }
            }

			AtlantisTable.GetAppEntitiesInOperatorItemsCount = 100;		// По умолчанию пусть будет равно 100
			var GetAppEntitiesInOperatorItemsCount = Settings["GetAppEntitiesInOperatorItemsCount"];
			if (GetAppEntitiesInOperatorItemsCount != null)
			{
				if (int.TryParse(GetAppEntitiesInOperatorItemsCount.ToString(), out var res))
				{
					AtlantisTable.GetAppEntitiesInOperatorItemsCount = res;
				}
			}
			_Info_GetAppEntitiesInOperatorItemsCount_0(AtlantisTable.GetAppEntitiesInOperatorItemsCount);

			//DispatcherHelper.OnStop += DispatcherHelper_OnStop;
			GalaktikaErpStarter.EnsureStartGalaktikaErp();

            services.AddSingleton<AtlantisStarterService>(this);
        }
        /// <summary>
        /// Stops atlantis sterter service
        /// </summary>
        /// <returns></returns>
        public override Task Stop()
        {
            //DispatcherHelper.BeginInvokeShutdown(DispatcherPriority.Normal);
            GalaktikaErpStarter.EnsureShutDownGalaktikaErp();
            return base.Stop();
        }

    }
}

























