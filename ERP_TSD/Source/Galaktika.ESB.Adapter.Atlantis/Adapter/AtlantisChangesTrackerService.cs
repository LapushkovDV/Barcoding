using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Galaktika.ERP.Interop;
using Galaktika.ESB.Adapter.Atlantis.Api;
using Galaktika.ESB.Adapter.DbUpdates;
using Galaktika.ESB.Adapter.ERP.Adapter;
using Galaktika.ESB.Contract;
using Galaktika.ESB.Core;
using Galaktika.ESB.ServiceAdapterApi;
using Galaktika.ESB.Storage.Adapter;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace Galaktika.ESB.Adapter.Atlantis
{
	/// <summary>
	/// Updates service for adapter ERP. Checks database change and publishes them.
	/// </summary>
	public partial class AtlantisChangesTrackerService : ServiceBase, IEsbConsumersImplementation, IDbUpdatesParserService, IUpdatesService
	{
		private AtlMeta _atlMeta;
		private int _maxRecordChangesCountInChunk;
		private int _interval;

		private AtlMeta AtlMeta
		{
			get
			{
				if (_atlMeta == null)
				{
					using (var ctx = new DictionaryApi.DictionaryContext())
					{
						_atlMeta = ctx.GetMeta;
					}
				}
				return _atlMeta;
			}
		}

		/// <summary>
		/// Configures updates service
		/// </summary>
		/// <param name="services"></param>
		public override void Configure(IServiceCollection services)
		{
			base.Configure(services);
			services.AddSingleton(this);
		}


		#region Settings

		/// <summary>
		/// Count of changes in chunk to get
		/// </summary>
		public int MaxChangesCountInChunk
		{
			get
			{
				if (_maxRecordChangesCountInChunk < 1)
				{
					var sMaxRecordChangesCountInChunk = Settings["MaxRecordChangesCountInChunk"];
					if (sMaxRecordChangesCountInChunk == null)
						_maxRecordChangesCountInChunk = 10000;
					else
						_maxRecordChangesCountInChunk = (int)sMaxRecordChangesCountInChunk;
				}
				return _maxRecordChangesCountInChunk;
			}
		}

		/// <summary>
		/// Start's service and register consumer.
		/// </summary>
		/// <returns></returns>
		public override Task Start()
		{
			//Проверка на активную модель, если модель не активна (неччего отслеживать), то и консьюмер не нужен
			using (var ctx = new AdapterDbContext(LogService))
			{
				if (ctx.DbTrackingTables.Any(tt => tt.IsTrackingEnabled))
				{
					var bus = BusConnectionFactory.GetInstance();
					bus.RegisterConsumer(typeof(EntityForTrackingConsumer));
				}
			}

			return base.Start();
		}

		/// <summary>
		/// Interval to check updates
		/// </summary>
		public int UpdatesCheckingInterval
		{
			get
			{
				if (_interval == 0)
				{
					var sErpUpdatesServiceIntervalSec = Settings["ErpUpdatesServiceIntervalSec"];
					if (null == sErpUpdatesServiceIntervalSec)
						_interval = 10;
					else
						_interval = (int)sErpUpdatesServiceIntervalSec;
				}

				return _interval;
			}
		}

		#endregion

		#region Реализация получения настроек списка таблиц

		/// <summary>
		/// Types of consumers
		/// </summary>
		public Type[] ConsumerTypes => new[] { typeof(AppStoreListenerSettingsConsumer), typeof(TrackingTablesConsumer) };

		private IEnumerable<DbTrackingTable> _dbTables;

		/// <summary>
		/// List of tabels to track
		/// </summary>
		public IEnumerable<DbTrackingTable> DbTables
		{
			get
			{
				if (_dbTables == null)
					using (var context = new AdapterDbContext(LogService))
						_dbTables = context.DbTrackingTables.ToList();

				return _dbTables;
			}
		}

		/// <summary>
		/// Updating tracking tables
		/// </summary>
		/// <param name="dbTablesJArray">tables to update</param>
		public void UpdateTrackingTables(JArray dbTablesJArray)
		{
			try
			{
				using (var context = new AdapterDbContext(LogService))
				{
					var list = dbTablesJArray.Select(_ => new DbTrackingTable
					{
						TableName = _.Get<string>("Name"),
						PrimaryKeyField = _.Get<string>("PrimaryKey"),
						IsTrackingEnabled = _.Get<bool>("IsTrackingEnabled"),
						ModelId = _.Get<string>("ModelId")
					});

					context.DbTrackingTables.RemoveRange(context.DbTrackingTables);
					context.DbTrackingTables.AddRange(list);
					context.SaveChanges();

					//SetChangeTrackingForTables(_dbTables.Select(t => t.TableId));

					_dbTables = context.DbTrackingTables.ToList();
					//TrackedTables = null;
				}
			}
			catch (Exception ex)
			{
				LogService.Error(ex);
				throw;
			}
		}

		/// <summary>
		/// Gets JArray of tracking tables
		/// </summary>
		/// <param name="typesToListen">List with tracking type</param>
		/// <returns>JArray of tracking tables</returns>
		public Task<JArray> GetTrackingTables(List<TrackingType> typesToListen)
		{
			//await WaitHandle.WaitAsync();
			var resultJArray = new JArray();
			foreach (var trackingType in typesToListen)
			{
				FILE file;

				try
				{
					file = AtlMeta.GetFile(trackingType.AppEntityTypeName);
					if (file == null)
						throw new InvalidOperationException("file == null for TyepaName=" + trackingType);
				}
				catch (Exception e)
				{
					LogWarnTableIsNotFound(trackingType, e.Message);
					continue;
				}

				resultJArray.Add(new JObject
								{
										{"Name", file.XF_CODE.ToString()},
										{"PrimaryKey", "NREC"},
										{"IsTrackingEnabled", trackingType.IsTrackingEnabled}
								});
			}

			return Task.FromResult(resultJArray);
		}

		#endregion


		/// <summary>
		/// Checks if tracking is enabled
		/// </summary>
		/// <returns>true if tracking is enabled, false - it's not</returns>
		public bool CheckTracking()
		{
			return true;
		}

		/// <summary>
		/// Method checking updates 
		/// </summary>
		/// <returns></returns>
		public Task CheckDbUpdates()
		{
			return Task.CompletedTask;
		}

		/// <summary>
		/// Starts the timer for checking updates
		/// </summary>
		/// <returns></returns>
		public void StartTimer()
		{

		}
	}
}
