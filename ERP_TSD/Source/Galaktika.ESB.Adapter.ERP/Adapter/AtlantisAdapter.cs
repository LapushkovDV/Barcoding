using Galaktika.ESB.Storage.Service;
using Galaktika.ESB.ServiceAdapterApi;

namespace Galaktika.ESB.Adapter.Atlantis
{
	class AtlantisAdapter : BaseAdapter
	{
		public AtlantisAdapter(ServiceAdapter serviceAdapter) : base(serviceAdapter)
		{

			// перенесено в ErpODataAdapter.Configure() 
			//ServiceFactory.RegisterService<IRestierApiService, RestierService>(new RestierService(), true);
			//LoadSampleModels(this);
		}

		/// <summary>
		/// Return's name of the owner application
		/// </summary>
		public override string OwnerAppName => "ERP";

		/// <summary>
		/// Adapter kind
		/// </summary>
		public override string AdapterKind => "ERP";
	}
}
