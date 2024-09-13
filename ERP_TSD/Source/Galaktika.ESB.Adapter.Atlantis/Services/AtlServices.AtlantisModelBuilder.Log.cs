using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Galaktika.ERP.Interop;
using Galaktika.ESB.Adapter.Atlantis.Api;
using Galaktika.ESB.ServiceAdapterApi.ModelBuilder;
using Microsoft.OData.Edm;
using Microsoft.Restier.Core;
using Microsoft.Restier.Core.Model;
using Microsoft.EntityFrameworkCore;
using Galaktika.ESB.Utils.LogService;

namespace Galaktika.ERP.Adapter.Atlantis
{
    public partial class AtlServices
    {
        public partial class AtlantisModelBuilder
        {
            [Log(MethodName = "_Warn_Exception_0", DefaultMessage = "Exception : {eToString}", FormatMessage = "Exception : {0}", LogLevel = "Warn", LocalId = 0)]
            private void _Warn_Exception_0(object eToString, LogPackager logPackager = null)
            {
                var msgDef = LogService.GetMessageDef(LogMessageDefs.ForType<AtlantisModelBuilder>.StartMessageId + 0);
                if (!msgDef.IsEnabled)
                    return;
                if (logPackager != null)
                    logPackager.AddMessage("Warn", msgDef.Message, eToString);
                else
                    LogService.Warn(msgDef.Message, eToString);
            }
        }
    }
}