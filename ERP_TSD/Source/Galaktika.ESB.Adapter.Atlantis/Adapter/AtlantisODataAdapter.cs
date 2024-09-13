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

namespace Galaktika.ESB.Adapter.Atlantis
{
	/// <summary>
	/// Ensure load ErpODataService, ESBServicesReg
	/// Register IRestierApiService, IExceptionCatcherService
	/// </summary>
	public partial class AtlantisODataAdapter : ServiceBase
	{
		/// <summary>
		/// Конфигурирует сервис
		/// </summary>
		/// <param name="services"></param>
		public override void Configure(IServiceCollection services)
		{
			base.Configure(services);
			services.Register<IRestierApiService, RestierService>(this);
			services.Register<IExceptionCatcherService, ExceptionCatcherService>(this);
			services.AddSingleton<AtlantisODataAdapter>(this);
		}
		/// <summary>
		/// Стартует сервис
		/// </summary>
		/// <returns></returns>
		public async override Task Start()
		{
		    if (this._AtlantisAdapter == null)
		    {
		        this._AtlantisAdapter = new AtlantisAdapter(this.Adapter);
		        this._AtlantisAdapter.Initialize();
		        this._AtlantisAdapter.Start();
                _Info_AtlantisAdapter_Adapter_is_started_OK();
            }
            await base.Start();

			if (this.AtlantisAdapter == null)
				throw new InvalidOperationException("this.AtlantisAdapter==null");

		}
		private AtlantisAdapter _AtlantisAdapter;
		internal AtlantisAdapter AtlantisAdapter
		{
			get
			{
				return this._AtlantisAdapter;
			}
		}

		/// <summary>
		/// Stop ErpODataAdapter
		/// </summary>
		/// <returns></returns>
		public override Task Stop()
		{
			AtlantisAdapter?.Stop();
			return base.Stop();
		}
		/// <summary>
		/// ErpODataAdapter depends of ErpODataService, ESBServicesReg
		/// </summary>
		public override IEnumerable<Type> DefaultRequiredServiceTypes
		{
			get
			{
				return new[] { typeof(AtlantisODataService), typeof(ESBServicesReg) };
			}
		}
	}
}

























