using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Galaktika.ERP.Interop;
using Galaktika.ESB.Adapter.ERP;
using Galaktika.ESB.Adapter.Services;
using Galaktika.ESB.Core.Services.Infrastructure;
using Galaktika.ESB.ServiceAdapterApi;
using Microsoft.Extensions.DependencyInjection;
using Galaktika.ESB.Utils.LogService;

namespace Galaktika.ESB.Adapter.Atlantis
{
    public partial class AtlantisODataAdapter
    {
        [Log(MethodName = "_Info_AtlantisAdapter_Adapter_is_started_OK", DefaultMessage = "AtlantisAdapter Adapter is started OK", FormatMessage = "AtlantisAdapter Adapter is started OK", LogLevel = "Info", LocalId = 0)]
        private void _Info_AtlantisAdapter_Adapter_is_started_OK(LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisODataAdapter>.StartMessageId + 0);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Info", msgDef.Message);
            else
                LogService.Info(msgDef.Message);
        }
    }
}