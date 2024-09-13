using System;
using Galaktika.ERP;
using Galaktika.ERP.Adapter.Atlantis;
using Galaktika.ESB.Core;
using Galaktika.ESB.ServiceAdapterApi.AttributeService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Restier.Core;
using Microsoft.Restier.Core.Model;
using Microsoft.Restier.Core.Submit;
using Microsoft.Restier.Providers.EntityFramework;

namespace Galaktika.ESB.Adapter.Atlantis.Api
{
    /// <summary>
    /// 
    /// </summary>
	public class DictionaryApi : EntityFrameworkApi<DictionaryApi.DictionaryContext>, IEsbApi
	{
        /// <summary>
        /// Creates temporary table
        /// </summary>
        /// <param name="dsql">Dsql request for creating temporary table</param>
        /// <returns>true if succeded, false if not</returns>
		public bool CreateTempTable(string dsql)
		{
			var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
			try
			{
				objMakeOrder = GalnetApi.InitCaller("M_MnPlan::IVipQuery", "M_MnPlan::VipQueryReal");
				var result = GalnetApi.FieldDef.Create(AtlTypes.Boolean);
				var resultStr = (bool)objMakeOrder.CallMethod("DsqlCreateTmpTable", result, dsql);
				return resultStr;
			}
			finally
			{
				objMakeOrder.MtCallDoneIfc();
			}
		}

        /// <summary>
        /// Configurates dictionary api
        /// </summary>
        /// <param name="apiType">Type of the api</param>
        /// <param name="services">Services collection</param>
        /// <returns>Collection of services</returns>
		public new static IServiceCollection ConfigureApi(Type apiType, IServiceCollection services)
		{
			services = EntityFrameworkApi<DictionaryContext>.ConfigureApi(apiType, services);
			services.AddService<IChangeSetInitializer>((sp, next) => new AtlantisChangeSetInitializer());
			services.AddService<IModelBuilder>((sp, next) => new AtlServices.AtlantisModelBuilder(sp, next));
			services.AddService<IModelMapper>((sp, next) => new AtlServices.AtlantisModelMapper(next));
			services.AddService<IAttributeService>((sp, next) => new AtlantisAttributeService(sp));
			return services;
		}

        /// <summary>
        /// Dictionary api constructor
        /// </summary>
        /// <param name="serviceProvider">Service provader</param>
		public DictionaryApi(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

        /// <summary>
        /// DictionaryContext
        /// </summary>
		public class DictionaryContext : AtlantisBaseContext
        {
        }
	}
}
