using System;
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
    /// Common part of atlantis api
    /// </summary>
	public partial class AtlantisApi : EntityFrameworkApi<AtlantisApi.AtlantisContext>, IEsbApi
	{
        /// <summary>
        /// Configurates atlantis api
        /// </summary>
        /// <param name="apiType">Type of the api</param>
        /// <param name="services">Services collection</param>
        /// <returns>Collection of services</returns>
		public new static IServiceCollection ConfigureApi(Type apiType, IServiceCollection services)
		{
			services = EntityFrameworkApi<AtlantisContext>.ConfigureApi(apiType, services);
			services.AddService<IChangeSetInitializer>((sp, next) => new AtlantisChangeSetInitializer());
			RegisterChangeSetInitializer(services);
			services.AddService<IModelBuilder>((sp, next) => new AtlServices.AtlantisModelBuilder(sp, next));
			services.AddService<IModelMapper>((sp, next) => new AtlServices.AtlantisModelMapper(next));
			services.AddService<IAttributeService>((sp, next) => new AtlantisAttributeService(sp));
			return services;
		}

        /// <summary>
        /// AtlantisApi constructor
        /// </summary>
        /// <param name="serviceProvider">Provider for services</param>
		public AtlantisApi(IServiceProvider serviceProvider)
				: base(serviceProvider)
		{
        }

		static partial void RegisterChangeSetInitializer(IServiceCollection services);

        /// <summary>
        /// AtlantisContext
        /// </summary>
		public partial class AtlantisContext : DictionaryApi.DictionaryContext
		{
        }

	}
}
