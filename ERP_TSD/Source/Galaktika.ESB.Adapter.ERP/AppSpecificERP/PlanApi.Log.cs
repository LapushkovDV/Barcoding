using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.Restier.Core;
using Microsoft.Restier.Core.Model;
using Microsoft.Restier.Publishers.OData.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Galaktika.ERP.Interop;
using Galaktika.ESB.Adapter.Atlantis.Api;
using Galaktika.ESB.Adapter.ERP.Adapter;
using Galaktika.ESB.Core;
using Galaktika.ESB.Core.Context;
using Galaktika.ESB.ServiceAdapterApi.ModelBuilder;
using Galaktika.ESB.ServiceAdapterApi;
using Galaktika.ESB.Utils.LogService;

namespace Galaktika.ESB.Adapter.ERP
{
    public partial class PlanApi
    {
        [Log(MethodName = "_Trace_PlanApi_IServiceProvider_srvProv", DefaultMessage = "PlanApi(IServiceProvider srvProv)", FormatMessage = "PlanApi(IServiceProvider srvProv)", LogLevel = "Trace", LocalId = 0)]
        private void _Trace_PlanApi_IServiceProvider_srvProv(LogPackager logPackager = null)
        {
            var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<PlanApi>.StartMessageId + 0);
            if (!msgDef.IsEnabled)
                return;
            if (logPackager != null)
                logPackager.AddMessage("Trace", msgDef.Message);
            else
                LogService.Trace(msgDef.Message);
        }
    }
}