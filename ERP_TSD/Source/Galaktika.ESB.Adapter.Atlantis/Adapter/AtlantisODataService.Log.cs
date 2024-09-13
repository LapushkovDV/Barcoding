using Galaktika.ESB.Utils.LogService;

namespace Galaktika.ESB.Adapter.Atlantis
{
    public partial class AtlantisODataService
    {
        [Log(MethodName = "_Info_ERP_Odata_is_started_OK", DefaultMessage = "ERP Odata is started OK", FormatMessage = "ERP Odata is started OK", LogLevel = "Info", LocalId = 0)]
        private void _Info_ERP_Odata_is_started_OK(LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisODataService>.StartMessageId + 0);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Info", msgDef.Message);
            else
                LogService.Info(msgDef.Message);
        }
    }
}