using System;
using System.Threading.Tasks;
using System.Web.OData.Extensions;
using Galaktika.ERP.Interop;
using Galaktika.ESB.Adapter.Services;
using Galaktika.ESB.ServiceAdapterApi;
using Microsoft.Restier.Publishers.OData;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Galaktika.ESB.Adapter.Atlantis.Api;

namespace Galaktika.ESB.Adapter.Atlantis
{
	/// <summary>
	/// Base class for ERP OData Restier provider service
	/// </summary>
	public partial class AtlantisODataService : ESBRestierProviderService
	{
		/// <summary>
		/// Configures service
		/// </summary>
		/// <param name="services"></param>
		public override void Configure(IServiceCollection services)
		{
			base.Configure(services);
			if (services.All(s => s.ServiceType != typeof(AtlantisODataService)))
				services.AddSingleton<AtlantisODataService>(this);
		}

		/// <summary>
		/// Starts service
		/// </summary>
		/// <returns></returns>
		public override Task Start()
		{
			var result = base.Start();

			if (this.GetRestierServiceProvider() == null)
				throw new InvalidOperationException("this.AtlantisServiceProvider==null");

			return result;
		}


		/// <summary>
		/// List of required services
		/// </summary>
		public override IEnumerable<Type> DefaultRequiredServiceTypes
		{
			get { yield return typeof(AtlantisStarterService); }
		}


		/// <summary>
		/// Service provider
		/// </summary>
		public override IServiceProvider RestierServiceProvider
		{
			get { return AtlantisServiceProvider; }
		}

		private ConcurrentDictionary<string, IServiceProvider> _rcm;

		/// <summary>
		/// Service providers
		/// </summary>
		public override ConcurrentDictionary<string, IServiceProvider> RestierServiceProviders
		{
			get
			{
				RegisterOData();
				_rcm = (ConcurrentDictionary<string, IServiceProvider>)this.Adapter.DefaultHttpHandler.Configuration
						.Properties["System.Web.OData.RootContainerMappingsKey"];
				return _rcm;
			}
		}

		private bool _isODataRegistered;

		private void RegisterOData()
		{
			if (!_isODataRegistered)
			{
				var server = Adapter.DefaultHttpHandler;
				var config = server.Configuration;
				config.Filter().Expand().Select().OrderBy().MaxTop(null).Count();
				config.SetUseVerboseErrors(true);
				RegisterODataEF7(server);
				// RegisterODataEF6(server);
				_isODataRegistered = true;
			}
		}

		private IServiceProvider _AtlantisServiceProvider;

		private IServiceProvider AtlantisServiceProvider
		{
			get
			{
				if (_AtlantisServiceProvider == null)
				{
					RegisterOData();
					var rcm = (ConcurrentDictionary<string, IServiceProvider>)this.Adapter.DefaultHttpHandler.Configuration.Properties["System.Web.OData.RootContainerMappingsKey"];
					this._AtlantisServiceProvider = rcm["AtlantisApi"];

                    _Info_ERP_Odata_is_started_OK();
                }
				return this._AtlantisServiceProvider;
			}
		}

		/// <summary>
		/// Registers oData
		/// </summary>
		/// <param name="server"></param>
		/// <param name="startPath"></param>
		protected virtual void RegisterODataEF7(HttpServer server, string startPath = "")
		{
			var _api = "api";

			if (string.IsNullOrEmpty(startPath))
				startPath = _api;
			else
				startPath += "/" + _api;

			server.RegisterOData<DictionaryApi>(startPath, this);
			server.RegisterOData<AtlantisApi>(startPath, this);
			//server.RegisterOData<PlanApi>(startPath);

			server.Configuration.Routes.MapHttpRoute(
					name: "Default" + startPath.Replace('/', '_'),

					routeTemplate: startPath + "/{controller}/{id}",
					defaults: new { id = RouteParameter.Optional }
				);
		}

		//protected virtual void RegisterODataEF6(HttpServer server, string startPath = "")
		//{
		// var _api = "api6";

		// if (string.IsNullOrEmpty(startPath))
		//  startPath = _api;
		// else
		//  startPath += "/" + _api;

		// server.RegisterOData<AtlantisApi6>(startPath);

		// server.Configuration.Routes.MapHttpRoute(
		//  name: "Default" + startPath.Replace('/', '_'),

		//  routeTemplate: startPath + "/{controller}/{id}",
		//  defaults: new { id = RouteParameter.Optional }
		// );
		//}

	}
}

























